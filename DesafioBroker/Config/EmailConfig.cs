using Newtonsoft.Json;

namespace DesafioBroker.Config
{
    public class EmailConfig : IConfig
    {
        public const string EMAIL_CONFIG_FILE_NAME = "EmailConfig.json";

        class EmailConfigJson
        {
            public string receiverEmail;

            public EmailConfigJson()
            {
                receiverEmail = string.Empty;
            }
        }

        EmailConfigJson configJson;

        bool isLoaded;

        public string ReceiverEmail { get => configJson.receiverEmail; }

        public EmailConfig()
        {
            configJson = new EmailConfigJson();
            isLoaded = false;
        }

        public void Save()
        {
            using (StreamWriter file = new StreamWriter(GetFullPath()))
            {
                string stringJson = JsonConvert.SerializeObject(configJson);
                file.Write(stringJson);
            }
        }

        public void Load(bool createDefault = false)
        {
            if (!File.Exists(GetFullPath()) && createDefault)
            {
                Save();
            }

            using (StreamReader file = new StreamReader(GetFullPath()))
            {
                string stringJson = file.ReadToEnd();
                configJson = JsonConvert.DeserializeObject<EmailConfigJson>(stringJson) ?? new EmailConfigJson();
                isLoaded = true;
            }
        }

        public bool IsLoaded() { return isLoaded; }

        public string GetFullPath()
        {
            return Path.Join(Program.CONFIG_FILE_PATH, EMAIL_CONFIG_FILE_NAME);
        }
    }
}
