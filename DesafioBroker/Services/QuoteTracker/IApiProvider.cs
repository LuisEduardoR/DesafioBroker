namespace DesafioBroker.Services.QuoteTracker
{
    internal interface IApiProvider
    {
        Task<float?> Quote(string asset);
    }
}
