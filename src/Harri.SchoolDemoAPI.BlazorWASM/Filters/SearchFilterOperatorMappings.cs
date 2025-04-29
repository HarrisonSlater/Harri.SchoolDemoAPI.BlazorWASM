using Harri.SchoolDemoAPI.Models.Dto;
using MudBlazor;

namespace Harri.SchoolDemoAPI.BlazorWASM.Filters
{
    public class SearchFilterOperatorMappings
    {
        /// <returns>
        /// Mapping from MudBlazor operators to SchoolDemoApi Query DTO
        /// </returns>
        /// <see cref="Constants.SearchFilters.Students.GPAFilterOperators" />
        /// <see cref="Harri.SchoolDemoAPI.Models.Dto.GPAQueryDto" />
        public static GPAQueryDto? GetGPAQueryDto(StudentSearchFilters filters)
        {
            //This guard returns null except for Number.Empty and Number.NotEmpty
            if (filters.ParsedGPAFilter is null && 
                (filters.GPAFilter?.Operator != FilterOperator.Number.Empty) && (filters.GPAFilter?.Operator != FilterOperator.Number.NotEmpty)) return null;

            return filters.GPAFilter?.Operator switch
            {
                FilterOperator.Number.Equal => new GPAQueryDto() { GPA = new() { Eq = filters.ParsedGPAFilter } },
                FilterOperator.Number.Empty => new GPAQueryDto() { GPA = new() { IsNull = true } },
                FilterOperator.Number.NotEmpty => new GPAQueryDto() { GPA = new() { IsNull = false } },
                FilterOperator.Number.GreaterThan => new GPAQueryDto() { GPA = new() { Gt = filters.ParsedGPAFilter } },
                FilterOperator.Number.LessThan => new GPAQueryDto() { GPA = new() { Lt = filters.ParsedGPAFilter } },
                _ => throw new ArgumentOutOfRangeException(nameof(filters.GPAFilter.Operator), $"Unexpected GPAFilter.Operator value: {filters.GPAFilter?.Operator}"),
            };
        }
    }
}
