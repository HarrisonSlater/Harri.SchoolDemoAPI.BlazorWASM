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

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit.BunitTests
{
    [TestFixture]
    public class ViewStudentsTests : BunitTestContext
    {
        private const string SuccessAlertSelector = "#student-success-alert";
        public const string IdDataCellsSelector = "td[data-label=\"Student ID\"]";
        public const string NameDataCellsSelector = "td[data-label=\"Name\"]";
        public const string GPADataCellsSelector = "td[data-label=\"GPA\"]";

        public const string StudentSearchNameSelector = ".filter-input-student-name.filter-header-cell input";
        //public const string StudentSearchNameSelectorClear = ".filter-input-student-name.filter-header-cell input";

        //These filters are selected via class because MudBlazor doesn't seem to have a way to set id's for the default filter implementation
        public const string StudentSearchSIdSelector = ".filter-input-sid.filter-header-cell input";
        //public const string StudentSearchSIdSelectorClear = ".filter-input-student-name.filter-header-cell input";

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

            /*JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);
            */

            /*
            JSInterop.SetupVoid("mudElementRef.getBoundingClientRect", _ => true);
            JSInterop.Setup<BoundingClientRect>("mudElementRef.getBoundingClientRect", _ => true);
            JSInterop.Setup<int>("mudpopoverHelper.countProviders");
            JSInterop.SetupVoid("watchDarkThemeMedia", _ => true);
            */


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

            //
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

            studentsPage.Instance.ParsedNameFilter.Should().Be(searchString);
        }

        //Search feature input validation
        [TestCase("10", 10)]
        [TestCase("1024", 1024)]
        [TestCase("", null)]
        [TestCase(" ", null)]
        public void ViewStudents_SearchFeature_ShouldFilterBySId(string searchString, int? parsedInt)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(StudentSearchSIdSelector);
            searchField.Input(searchString);
            //searchField.Change(searchString);

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            studentsPage.WaitForAssertion(() => {
                VerifyDefaultGetStudentsRestResponseCalled(Times.Exactly(2));
            });

            ShouldSeeExpectedStudentsInGrid(studentsPage);

            studentsPage.FindAll(ErrorInputsSelector).Should().BeEmpty();

            studentsPage.Instance.ParsedSIdFilter.Should().Be(parsedInt);
        }

        // Impossible input test cases, auto corrected, no error
        //[TestCase("", null)]
        //[TestCase(" ", null)]
        //[TestCase("10 1")]
        //[TestCase("invalid")]
        //[TestCase("101f")]
        //[TestCase("-1")] //TODO negative fail case

        // Error message is shown
        [TestCase("-1")] //TODO negative fail case
        public void ViewStudents_SearchFeature_ShouldHaveErrors_WhenFilteringBySId(string invalidSearchString)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(StudentSearchSIdSelector);
            searchField.Input(invalidSearchString);

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            studentsPage.WaitForAssertion(() => {
                VerifyDefaultGetStudentsRestResponseCalled();
            });

            //studentsPage.WaitForState(() => studentsPage.Instance.DataGrid.FilterDefinitions.Any(x => x.Column.FilterContext.));

            ShouldSeeExpectedStudentsInGrid(studentsPage);


            //TODO
            //studentsPage.Instance.SIdError.Should().BeTrue();

            var errorMessages = studentsPage.FindAll(ErrorInputsSelector);
            errorMessages.Should().ContainSingle();
            errorMessages.Single().Text().Should().Be(Text.StudentsPage.SIdFilterErrorText);

            studentsPage.Instance.ParsedSIdFilter.Should().BeNull();
            // TODO
            //studentsPage.Instance.ParsedNameFilter.Should().Be(invalidSearchString);
        }



        private void VerifyDefaultGetStudentsRestResponseCalled(Times? times = null)
        {
            if (times is null) times = Times.Once();

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(It.IsAny<int?>(), null, null, null, null, 1, 15), times.Value);
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
