using DesafioBroker.Config;

namespace DesafioBroker.Services.QuoteTracker
{
    internal class QuoteTrackerService : IService
    {
        const string DEFAULT_PROVIDER_STRING = "default";

        AssetConfig assetConfig;
        IApiProvider apiProvider;
        EmailService emailService;

        bool isRunning;
        int stopSignal;

        public QuoteTrackerService(AssetConfig assetConfig, ApiConfig apiConfig, EmailService emailService)
        {
            isRunning = false;
            stopSignal = 0;

            if (!assetConfig.IsLoaded() || !apiConfig.IsLoaded())
            {
                throw new Exception("Config not loaded");
            }

            this.assetConfig = assetConfig;
            this.apiProvider = LoadAPIService(apiConfig);
            this.emailService = emailService;
        }

        public void Run()
        {
            isRunning = true;
            Program.ThreadSafeWriteLine("Quote tracker service is running!");

            while (!ShouldStopExecution())
            {
                DateTime quoteTime = DateTime.Now;

                Task<float?> quoteTask = apiProvider.Quote(assetConfig.AssetToTrack);
                Task.WaitAll(quoteTask);

                if (quoteTask.Result.HasValue)
                {
                    WriteServiceMessage($"{quoteTime} - {assetConfig.AssetToTrack} = {quoteTask.Result.Value}");
                    ProcessQuotePrice(quoteTask.Result.Value);
                }

                while (!ShouldStopExecution() && (DateTime.Now - quoteTime).TotalMilliseconds < 1000.0f * assetConfig.RequestDelay) 
                { 
                    Thread.Sleep(1000);
                }
            }

            isRunning = false;
            Program.ThreadSafeWriteLine("Quote tracker service has stopped!");
        }

        public void Stop()
        {
            Interlocked.Increment(ref stopSignal);
        }

        bool ShouldStopExecution()
        {
            return stopSignal != 0;
        }

        public bool IsRunning()
        {
            return isRunning;
        }

        void ProcessQuotePrice(float quotePrice)
        {
            if (!emailService.IsRunning())
            {
                WriteServiceMessage("Unable to send email notification - Service is not running!");
                return;
            }

            if (quotePrice > assetConfig.MaxPrice)
            {
                emailService.Notify(EmailService.EmailNotificationType.AboveMaxPrice, quotePrice);
            } 
            else if (quotePrice < assetConfig.MinPrice)
            {
                emailService.Notify(EmailService.EmailNotificationType.BelowMinPrice, quotePrice);
            }
        }

        void WriteServiceMessage(string message)
        {
            string messageTitle = "# Quote tracker service message:";
            string messageBody = $"# {message}";
            string separator = new String('#', Math.Max(messageTitle.Length, messageBody.Length));
            Program.ThreadSafeWriteLines( new string[] { "", separator, messageTitle, messageBody, separator });
        }

        IApiProvider LoadAPIService(ApiConfig config)
        {
            switch (config.Provider.ToLower())
            {
                case DEFAULT_PROVIDER_STRING:
                case AlphaVantageProvider.PROVIDER_NAME:
                    return new AlphaVantageProvider(config);
                default:
                    throw new ArgumentException($"Provider {config.Provider} is not supported, configure a new one in {config.GetFullPath()}");
            }
        }
    }
}
