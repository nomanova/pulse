using ErrorOr;

namespace Pulse.Domain.Common.Errors;

public static partial class DomainErrors
{
    public static class Workflow
    {
        public static readonly Error NoSteps =
            Error.Validation("Workflow.NoSteps", "The workflow must have at least one step.");

        public static readonly Error InvalidOrder =
            Error.Validation("Workflow.InvalidOrder", "Invalid order.");
        
        public static readonly Error UnknownStep =
            Error.Validation("Workflow.UnknownStep", "Unknown step.");
        
        public static readonly Error VersionNotDraft =
            Error.Validation("Workflow.VersionNotDraft", "Version not draft.");
        
        public static readonly Error VersionNotPublished = 
            Error.Validation("Workflow.VersionNotPublished", "Version not published.");
        
        public static readonly Error DraftAlreadyExists = 
            Error.Validation("Workflow.DraftAlreadyExists", "Draft exists.");
        
        public static readonly Error NoDraftVersion = 
            Error.Validation("Workflow.NoDraftVersion", "No draft version.");
        
        public static readonly Error NoPublishedVersion = 
            Error.Validation("Workflow.NoPublishedVersion", "No published version.");
    }
}