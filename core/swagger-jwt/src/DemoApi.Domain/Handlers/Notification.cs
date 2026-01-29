namespace DemoApi.Domain.Handlers;

public class Notification
{
    #region Constructors

    public Notification(string message)
    {
        Message = message;
    }

    #endregion

    #region Properties

    public string Message { get; }

    #endregion
}