namespace Pulse.Api.Ctrl.Contract.Applications;

public class DeleteApplicationRequest
{
    public string? OrganizationName { get; set; }
    
    public string? ApplicationName { get; set; }
}