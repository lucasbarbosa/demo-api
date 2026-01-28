namespace DemoApi.Domain.Interfaces;

using DemoApi.Domain.Handlers;

public interface INotificatorHandler
{
    bool HasErrors();

    List<Notification> GetErrors();

    void AddError(string error);
}