using Newtonsoft.Json;

namespace DesafioBroker.Config
{
    public class AssetConfig : IConfig
    {
        public const string ASSET_CONFIG_FILE_NAME = "AssetConfig.json";

        class AssetConfigJson {
            public string assetToTrack;

            public float maxPrice;
            public float minPrice;

            public AssetConfigJson(string assetToTrack, float maxPrice, float minPrice) {
                this.assetToTrack = assetToTrack;

                this.maxPrice = maxPrice;
                this.minPrice = minPrice;
            }

            public AssetConfigJson() : this(string.Empty, float.MaxValue, 0) { }
        }

        AssetConfigJson configJson;

        bool isLoaded;

        public string AssetToTrack { get => configJson.assetToTrack; }

        public float MaxPrice { get => configJson.maxPrice; }
        public float MinPrice { get => configJson.minPrice; }

        public AssetConfig ()
        {
            configJson = new AssetConfigJson();
            isLoaded = false;
        }

        public AssetConfig (string assetToTrack, float maxPrice, float minPrice)
        {
            configJson = new AssetConfigJson(assetToTrack, maxPrice, minPrice);
            isLoaded = true;
        }

        public AssetConfig (string[] args) : this(args[0], float.Parse(args[1]), float.Parse(args[2])) { }

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
            if(!File.Exists(GetFullPath()) && createDefault)
            {
                Save();
            }

            using (StreamReader file = new StreamReader(GetFullPath()))
            {
                string stringJson = file.ReadToEnd();
                configJson = JsonConvert.DeserializeObject<AssetConfigJson>(stringJson) ?? new AssetConfigJson();
                isLoaded = true;
            }
        }

        public bool IsLoaded() { return isLoaded; }

        public string GetFullPath()
        {
            return Path.Join(Program.CONFIG_FILE_PATH, ASSET_CONFIG_FILE_NAME);
        }
    }
}
