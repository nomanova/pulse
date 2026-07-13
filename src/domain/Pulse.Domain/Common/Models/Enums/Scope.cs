namespace Pulse.Domain.Common.Models.Enums;

/**
 * Numbering on Scope is important as it is used for authorization purposes.
 * The scopes should be ordered from most-to-least specific.
 */
public enum Scope
{
    Environment = 0,
    Application = 1,
    Organization = 2,
    Server = 3
}