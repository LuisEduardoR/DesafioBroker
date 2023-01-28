using DesafioBroker.Config;

namespace DesafioBroker.Services
{
    internal class EmailService : IService
    {
        public enum EmailNotificationType
        {
            None,
            AboveMaxPrice,
            BelowMinPrice
        }

        bool isRunning;

        public EmailService(EmailConfig config)
        {
            if (!config.IsLoaded())
            {
                throw new Exception("Config not loaded");
            }

            isRunning = false;
        }

        public void Run()
        {
            isRunning = true;

            Program.ThreadSafeWriteLine("Email service is running!");

            isRunning = false;
            Program.ThreadSafeWriteLine("Email service has stopped!");
        }

        public void Stop()
        {
            isRunning = false;
        }

        public bool IsRunning()
        {
            return isRunning;
        }

        public void Notify(EmailNotificationType type, float quotePrice)
        {

        }
    }
}
