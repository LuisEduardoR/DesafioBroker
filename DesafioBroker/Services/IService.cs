namespace DesafioBroker.Services
{
    internal interface IService
    {
        void Run();

        void Stop();

        bool IsRunning();
    }
}
