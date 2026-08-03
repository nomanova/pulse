using Microsoft.EntityFrameworkCore;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Memberships;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Aggregates.Roles;
using Pulse.Domain.Aggregates.Users;
using Pulse.Domain.Aggregates.WorkflowInstances;
using Pulse.Domain.Aggregates.Workflows;
using Pulse.Domain.Aggregates.Workflows.Entities;

namespace Pulse.App.Common.Database;

public interface IDatabaseContext
{
    DbSet<User> Users { get; }
    
    DbSet<Organization> Organizations { get; }
    
    DbSet<Application> Applications { get; }
    
    DbSet<Environment> Environments { get; }
    
    DbSet<Membership> Memberships { get; }
    
    DbSet<Role> Roles { get; }
    
    DbSet<Workflow> Workflows { get; init; }
    
    DbSet<WorkflowVersion> WorkflowVersions { get; init; }
    
    DbSet<WorkflowInstance> WorkflowInstances { get; init; }
}