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
using RestSharp;
using System.Net;

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
                .Returns(Task.FromResult(new RestResponse<PagedList<StudentDto>>(new RestRequest())
                {
                    IsSuccessStatusCode = true,
                    StatusCode = HttpStatusCode.NotFound,
                    Data = new PagedList<StudentDto>() { Items = new(), TotalCount = 0 }
                }));

            var studentsPage = RenderComponent<Students>();

            var gridState = new GridState<StudentDto>() { Page = 0, PageSize = 15 };
            var parsedFilters = new StudentSearchFilters()
            {
                ParsedSIdFilter = parsedSIdFilter,
                ParsedNameFilter = parsedNameFilter,
            };

            _mockStudentApiClient.Invocations.Clear(); //Ignore the first call made by instantiating the component

            // Act
            await studentsPage.Instance.ServerReload(gridState, parsedFilters);

            // Assert
            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(parsedSIdFilter, parsedNameFilter, It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), 1, 15), Times.Once());
        }

        [Test]
        public async Task ViewStudents_ServerReload_ShouldHandleGPAFilterValuesAndOperators()
        {
            // Arrange
            decimal? parsedGPAFilter = 4m;
            string selectedOperator = FilterOperator.Number.Equal;
            GPAQueryDto? expectedGPAQueryDto = new GPAQueryDto() { GPA = new() { Eq = 4m } };

            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(Task.FromResult(new RestResponse<PagedList<StudentDto>>(new RestRequest())
                {
                    IsSuccessStatusCode = true,
                    StatusCode = HttpStatusCode.NotFound,
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

            _mockStudentApiClient.Invocations.Clear(); 

            // Act
            await studentsPage.Instance.ServerReload(gridState, parsedFilters);

            // Assert

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsNotNull<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), 1, 15), Times.Once());
            var gpaQueryDtoActual = _mockStudentApiClient.Invocations.Single().Arguments[2] as GPAQueryDto;
            gpaQueryDtoActual.Should().BeEquivalentTo(expectedGPAQueryDto);
        }

        private static RestResponse<PagedList<StudentDto>> GetTestRestResponse(HttpStatusCode statusCode, bool isSuccess = true)
        {
            var data = new PagedList<StudentDto>() { Items = [new(), new(), new()], TotalCount = 3, Page = 1, PageSize = 3 };

            return new RestResponse<PagedList<StudentDto>>(new RestRequest() { })
            {
                StatusCode = statusCode,
                IsSuccessStatusCode = isSuccess,
                Data = isSuccess ? data : null
            };
        }

        private static IEnumerable<TestCaseData> StudentApiResponseTestCases()
        {
            yield return new TestCaseData(GetTestRestResponse(HttpStatusCode.OK), false, 3);
            yield return new TestCaseData(GetTestRestResponse(HttpStatusCode.NotFound, false), false, 0);
            yield return new TestCaseData(GetTestRestResponse(HttpStatusCode.BadRequest, false), true, 0);
            yield return new TestCaseData(GetTestRestResponse(HttpStatusCode.InternalServerError, false), true, 0);

            var nullDataTestResponse = GetTestRestResponse(HttpStatusCode.OK);
            nullDataTestResponse.Data = null;
            yield return new TestCaseData(nullDataTestResponse, true, 0);
        }

        [TestCaseSource(nameof(StudentApiResponseTestCases))]
        public async Task ViewStudents_ServerReload_ShouldHandleStudentApiResponses(RestResponse<PagedList<StudentDto>> restResponse, bool shouldShowError, int expectedTotalCount)
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(Task.FromResult(restResponse));

            var studentsPage = RenderComponent<Students>();

            var gridState = new GridState<StudentDto>() { Page = 0, PageSize = 15 };

            _mockStudentApiClient.Invocations.Clear(); 

            // Act
            var gridData = await studentsPage.Instance.ServerReload(gridState, new StudentSearchFilters());

            // Assert
            studentsPage.Instance.ShowError.Should().Be(shouldShowError);
            gridData.TotalItems.Should().Be(expectedTotalCount);
        }
    }
}
