using ErrorOr;
using FluentValidation;
using Pulse.App.Common.Dispatcher;
using Pulse.App.Common.Validation;
using Pulse.App.Dto.Common;

namespace Pulse.App.Common.Requests;

public interface ISearchQuery
{
    public const uint DefaultPageSize = 20;
    public const uint MaximumPageSize = 100;
}

public record SearchQuery<T> : IQuery<ErrorOr<PagedSearchResultDto<T>>>
{
    public string? Query { get; init; }

    public string? LastId { get; init; }

    public uint PageSize { get; init; }

    public bool? Ascending { get; init; }

    public string? OrderBy { get; init; }
}

public class SearchQueryValidator<T, Ts> : AbstractValidator<T> where T : SearchQuery<Ts>
{
    protected SearchQueryValidator()
    {
        RuleFor(query => query.LastId)
            .ValidIdentity()
            .When(query => query.LastId != null);

        RuleFor(query => query.PageSize)
            .GreaterThan((uint)0);

        RuleFor(query => query.PageSize)
            .LessThanOrEqualTo(ISearchQuery.MaximumPageSize);
    }
}