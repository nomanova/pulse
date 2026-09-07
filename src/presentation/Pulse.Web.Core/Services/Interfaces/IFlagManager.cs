namespace Pulse.Web.Core.Services.Interfaces;

public interface IFlagManager
{
    void RaiseSuccess(string content);

    void RaiseWarning(string content);
    
    void RaiseError(string content);
}