using Newtonsoft.Json;

namespace DesafioBroker.Config
{
    public class AssetConfig : IConfig
    {
        public const string ASSET_CONFIG_FILE_NAME = "AssetConfig.json";

        class AssetConfigJson
        {
            public string assetToTrack;

            public float maxPrice;
            public float minPrice;

            public float requestDelay; // In seconds

            public AssetConfigJson(string assetToTrack, float maxPrice, float minPrice, float requestDelay)
            {
                this.assetToTrack = assetToTrack;

                this.maxPrice = maxPrice;
                this.minPrice = minPrice;

                this.requestDelay = requestDelay;
            }

            public AssetConfigJson() : this(string.Empty, 100000.0f, 0.0f, 60.0f) { }
        }

        AssetConfigJson configJson;

        bool isLoaded;

        public string AssetToTrack
        { 
            get
            {
                if (!IsLoaded()) 
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.assetToTrack;
            }
        }

        public float MaxPrice
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.maxPrice;
            }
        }
        public float MinPrice
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.minPrice;
            }
        }

        public float RequestDelay
        {
            get
            {
                if (!IsLoaded())
                {
                    throw new Exception("Config not loaded");
                }
                return configJson.requestDelay;
            }
        }

        public AssetConfig ()
        {
            configJson = new AssetConfigJson();
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
                configJson = JsonConvert.DeserializeObject<AssetConfigJson>(stringJson) ?? new AssetConfigJson();
                ValidateConfig();
                isLoaded = true;
            }
        }

        public void LoadFromArgs(string[] args)
        {
            configJson.assetToTrack = args[0];
            configJson.maxPrice = float.Parse(args[1]);
            configJson.minPrice = float.Parse(args[2]);
            ValidateConfig();
            isLoaded = true;
        }

        public bool IsLoaded()
        { 
            return isLoaded;
        }

        public string GetFullPath()
        {
            return Path.Join(Program.CONFIG_FILE_PATH, ASSET_CONFIG_FILE_NAME);
        }

        void ValidateConfig()
        {
            if (configJson.assetToTrack == string.Empty)
            {
                throw new ArgumentException($"\"assetToTrack\" in {GetFullPath()} must not be empty");
            }

            if (configJson.maxPrice <= 0.0f)
            {
                throw new ArgumentException($"\"maxPrice\" in {GetFullPath()}  must be greater than zero");
            }

            if (configJson.minPrice <= 0.0f)
            {
                throw new ArgumentException($"\"minPrice\" in {GetFullPath()}  must be greater than zero");
            }

            if (configJson.requestDelay <= 0.0f)
            {
                throw new ArgumentException($"\"requestDelay\" in {GetFullPath()} must be greater than zero");
            }
        }
    }
}
