namespace APIConnectorCore.Services

{
    public interface ICurrencyService
    {
        
        Task<decimal> ConvertToZAR(decimal amount, string fromCurrency);
    }
}

