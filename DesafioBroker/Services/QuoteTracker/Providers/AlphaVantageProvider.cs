using DesafioBroker.Config;
using System.Text.Json.Nodes;

namespace DesafioBroker.Services.QuoteTracker.Providers
{
    internal class AlphaVantageProvider : IApiProvider
    {
        public const string PROVIDER_NAME = "alphavantage";

        string key;

        public AlphaVantageProvider(ApiConfig config)
        {
            key = config.Key;
        }

        public async Task<float?> Quote(string asset)
        {
            // For alphavantage we add .SA to the end of the asset name for it to work with B3 assets.
            Uri queryUri = new Uri($"https://www.alphavantage.co/query?function=GLOBAL_QUOTE&symbol={asset}.SA&apikey={key}");
            using (HttpClient client = new HttpClient())
            {
                Task<HttpResponseMessage> request = client.GetAsync(queryUri);

                await request;

                using (HttpResponseMessage response = request.Result)
                {
                    using (HttpContent content = response.Content)
                    {
                        Task<string> jsonReader = content.ReadAsStringAsync();

                        await jsonReader;

                        string jsonString = jsonReader.Result.Replace("{\n    \"Global Quote\":", "");
                        jsonString = jsonString.Remove(jsonString.Length - 1);
                        JsonNode? jsonNode = JsonNode.Parse(jsonString);

                        if (jsonNode == null)
                        {
                            return null;
                        }

                        string? priceString = jsonNode["05. price"]?.ToString();
                        return priceString != null ? float.Parse(priceString) : null;
                    }
                }
            }
        }
    }
}
