namespace Pulse.App.Common.Security.Interfaces;

public interface IUserClaimProvider
{
    string Id { get; }

    string SecurityStamp { get; }
}