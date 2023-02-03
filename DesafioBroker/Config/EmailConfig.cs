using Newtonsoft.Json;

namespace DesafioBroker.Config
{
    public class EmailConfig : IConfig
    {
        public const string EMAIL_CONFIG_FILE_NAME = "EmailConfig.json";

        const string DEFAULT_CLIENT = "default";
        const string DEFAULT_EMAIL = "example@mail.com";
        const string DEFAULT_PASSWORD = "password";
        const string DEFAULT_DISPLAY_NAME = "Quote Tracker";

        class EmailConfigJson
        {
            public string smtpClient;

            public string senderEmail;
            public string senderPassword;
            public string senderDisplayName;

            public string receiverEmail;

            public EmailConfigJson()
            {
                smtpClient = DEFAULT_CLIENT;

                senderEmail = DEFAULT_EMAIL;
                senderPassword = DEFAULT_PASSWORD;
                senderDisplayName = DEFAULT_DISPLAY_NAME;

                receiverEmail = DEFAULT_EMAIL;
            }
        }

        EmailConfigJson configJson;

        bool isLoaded;

        public string SmtpClient
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.smtpClient;
            }
        }

        public string SenderEmail
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.senderEmail;
            }
        }

        public string SenderPassword
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.senderPassword;
            }
        }

        public string SenderDisplayName
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.senderDisplayName;
            }
        }

        public string ReceiverEmail {
            get 
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.receiverEmail;
            }
        }

        public EmailConfig()
        {
            configJson = new EmailConfigJson();
            isLoaded = false;
        }

        public void Save()
        {
            using (StreamWriter file = new StreamWriter(GetFullPath()))
            {
                string stringJson = JsonConvert.SerializeObject(configJson, Formatting.Indented);
                file.Write(stringJson);
            }
        }

        public void Load()
        {
            using (StreamReader file = new StreamReader(GetFullPath()))
            {
                string stringJson = file.ReadToEnd();
                configJson = JsonConvert.DeserializeObject<EmailConfigJson>(stringJson) ?? new EmailConfigJson();
                isLoaded = true;
            }
        }

        public bool IsLoaded()
        { 
            return isLoaded;
        }

        public string GetFullPath()
        {
            return Path.Join(Program.CONFIG_FILE_PATH, EMAIL_CONFIG_FILE_NAME);
        }
    }
}
