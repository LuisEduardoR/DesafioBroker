using DesafioBroker.Config;

namespace DesafioBroker.Services.QuoteTracker.Providers
{
    // This provider will oscillate the price repeatedly between 22.57 and 22.68 for testing the notification system.
    internal class TestProvider : IApiProvider
    {
        public const string PROVIDER_NAME = "test";

        const int QUOTE_DELAY = 120; // In milliseconds

        static readonly float[] PRICE_CURVE = { 22.650f, 22.665f, 22.675f, 22.680f, 22.675f, 22.665f, 22.650f, 22.625f, 22.575f, 22.570f, 22.575f, 22.625f };

        int currentIndex;

        public TestProvider(ApiConfig config)
        {
            currentIndex = 0;
        }

        public async Task<float?> Quote(string asset)
        {
            await Task.Delay(QUOTE_DELAY);

            float quotePrice = PRICE_CURVE[currentIndex];

            currentIndex = (currentIndex + 1) % PRICE_CURVE.Length;

            return quotePrice;
        }
    }
}
