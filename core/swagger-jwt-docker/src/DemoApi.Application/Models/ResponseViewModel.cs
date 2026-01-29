namespace DemoApi.Application.Models;

public class ResponseViewModel : BaseViewModel
{
    #region Properties

    public bool Success { get; set; }

    public object? Data { get; set; }

    public IList<string> Errors { get; set; } = [];

    #endregion
}