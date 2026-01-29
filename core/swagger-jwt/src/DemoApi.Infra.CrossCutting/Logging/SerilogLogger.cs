using DemoApi.Infra.CrossCutting.Interfaces;
using Serilog;

namespace DemoApi.Infra.CrossCutting.Logging;

public class SerilogLogger : DemoApi.Infra.CrossCutting.Interfaces.ILogger
{
    #region Properties

    private readonly Serilog.ILogger _appExLogger;

    #endregion

    #region Constructors

    public SerilogLogger()
    {
        _appExLogger = Log.ForContext("SourceContext", "AppExceptionLog");
    }

    #endregion

    #region Public Methods

    public void LogException(Exception ex)
    {
        _appExLogger.Error(ex, ex.Message);
    }

    public void LogException(Exception ex, string message)
    {
        _appExLogger.Error(ex, message);
    }

    #endregion
}