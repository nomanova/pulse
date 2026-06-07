using System;
using System.Collections.Generic;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Roles;

public partial class Role
{
    public static class BuiltIn
    {
        private static readonly RoleId OrgOwnerRoleId = RoleId.New(
            IdentityProvider.New(new Guid("00000000-0000-0000-0001-000000000001")));

        private static readonly RoleId AppOwnerRoleId = RoleId.New(
            IdentityProvider.New(new Guid("00000000-0000-0000-0002-000000000001")));

        private static readonly RoleId EnvOwnerRoleId = RoleId.New(
            IdentityProvider.New(new Guid("00000000-0000-0000-0003-000000000001")));

        public static readonly Role OrgOwner = new(
            OrgOwnerRoleId, true, Scope.Organization, "Organization Owner");

        public static readonly Role AppOwner = new(
            AppOwnerRoleId, true, Scope.Application, "Application Owner");

        public static readonly Role EnvOwner = new(
            EnvOwnerRoleId, true, Scope.Environment, "Environment Owner");

        public static readonly IReadOnlyCollection<Role> All =
        [
            OrgOwner,
            AppOwner,
            EnvOwner
        ];
    }
}