using DemoApi.Domain.Handlers;

namespace DemoApi.Domain.Interfaces;

public interface INotificatorHandler
{
    bool HasErrors();

    List<Notification> GetErrors();

    void AddError(string error);

    void AddErrors(params IEnumerable<string> errors);
}