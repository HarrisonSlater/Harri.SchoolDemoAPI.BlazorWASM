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

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    [TestFixture]
    public class ViewStudentsTests : BunitTestContext
    {
        private const string SuccessAlertSelector = "#student-success-alert";
        public const string IdDataCellsSelector = "td[data-label=\"Student ID\"]";
        public const string NameDataCellsSelector = "td[data-label=\"Name\"]";
        public const string GPADataCellsSelector = "td[data-label=\"GPA\"]";

        //These filters are selected via class because MudBlazor doesn't seem to have a way to set id's for the default filter implementation
        public const string StudentSearchSIdSelector = ".filter-input-sid.filter-header-cell input";

        public const string StudentSearchNameSelector = ".filter-input-student-name.filter-header-cell input";

        public const string StudentSearchGPASelector = ".filter-input-gpa.filter-header-cell input";

        private const string ErrorInputsSelector = ".mud-input-control.mud-input-error";

        private const string ErrorAlertSelector = "#student-error-alert";

        private Mock<IStudentApi> _mockStudentApiClient = new Mock<IStudentApi>();
        private List<StudentDto>? _mockExistingStudents;

        private List<string?> _expectedSIds = [];
        private List<string?> _expectedNames = [];
        private List<string?> _expectedGpas = [];

        [SetUp]
        public void SetUp()
        {
            _mockStudentApiClient = new Mock<IStudentApi>();

            Services.AddSingleton(_mockStudentApiClient.Object);

            Services.AddMudServices();
            JSInterop.Mode = JSRuntimeMode.Loose; // Ignores mudblazor JS calls

            TestContext!.RenderTree.Add<MainLayout>();
        }

        private List<StudentDto>? SetUpMockExistingStudents()
        {
            _mockExistingStudents = new List<StudentDto>() {
                new StudentDto()
                {
                    SId = 1,
                    Name = "Test Existing Student 1",
                    GPA = 1.11m
                },
                new StudentDto()
                {
                    SId = 2,
                    Name = "Test Existing Student 2",
                    GPA = 2.22m
                },
                new StudentDto()
                {
                    SId = 3,
                    Name = "Test Existing Student 3",
                    GPA = 3.33m
                }};

            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), It.IsAny<int?>(), It.IsAny<int?>()))
                .Returns(Task.FromResult(new RestSharp.RestResponse<PagedList<StudentDto>>(new RestSharp.RestRequest())
                {
                    IsSuccessStatusCode = true,
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Data = new PagedList<StudentDto>() { Items = _mockExistingStudents, Page = 1, PageSize = 10, TotalCount = 3 }
                }));
            _expectedSIds = _mockExistingStudents.Select(x => x.SId.ToString()).ToList();
            _expectedNames = _mockExistingStudents.Select(x => x.Name).ToList();
            _expectedGpas = _mockExistingStudents.Select(x => x.GPA.ToString()).ToList();

            return _mockExistingStudents;
        }

        [Test]
        public void ViewStudents_RendersSuccessfully()
        {
            // Arrange
            SetUpMockExistingStudents();

            // Act
            var studentsPage = RenderComponent<Students>();

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            VerifyDefaultGetStudentsRestResponseCalled();
        }

        [Test]
        public void ViewStudents_ShowsErrorOnFail()
        {
            // Arrange

            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse(null, null, null, null, null, 1, 15))
                .Returns(Task.FromResult(new RestSharp.RestResponse<PagedList<StudentDto>>(new RestSharp.RestRequest())
                {
                    Data = null
                }));

            // Act
            var studentsPage = RenderComponent<Students>();

            // Assert
            studentsPage.WaitForElement(ErrorAlertSelector);

            VerifyDefaultGetStudentsRestResponseCalled();
        }

        [Test]
        public void ViewStudents_RendersWithSuccessIdAlert()
        {
            // Arrange
            SetUpMockExistingStudents();
            var studentSuccessId = 123;

            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.NavigateTo($"?successId={studentSuccessId}");

            // Act
            var studentsPage = RenderComponent<Students>(parameters => parameters.Add(p => p.PageNumber, 1));

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);
            var successAlert = studentsPage.Find(SuccessAlertSelector);
            successAlert.TextContent.Should().Contain(studentSuccessId.ToString());

            VerifyDefaultGetStudentsRestResponseCalled();
        }

        [Test]
        public void ViewStudents_HandlesInvalidSuccessIdAlert()
        {
            // Arrange
            SetUpMockExistingStudents();
            var studentSuccessId = "asdfasdf";

            var navMan = Services.GetRequiredService<FakeNavigationManager>();
            navMan.NavigateTo($"?successId={studentSuccessId}");

            // Act
            var studentsPage = RenderComponent<Students>(parameters => parameters.Add(p => p.PageNumber, 1));

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);
            var successAlert = studentsPage.Find(SuccessAlertSelector);
            successAlert.TextContent.Should().Contain(studentSuccessId.ToString());

            VerifyDefaultGetStudentsRestResponseCalled();
        }

        [TestCase("Test Existing Student")]
        [TestCase("  ")]
        [TestCase("")]
        public void ViewStudents_SearchFeatureShouldMatchAllStudents(string searchString)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(StudentSearchNameSelector);
            searchField.Input(searchString);

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            studentsPage.WaitForAssertion(() =>
            {
                _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(It.IsAny<int?>(), It.IsAny<string?>(), It.IsAny<GPAQueryDto?>(), It.IsAny<SortOrder?>(), It.IsAny<string?>(), 1, 15), Times.Exactly(2));
            });

            studentsPage.Instance.Filters!.ParsedNameFilter.Should().Be(searchString);
        }

        [TestCase("10", 10)]
        [TestCase("1024", 1024)]
        public void ViewStudents_SearchFeature_ShouldFilterBySId(string searchString, int? parsedInt)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(StudentSearchSIdSelector);
            searchField.Input(searchString);

            // Assert
            AssertGetStudentsRestResponseCalled(studentsPage, () => VerifyGetStudentsRestResponseCalledWithSId());

            studentsPage.Instance.Filters!.ParsedSIdFilter.Should().Be(parsedInt);
        }

        [TestCase("Test Student")]
        public void ViewStudents_SearchFeature_ShouldFilterByName(string searchString)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(StudentSearchNameSelector);
            searchField.Input(searchString);

            // Assert
            AssertGetStudentsRestResponseCalled(studentsPage, () => VerifyGetStudentsRestResponseCalledWithName());

            studentsPage.Instance.Filters!.ParsedNameFilter.Should().Be(searchString);
        }

        [TestCase("3.95", 3.95d)]
        [TestCase("0", 0d)]
        public void ViewStudents_SearchFeature_ShouldFilterByGPA(string searchString, decimal? parsedGPA)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(StudentSearchGPASelector);
            searchField.Input(searchString);

            // Assert
            AssertGetStudentsRestResponseCalled(studentsPage, () => VerifyGetStudentsRestResponseCalledWithGPA());

            studentsPage.Instance.Filters!.ParsedGPAFilter.Should().Be(parsedGPA);
        }

        // Search feature filters should correctly call back end
        private void AssertGetStudentsRestResponseCalled(IRenderedComponent<Students> studentsPage, Action extraVerify)
        {
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            studentsPage.WaitForAssertion(() =>
            {
                VerifyDefaultGetStudentsRestResponseCalled();
                extraVerify.Invoke();
                _mockStudentApiClient.Invocations.Count().Should().Be(2);
            });

            ShouldSeeExpectedStudentsInGrid(studentsPage);

            studentsPage.FindAll(ErrorInputsSelector).Should().BeEmpty();
        }

        private void VerifyDefaultGetStudentsRestResponseCalled(Times? times = null)
        {
            if (times is null) times = Times.Once();

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(null, null, null, null, null, 1, 15), times.Value);
        }

        private void VerifyGetStudentsRestResponseCalledWithSId(Times? times = null)
        {
            if (times is null) times = Times.Once();

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(It.IsNotNull<int>(), null, null, null, null, 1, 15), times.Value);
        }

        private void VerifyGetStudentsRestResponseCalledWithName(Times? times = null)
        {
            if (times is null) times = Times.Once();

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(null, It.IsNotNull<string>(), null, null, null, 1, 15), times.Value);
        }

        private void VerifyGetStudentsRestResponseCalledWithGPA(Times? times = null)
        {
            if (times is null) times = Times.Once();

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(null, null, It.IsNotNull<GPAQueryDto>(), null, null, 1, 15), times.Value);
        }

        private void ShouldSeeExpectedStudentsInGrid(IRenderedComponent<Students> studentsPage)
        {
            SelectorShouldHaveText(studentsPage, IdDataCellsSelector, _expectedSIds);
            SelectorShouldHaveText(studentsPage, NameDataCellsSelector, _expectedNames);
            SelectorShouldHaveText(studentsPage, GPADataCellsSelector, _expectedGpas);
        }

        private void SelectorShouldHaveText(IRenderedComponent<Students> studentsPage, string selector, List<string?> expectedText)
        {
            var elements = studentsPage.FindAll(selector).ToList();

            elements.Should().HaveCount(expectedText.Count);

            var elementTexts = elements.Select(x => x.GetInnerText()).ToList();

            elementTexts.Should().BeEquivalentTo(expectedText);
        }
    }
}
