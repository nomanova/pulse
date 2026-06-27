using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Users;

namespace Pulse.App.Tests.Framework.Contexts;

public sealed record OrganizationContext(string Password, User User, Organization Organization);