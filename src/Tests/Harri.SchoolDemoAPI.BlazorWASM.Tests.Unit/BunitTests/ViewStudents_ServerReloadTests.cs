using AngleSharp.Dom;
using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Pages;
using Harri.SchoolDemoAPI.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using Moq;
using MudBlazor.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using Harri.SchoolDemoAPI.Models.Enums;
using Bunit.Extensions;
using Bunit;
using Harri.SchoolDemoAPI.BlazorWASM.Layout;
using MudBlazor.Interop;
using MudBlazor;
using Bunit.Extensions.WaitForHelpers;
using Harri.SchoolDemoAPI.BlazorWASM.Filters;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    [TestFixture]
    public class ViewStudents_ServerReloadTests : BunitTestContext
    {
        private Mock<IStudentApi> _mockStudentApiClient;

        [SetUp]
        public void SetUp()
        {
            _mockStudentApiClient = new Mock<IStudentApi>();

            Services.AddSingleton(_mockStudentApiClient.Object);

            Services.AddMudServices();
            JSInterop.Mode = JSRuntimeMode.Loose; // Ignore mudblazor JS calls

            TestContext!.RenderTree.Add<MainLayout>();
        }

        private static IEnumerable<TestCaseData> FilterValuesTestCases()
        {
            yield return new TestCaseData(null, null);
            yield return new TestCaseData(101, null);
            yield return new TestCaseData(null, "Test Student");
            yield return new TestCaseData(101, "Test Student");
        }

        [TestCaseSource(nameof(FilterValuesTestCases))]
        public async Task ViewStudents_ServerReload_ShouldHandleBasicFilterValues(int? parsedSIdFilter, string? parsedNameFilter)
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(Task.FromResult(new RestSharp.RestResponse<PagedList<StudentDto>>(new RestSharp.RestRequest())
                {
                    IsSuccessStatusCode = true,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Data = new PagedList<StudentDto>() { Items = new(), TotalCount = 0 }
                }));

            var studentsPage = RenderComponent<Students>();

            var gridState = new GridState<StudentDto>() { Page = 0, PageSize = 15 };
            var parsedFilters = new StudentSearchFilters()
            {
                ParsedSIdFilter = parsedSIdFilter,
                ParsedNameFilter = parsedNameFilter,
                //ParsedGPAFilter = parsedGPAFilter, 
            };

            _mockStudentApiClient.Invocations.Clear(); //Ignore the first call made by instantiating the component

            // Act
            await studentsPage.Instance.ServerReload(gridState, parsedFilters);

            // Assert
            var mockSequence = new MockSequence();
            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(parsedSIdFilter, parsedNameFilter, It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), 1, 15), Times.Once());
        }

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
        public async Task ViewStudents_ServerReload_ShouldHandleGPAFilterValuesAndOperators(decimal? parsedGPAFilter, string selectedOperator, GPAQueryDto? expectedGPAQueryDto)
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(Task.FromResult(new RestSharp.RestResponse<PagedList<StudentDto>>(new RestSharp.RestRequest())
                {
                    IsSuccessStatusCode = true,
                    StatusCode = System.Net.HttpStatusCode.NotFound,
                    Data = new PagedList<StudentDto>() { Items = new(), TotalCount = 0 }
                }));

            var studentsPage = RenderComponent<Students>();
            var mockFilterDefinition = new Mock<IFilterDefinition<StudentDto>>();
            mockFilterDefinition.Setup(x => x.Operator).Returns(selectedOperator);

            var gridState = new GridState<StudentDto>() { Page = 0, PageSize = 15 };
            var parsedFilters = new StudentSearchFilters()
            {
                ParsedGPAFilter = parsedGPAFilter,
                GPAFilter = mockFilterDefinition.Object
            };

            _mockStudentApiClient.Invocations.Clear(); //Ignore the first call made by instantiating the component

            // Act
            await studentsPage.Instance.ServerReload(gridState, parsedFilters);

            // Assert
            var shouldBeNull = expectedGPAQueryDto ?? It.IsNotNull<GPAQueryDto?>();

            if (expectedGPAQueryDto is null)
            {
                _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), null, It.IsAny<SortOrder?>(), It.IsAny<string?>(), 1, 15), Times.Once());
            }
            else
            {
                _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsNotNull<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), 1, 15), Times.Once());
                var gpaQueryDtoActual = _mockStudentApiClient.Invocations.Single().Arguments[2] as GPAQueryDto;
                gpaQueryDtoActual.Should().BeEquivalentTo(expectedGPAQueryDto);
            }
        }
        // TODO ViewStudents_ServerReload_ShouldHandleStudentApiResponses
    }
}
