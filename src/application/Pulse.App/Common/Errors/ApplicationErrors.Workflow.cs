using ErrorOr;

namespace Pulse.App.Common.Errors;

public static partial class ApplicationErrors
{
    public static class Workflow
    {
        public static readonly Error ProviderMissing = Error.Validation(
            "Workflow.ProviderNotFound", "No provider for the requested channel is available");
    }
}