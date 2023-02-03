using DesafioBroker.Config;
using System.Net.Mail;

namespace DesafioBroker.Services
{
    internal class EmailService : IService
    {
        const string SERVICE_NAME = "Email";

        const int SMTP_SSL_PORT = 587;

        const string GMAIL_SMTP_CLIENT = "smtp.gmail.com";

        const string DEFAULT_SMTP_CLIENT_STRING = "default";
        const string DEFAULT_SMTP_CLIENT = GMAIL_SMTP_CLIENT;

        public enum EmailNotificationType
        {
            None,
            Sell,
            Buy
        }

        EmailConfig emailConfig;
        SmtpClient smtpClient;

        Semaphore emailSemaphore;

        Queue<MailMessage> emailQueue;

        bool isRunning;
        int stopSignal;

        public EmailService(EmailConfig config)
        {
            if (!config.IsLoaded())
            {
                throw new Exception("Config not loaded");
            }

            emailConfig = config;
            smtpClient = CreateSmtpClient();

            emailSemaphore = new Semaphore(0, 1);
            emailQueue = new Queue<MailMessage>();
            emailSemaphore.Release();

            isRunning = false;
            stopSignal = 0;
        }

        public void Run()
        {
            isRunning = true;

            Program.ThreadSafeWriteLine($"{SERVICE_NAME} service is running!");

            while (!ShouldStopExecution())
            {
                MailMessage? emailToSend = null;

                emailSemaphore.WaitOne();      
                if (emailQueue.Count > 0)
                {
                    emailToSend = emailQueue.Dequeue();
                }
                emailSemaphore.Release();

                if (emailToSend != null)
                {
                    SendEmail(emailToSend);
                }

                Thread.Sleep(1000);
            }

            isRunning = false;
            Program.ThreadSafeWriteLine($"{SERVICE_NAME} service has stopped!");
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

        SmtpClient CreateSmtpClient()
        {
            string client;
            if (emailConfig.SmtpClient == DEFAULT_SMTP_CLIENT_STRING)
            {
                client = DEFAULT_SMTP_CLIENT;
            } 
            else
            {
                client = emailConfig.SmtpClient;
            }

            SmtpClient smtpClient = new SmtpClient(client);

            smtpClient.Port = SMTP_SSL_PORT;
            smtpClient.EnableSsl = true;

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;


            smtpClient.Credentials = new System.Net.NetworkCredential(emailConfig.SenderEmail, emailConfig.SenderPassword);

            return smtpClient;
        }

        public void Notify(EmailNotificationType type, string quotedAsset, float quotePrice)
        {
            string order;
            string message;

            switch (type)
            {
                case EmailNotificationType.Sell:
                    order = $"{quotedAsset} Sell Order";
                    message = $"Sell order for {quotedAsset} at ${quotePrice}";
                    break;
                case EmailNotificationType.Buy:
                    order = $"{quotedAsset} Buy Order";
                    message = $"Buy order for {quotedAsset} at ${quotePrice}";
                    break;
                default:
                    throw new InvalidOperationException($"Can't send an email notification of type {type}");
            }

            string subject = $"[Desafio broker] {order}";
            string htmlBody = $"<h1>{subject}</h1><div>{DateTime.Now} - {message}</div>";

            MailMessage email = CreateEmail(subject, htmlBody);

            emailSemaphore.WaitOne();
            emailQueue.Enqueue(email);
            emailSemaphore.Release();
        }

        MailMessage CreateEmail(string subject, string htmlBody)
        {
            MailMessage email = new MailMessage();

            email.From = new MailAddress(emailConfig.SenderEmail, emailConfig.SenderDisplayName);
            email.To.Add(emailConfig.ReceiverEmail);

            email.Subject = subject;

            email.IsBodyHtml = true;
            email.Body = htmlBody;

            return email;
        }

        void SendEmail(MailMessage emailToSend)
        {
            try
            {
                smtpClient.Send(emailToSend);

                Program.WriteServiceMessage(SERVICE_NAME, $"{DateTime.Now} - Email notification sent");
            } 
            catch (SmtpException e)
            {
                string errorMessage = $"{DateTime.Now} - Email notification error: {e.Message} ({e.StatusCode})";

                if (e.StatusCode == SmtpStatusCode.MustIssueStartTlsFirst && emailConfig.SmtpClient == GMAIL_SMTP_CLIENT)
                {
                    Program.WriteServiceMessages(SERVICE_NAME, new string[]{ errorMessage, "Are you using gmail? Configure an App Password: https://support.google.com/accounts/answer/185833?hl=en" });
                } 
                else
                {
                    Program.WriteServiceMessage(SERVICE_NAME, errorMessage);
                }
            }
        }
    }
}
