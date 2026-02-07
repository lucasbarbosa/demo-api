namespace DemoApi.Domain.Handlers;

public class Notification(string message)
{

    #region Constructors

    #endregion

    #region Properties

    public string Message { get; } = message;

    #endregion
}