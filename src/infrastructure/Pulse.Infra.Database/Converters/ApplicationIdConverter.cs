using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pulse.Domain.Aggregates.Applications;

namespace Pulse.Infra.Database.Converters;

public sealed class ApplicationIdConverter : ValueConverter<ApplicationId, string>
{
    public ApplicationIdConverter()
        : base(
            absenceId => absenceId.Value,
            value => ApplicationId.New(value))
    {
    }
}