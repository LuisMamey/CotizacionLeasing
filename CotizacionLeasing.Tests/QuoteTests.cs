using System;
using System.Linq;
using FluentValidation;
using Xunit;
using Moq;
using CotizacionLeasing.Domain.Entities;
using CotizacionLeasing.Domain.Exceptions;
using CotizacionLeasing.Application.DTOs;
using CotizacionLeasing.Application.Validators;
using CotizacionLeasing.Application.Services;
using CotizacionLeasing.Infrastructure.Interfaces;
using CotizacionLeasing.Infrastructure.Repositories;

namespace CotizacionLeasing.Tests
{
    public class QuoteRequestValidatorTests
    {
        private readonly QuoteRequestValidator _validator = new();

        [Fact]
        public void ValidRequest_ShouldNotHaveErrors()
        {
            var dto = new QuoteRequestDto
            {
                ClientName  = "ACME",
                Price       = 100_000m,
                DownPayment = 10_000m,   // 10% for 12 months
                TermMonths  = 12,
                Residual    = 20_000m,   // 20% < 30%
                AnnualRate  = 0.12
            };

            var result = _validator.Validate(dto);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void ResidualTooHigh_ShouldHaveValidationError()
        {
            var dto = new QuoteRequestDto
            {
                ClientName  = "ACME",
                Price       = 100_000m,
                DownPayment = 10_000m,
                TermMonths  = 12,
                Residual    = 35_000m,   // 35% > 30%
                AnnualRate  = 0.10
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == nameof(dto.Residual));
        }

        [Theory]
        [InlineData(12, 9000.0, "Para 12 meses")]
        [InlineData(18, 7000.0, "Para 13–23 meses")]
        [InlineData(36, 4000.0, "Para más de 24 meses")]
        public void DownPaymentTooLow_ShouldHaveValidationError(int term, double dpDouble, string containsMsg)
        {
            // Convertimos el double a decimal para construir el DTO
            decimal dp = Convert.ToDecimal(dpDouble);

            var dto = new QuoteRequestDto
            {
                ClientName  = "ACME",
                Price       = 100_000m,
                DownPayment = dp,
                TermMonths  = term,
                Residual    = 10_000m,
                AnnualRate  = 0.08
            };

            var result = _validator.Validate(dto);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.ErrorMessage.Contains(containsMsg));
        }
    }

    public class QuoteDomainTests
    {
        [Fact]
        public void ValidQuote_ShouldCalculatePositiveMonthlyPayment()
        {
            var client = new Client("ACME");
            var quote  = new Quote(client, 120_000m, 12_000m, 12, 20_000m, 0.10);

            Assert.True(quote.MonthlyPayment > 0);
        }

        [Fact]
        public void ResidualRuleViolation_ShouldThrowBusinessRuleException() =>
            Assert.Throws<BusinessRuleException>(() =>
                new Quote(new Client("X"), 100_000m, 10_000m, 12, 40_000m, 0.1));

        [Fact]
        public void DownPaymentRule12MonthsViolation_ShouldThrowBusinessRuleException() =>
            Assert.Throws<BusinessRuleException>(() =>
                new Quote(new Client("X"), 100_000m, 5_000m, 12, 10_000m, 0.1));

        [Fact]
        public void DownPaymentRule13To23MonthsViolation_ShouldThrowBusinessRuleException() =>
            Assert.Throws<BusinessRuleException>(() =>
                new Quote(new Client("X"), 100_000m, 6_000m, 18, 10_000m, 0.1));

        [Fact]
        public void DownPaymentRuleGreater24MonthsViolation_ShouldThrowBusinessRuleException() =>
            Assert.Throws<BusinessRuleException>(() =>
                new Quote(new Client("X"), 100_000m, 4_000m, 36, 10_000m, 0.1));
    }

    public class QuoteServiceTests
    {
        [Fact]
        public void CalculateQuote_ReturnsDtoWithCorrectClientAndId()
        {
            var repoMock = new Mock<IQuoteRepository>();
            var service  = new QuoteService(repoMock.Object);

            var dto = new QuoteRequestDto
            {
                ClientName  = "TestCo",
                Price       = 80_000m,
                DownPayment = 8_000m,
                TermMonths  = 12,
                Residual    = 16_000m,
                AnnualRate  = 0.10
            };

            var result = service.CalculateQuote(dto);

            Assert.Equal("TestCo", result.ClientName);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.True(result.MonthlyPayment > 0);
        }

        [Fact]
        public void SaveQuote_CallsRepositoryAdd()
        {
            var repoMock = new Mock<IQuoteRepository>();
            var service  = new QuoteService(repoMock.Object);

            var dto = new QuoteRequestDto
            {
                ClientName  = "TestCo",
                Price       = 50_000m,
                DownPayment = 5_000m,
                TermMonths  = 12,
                Residual    = 10_000m,
                AnnualRate  = 0.08
            };

            service.SaveQuote(dto);

            repoMock.Verify(r => r.Add(It.IsAny<Quote>()), Times.Once);
        }

        [Fact]
        public void GetQuotesByClient_ReturnsMappedResponseDtos()
        {
            var stored = new[]
            {
                new Quote(new Client("X"), 100_000m, 10_000m, 12, 20_000m, 0.1),
                new Quote(new Client("X"), 200_000m, 20_000m, 12, 40_000m, 0.1)
            };
            var repoMock = new Mock<IQuoteRepository>();
            repoMock.Setup(r => r.GetByClientName("X")).Returns(stored);

            var service = new QuoteService(repoMock.Object);
            var results = service.GetQuotesByClient("X").ToList();

            Assert.Equal(2, results.Count);
            Assert.All(results, dto => Assert.Equal("X", dto.ClientName));
        }
    }

    public class InMemoryQuoteRepositoryTests
    {
        [Fact]
        public void AddAndGetByClientName_ShouldReturnStoredQuotes()
        {
            var repo = new InMemoryQuoteRepository();
            var q1   = new Quote(new Client("A"), 10_000m, 1_000m, 12, 2_000m, 0.05);
            var q2   = new Quote(new Client("A"), 20_000m, 2_000m, 12, 4_000m, 0.05);

            repo.Add(q1);
            repo.Add(q2);

            var list = repo.GetByClientName("A").ToList();
            Assert.Equal(2, list.Count);
            Assert.Contains(q1, list);
            Assert.Contains(q2, list);
        }
    }
}
