namespace DesafioBroker.Config
{
    public interface IConfig
    {
        string GetFullPath();
        void Save();
        void Load();
        bool IsLoaded();
    }
}
