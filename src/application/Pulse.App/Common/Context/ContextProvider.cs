using Pulse.App.Common.Exceptions;
using Pulse.Domain.Aggregates.Applications;
using Pulse.Domain.Aggregates.Environments;
using Pulse.Domain.Aggregates.Organizations;
using Pulse.Domain.Common.Models.Enums;

namespace Pulse.App.Common.Context;

public sealed class ContextProvider : IContextProvider
{
    private Scope? _scope;

    public Scope Scope
    {
        get => _scope ?? throw new ContextException("Scope is not set.");
        set
        {
            if (_scope != null)
            {
                throw new ContextException("Scope is already set.");
            }

            _scope = value;
        }
    }

    public Organization Organization
    {
        get => field == null ? throw new ContextException("Organization is not set.") : field;
        set
        {
            if (field != null)
            {
                throw new ContextException("Organization is already set.");
            }

            field = value;
        }
    }

    public Application Application
    {
        get => field == null ? throw new ContextException("Application is not set.") : field;
        set
        {
            if (field != null)
            {
                throw new ContextException("Application is already set.");
            }

            field = value;
        }
    }

    public Environment Environment
    {
        get => field == null ? throw new ContextException("Environment is not set.") : field;
        set
        {
            if (field != null)
            {
                throw new ContextException("Environment is already set.");
            }

            field = value;
        }
    }
}