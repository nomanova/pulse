using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Tests.Framework.Contexts;

public sealed record UserContext(string Password, User User);