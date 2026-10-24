using System;
using System.Collections.Generic;
using Pulse.Domain.Common.Errors;
using Pulse.Domain.Common.Models.Enums;
using Pulse.Domain.Common.Models.ValueObjects;
using Pulse.Domain.Common.Services;

namespace Pulse.Domain.Aggregates.Roles;

public partial class Role
{
    public static class BuiltIn
    {
        private static readonly RoleId SrvOwnerRoleId = RoleId.New(
            IdentityProvider.New(new Guid("00000000-0000-0000-0001-000000000001")));
        
        private static readonly RoleId OrgOwnerRoleId = RoleId.New(
            IdentityProvider.New(new Guid("00000000-0000-0000-0002-000000000001")));

        private static readonly RoleId AppOwnerRoleId = RoleId.New(
            IdentityProvider.New(new Guid("00000000-0000-0000-0003-000000000001")));

        private static readonly RoleId EnvOwnerRoleId = RoleId.New(
            IdentityProvider.New(new Guid("00000000-0000-0000-0004-000000000001")));

        public static readonly Role SrvOwner = new(
            SrvOwnerRoleId, 
            true, 
            Scope.Server, 
            ObjectName.Create("Server Owner").Assert(), 
            ObjectKey.Create("srv-owner").Assert());
        
        public static readonly Role OrgOwner = new(
            OrgOwnerRoleId, 
            true, 
            Scope.Organization,
            ObjectName.Create("Organization Owner").Assert(),
            ObjectKey.Create("org-owner").Assert());

        public static readonly Role AppOwner = new(
            AppOwnerRoleId, 
            true, 
            Scope.Application, 
            ObjectName.Create("Application Owner").Assert(), 
            ObjectKey.Create("app-owner").Assert());

        public static readonly Role EnvOwner = new(
            EnvOwnerRoleId, 
            true, 
            Scope.Environment, 
            ObjectName.Create("Environment Owner").Assert(), 
            ObjectKey.Create("env-owner").Assert());

        public static readonly IReadOnlyCollection<Role> All =
        [
            SrvOwner,
            OrgOwner,
            AppOwner,
            EnvOwner
        ];
    }
}