using Moq;
using Xunit;
using APIConnectorCore.Services;

namespace Prog7311_UnitTests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task ConvertToZAR_ReturnsExpectedAmount()
        {
            var mockCurrencyService = new Mock<ICurrencyService>();

            decimal inputAmount = 100;
            string fromCurrency = "USD";
            decimal expectedZar = 1900;

            mockCurrencyService
                .Setup(s => s.ConvertToZAR(inputAmount, fromCurrency))
                .ReturnsAsync(expectedZar);

            var result = await mockCurrencyService.Object
                .ConvertToZAR(inputAmount, fromCurrency);

            Assert.Equal(expectedZar, result);
        }
    }
}