using DemoApi.Domain.Interfaces;

namespace DemoApi.Domain.Handlers;

public class NotificatorHandler : INotificatorHandler
{
    #region Properties

    private readonly List<Notification> _errors;

    #endregion

    #region Constructors

    public NotificatorHandler() => _errors = [];

    #endregion

    #region Public Methods

    public bool HasErrors()
    {
        return _errors.Any();
    }

    public void AddError(string error)
    {
        _errors.Add(new Notification(error));
    }

    public List<Notification> GetErrors()
    {
        return _errors;
    }

    #endregion
}