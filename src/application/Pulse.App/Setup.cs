using System.Reflection;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Pulse.App.Common.Authorization;
using Pulse.App.Common.Context;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Security;
using Pulse.App.Common.Security.Interfaces;
using Pulse.App.Common.Validation;
using Pulse.App.Handlers.Applications.Common;
using Pulse.App.Handlers.Environments.Common;
using Pulse.App.Handlers.Memberships.Common;
using Pulse.App.Handlers.Organizations.Common;
using Pulse.App.Handlers.Roles.Common;
using Pulse.App.Handlers.Users.Common;
using Pulse.App.Handlers.WorkflowInstances.Common;
using Pulse.App.Handlers.Workflows.Common;

namespace Pulse.App;

public interface IAssemblyReference;

public static class Setup
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddApplication()
        {
            services.AddRequestPipeline();
            services.AddRepositories();
            services.AddServices();
        
            return services;
        }

        private void AddRequestPipeline()
        {
            services.AddDispatcher(Assembly.GetExecutingAssembly());

            // Validators
            services.AddPipelineBehavior(typeof(ValidationBehavior<,>));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            
            // Context
            services.AddPipelineBehavior(typeof(ContextProviderBehavior<,>));
            
            // Authorizers
            services.AddPipelineBehavior(typeof(AuthorizationBehavior<,>));
            services.AddAuthorizersFromAssembly(Assembly.GetExecutingAssembly());
            services.AddAuthorizationRequirementsFromAssembly(Assembly.GetExecutingAssembly());
        }

        private void AddRepositories()
        {
            // (Write) repositories (aggregate roots)
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IEnvironmentRepository, EnvironmentRepository>();
        
            services.AddScoped<IMembershipRepository, MembershipRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            
            services.AddScoped<IWorkflowRepository, WorkflowRepository>();
            services.AddScoped<IWorkflowInstanceRepository, WorkflowInstanceRepository>();
        }

        private void AddServices()
        {
            services.AddScoped<IUserProvider, UserProvider>();
            services.AddScoped<IContextProvider, ContextProvider>();
            
            services.AddScoped<IWorkflowStepExecutor, WorkflowStepExecutor>();
        }
    }
}