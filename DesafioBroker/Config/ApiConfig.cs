using Newtonsoft.Json;

namespace DesafioBroker.Config
{
    public class ApiConfig : IConfig
    {
        public const string API_CONFIG_FILE_NAME = "ApiConfig.json";

        const string DEFAULT_PROVIDER = "default";
        const string DEFAULT_KEY = "api-key";

        class ApiConfigJson
        {
            public string provider;
            public string key;

            public ApiConfigJson()
            {
                provider = DEFAULT_PROVIDER;
                key = DEFAULT_KEY;
            }
        }

        ApiConfigJson configJson;

        bool isLoaded;

        public string Provider
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.provider;
            }
        }

        public string Key
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.key;
            }
        }

        public ApiConfig()
        {
            configJson = new ApiConfigJson();
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

        public void Load(bool createDefault = false)
        {
            if (!File.Exists(GetFullPath()) && createDefault)
            {
                Save();
            }

            using (StreamReader file = new StreamReader(GetFullPath()))
            {
                string stringJson = file.ReadToEnd();
                configJson = JsonConvert.DeserializeObject<ApiConfigJson>(stringJson) ?? new ApiConfigJson();
                isLoaded = true;
            }
        }

        public bool IsLoaded()
        {
            return isLoaded;
        }

        public string GetFullPath()
        {
            return Path.Join(Program.CONFIG_FILE_PATH, API_CONFIG_FILE_NAME);
        }
    }
}
