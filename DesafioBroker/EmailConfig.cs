using Newtonsoft.Json;

namespace DesafioBroker
{
    internal class EmailConfig
    {
        public const string DEFAULT_CONFIG =
            "{\n" +
            "    email: \"\"" +
            "\n}";

        class EmailJsonConfig
        {
            public string receiverEmail;

            public EmailJsonConfig()
            {
               receiverEmail = string.Empty;
            }
        }

        EmailJsonConfig jsonConfig;

        public string ReceiverEmail { get => jsonConfig.receiverEmail; }

        public EmailConfig(string path)
        {
            using (StreamReader file = new StreamReader(path))
            {
                string json = file.ReadToEnd();
                jsonConfig = JsonConvert.DeserializeObject<EmailJsonConfig>(json) ?? new EmailJsonConfig();
            }
        }
    }
}
