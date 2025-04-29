using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Components;
using Harri.SchoolDemoAPI.BlazorWASM.Filters;
using Harri.SchoolDemoAPI.Models.Dto;
using Moq;
using MudBlazor;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit
{
    [TestFixture]
    public class SearchFilterOperatorMappingsTests
    {

        /// <see cref="Constants.SearchFilters.Students.GPAFilterOperators"/>
        private static IEnumerable<TestCaseData> GPAFilterValuesTestCases()
        {
            yield return new TestCaseData(null, FilterOperator.Number.Equal, null);
            yield return new TestCaseData(null, FilterOperator.Number.GreaterThan, null);
            yield return new TestCaseData(null, FilterOperator.Number.LessThan, null);
            yield return new TestCaseData(null, FilterOperator.Number.Empty, new GPAQueryDto() { GPA = new() { IsNull = true } });
            yield return new TestCaseData(0m, FilterOperator.Number.Equal, new GPAQueryDto() { GPA = new() { Eq = 0m } });
            yield return new TestCaseData(4m, FilterOperator.Number.Equal, new GPAQueryDto() { GPA = new() { Eq = 4m } });
            yield return new TestCaseData(3.47m, FilterOperator.Number.Equal, new GPAQueryDto() { GPA = new() { Eq = 3.47m } });
            yield return new TestCaseData(0m, FilterOperator.Number.GreaterThan, new GPAQueryDto() { GPA = new() { Gt = 0m } });
            yield return new TestCaseData(4m, FilterOperator.Number.GreaterThan, new GPAQueryDto() { GPA = new() { Gt = 4m } });
            yield return new TestCaseData(3.47m, FilterOperator.Number.GreaterThan, new GPAQueryDto() { GPA = new() { Gt = 3.47m } });
            yield return new TestCaseData(0m, FilterOperator.Number.LessThan, new GPAQueryDto() { GPA = new() { Lt = 0m } });
            yield return new TestCaseData(4m, FilterOperator.Number.LessThan, new GPAQueryDto() { GPA = new() { Lt = 4m } });
            yield return new TestCaseData(3.47m, FilterOperator.Number.LessThan, new GPAQueryDto() { GPA = new() { Lt = 3.47m } });
        }

        [TestCaseSource(nameof(GPAFilterValuesTestCases))]
        public void GetGPAQueryDto_ShouldMapCorrectly(decimal? parsedGPAFilter, string selectedOperator, GPAQueryDto? expectedGPAQueryDt)
        {
            // Arrange
            var mockFilterDefinition = new Mock<IFilterDefinition<StudentDto>>();
            mockFilterDefinition.Setup(x => x.Operator).Returns(selectedOperator);

            var gridState = new GridState<StudentDto>() { Page = 0, PageSize = 15 };
            var parsedFilters = new StudentSearchFilters()
            {
                ParsedGPAFilter = parsedGPAFilter,
                GPAFilter = mockFilterDefinition.Object
            };

            // Act
            var gpaQueryDto = SearchFilterOperatorMappings.GetGPAQueryDto(parsedFilters);

            // Assert
            gpaQueryDto.Should().BeEquivalentTo(expectedGPAQueryDt);
        }

        private static IEnumerable<TestCaseData> GPAFilterAllOperatorsTestCases()
        {
            foreach (var gpaOperator in Constants.SearchFilters.Students.GPAFilterOperators)
            {
                yield return new TestCaseData(gpaOperator);
            }
        }

        [TestCaseSource(nameof(GPAFilterAllOperatorsTestCases))]
        public void GetGPAQueryDto_ShouldHandleAllOperators(string? gpaOperator)
        {
            // Arrange
            var mockFilterDefinition = new Mock<IFilterDefinition<StudentDto>>();
            mockFilterDefinition.Setup(x => x.Operator).Returns(gpaOperator);
            var parsedFilters = new StudentSearchFilters()
            {
                ParsedGPAFilter = 4,
                GPAFilter = mockFilterDefinition.Object
            };

            // Act

            var action = () => SearchFilterOperatorMappings.GetGPAQueryDto(parsedFilters);

            // Assert
            action.Should().NotThrow();
        }
    }
}
