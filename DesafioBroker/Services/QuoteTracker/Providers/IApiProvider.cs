namespace DesafioBroker.Services.QuoteTracker.Providers
{
    internal interface IApiProvider
    {
        Task<float?> Quote(string asset);
    }
}
