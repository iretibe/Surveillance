namespace Surveillance.Logging
{
    public interface IAppLogger
    {
        void Info(string message);
        void Error(string message);
    }
}
