namespace DesafioBroker
{
    internal class AssetConfig
    {
        public string AssetToTrack { get; private set; }

        public float MaxPrice { get; private set; }
        public float MinPrice { get; private set; }

        public AssetConfig(string assetToTrack, float maxPrice, float minPrice) 
        {
            AssetToTrack = assetToTrack;
            MaxPrice = maxPrice;
            MinPrice = minPrice;
        }

        public AssetConfig(string[] args) : this(args[0], float.Parse(args[1]), float.Parse(args[2])) { }
    }
}
