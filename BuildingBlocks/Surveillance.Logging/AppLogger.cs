using Serilog;

namespace Surveillance.Logging
{
    public class AppLogger : IAppLogger
    {
        public void Info(string message)
            => Log.Information(message);

        public void Error(string message)
            => Log.Error(message);
    }
}
