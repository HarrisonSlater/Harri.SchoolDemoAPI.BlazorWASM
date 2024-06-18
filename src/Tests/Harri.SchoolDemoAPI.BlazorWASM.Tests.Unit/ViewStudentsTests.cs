using AngleSharp.Dom;
using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Pages;
using Harri.SchoolDemoApi.Client;
using Harri.SchoolDemoAPI.Models.Dto;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using MudBlazor;
using MudBlazor.Services;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Text;
using NUnit.Framework.Constraints;
using System;


namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit
{
    [TestFixture]
    public class ViewStudentsTests : BunitTestContext
    {
        private const string SuccessAlertSelector = "#student-success-alert";
        public const string IdDataCellsSelector = "td[data-label=\"SId\"]";
        public const string NameDataCellsSelector = "td[data-label=\"Name\"]";
        public const string GPADataCellsSelector = "td[data-label=\"GPA\"]";
        public const string SearchFieldSelector = "#student-search";

        private const string ErrorAlertSelector = "#student-error-alert";


        private Mock<IStudentApiClient> _mockStudentApiClient;
        private List<StudentDto> _mockExistingStudents;

        private List<string?> _expectedSIds = [];
        private List<string?> _expectedNames = [];
        private List<string?> _expectedGpas = [];

        [SetUp] 
        public void SetUp()
        {
            _mockStudentApiClient = new Mock<IStudentApiClient>();

            Services.AddSingleton(_mockStudentApiClient.Object);
            Services.AddMudServices();

            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);
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

            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse())
                .Returns(Task.FromResult(new RestSharp.RestResponse<List<StudentDto>>(new RestSharp.RestRequest()) {
                    Data = _mockExistingStudents
                }));
            _expectedSIds = _mockExistingStudents.Select(x => x.SId.ToString()).ToList();
            _expectedNames = _mockExistingStudents.Select(x => x.Name).ToList();
            _expectedGpas = _mockExistingStudents.Select(x => x.GPA.ToString()).ToList();

            return _mockExistingStudents;
        }

        // View students
        [Test]
        public void ViewStudents_RendersSuccessfully()
        {
            // Arrange
            SetUpMockExistingStudents();

            // Act
            var studentsPage = RenderComponent<Students>();

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(), Times.Once);
        }

        [Test]
        public void ViewStudents_ShowsErrorOnFail()
        {
            // Arrange
            _mockStudentApiClient.Setup(client => client.GetStudentsRestResponse())
                .Returns(Task.FromResult(new RestSharp.RestResponse<List<StudentDto>>(new RestSharp.RestRequest())
                {
                    Data = null
                }));

            // Act
            var studentsPage = RenderComponent<Students>();

            // Assert
            studentsPage.WaitForElement(ErrorAlertSelector);

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(), Times.Once);
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

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(), Times.Once);
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

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(), Times.Once);
        }

        [TestCase("Test Existing Student")]
        [TestCase("Student")]
        [TestCase("  ")]
        [TestCase("")]
        public void ViewStudents_SearchFeatureShouldMatchAllStudents(string searchString)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(SearchFieldSelector);
            searchField.Input(searchString);

            // Assert
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(), Times.Once);
        }

        [TestCase("1", 1)]
        [TestCase("1.11", 1)]
        [TestCase("Test Existing Student 1", 1)]
        [TestCase("2", 2)]
        [TestCase("2.22", 2)]
        [TestCase("Test Existing Student 2", 2)]
        [TestCase("3", 3)]
        [TestCase("3.33", 3)]
        [TestCase("Test Existing Student 3", 3)]
        public void ViewStudents_SearchFeatureShouldFilterOneStudentCorrectly(string searchString, int expectedStudentId)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(SearchFieldSelector);
            searchField.Input(searchString);

            // Assert
            ShouldSeeOnlyOneStudentInGrid(studentsPage, expectedStudentId);

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(), Times.Once);
        }

        [TestCase("Test Existing Student 33")]
        [TestCase("asdfasdf")]
        public void ViewStudents_SearchFeatureShouldNotMatchAnyStudents(string searchString)
        {
            // Arrange
            SetUpMockExistingStudents();

            var studentsPage = RenderComponent<Students>();
            ShouldSeeExpectedStudentsInGrid(studentsPage);

            // Act
            var searchField = studentsPage.Find(SearchFieldSelector);
            searchField.Input(searchString);

            // Assert
            ShouldSeeNoStudentsInGrid(studentsPage);

            _mockStudentApiClient.Verify(x => x.GetStudentsRestResponse(), Times.Once);
        }

        private void ShouldSeeNoStudentsInGrid(IRenderedComponent<Students> studentsPage)
        {
            var sids = studentsPage.FindAll(IdDataCellsSelector).ToList();
            var names = studentsPage.FindAll(NameDataCellsSelector).ToList();
            var gpas = studentsPage.FindAll(GPADataCellsSelector).ToList();

            sids.Should().HaveCount(0);
            names.Should().HaveCount(0);
            gpas.Should().HaveCount(0);
        }
        private void ShouldSeeOnlyOneStudentInGrid(IRenderedComponent<Students> studentsPage, int studentId)
        {
            var expectedStudent = _mockExistingStudents.Find(s => s.SId == studentId);
            if (expectedStudent == null) throw new ArgumentException($"Invalid test studentId: {studentId}");

            var sids = studentsPage.FindAll(IdDataCellsSelector).ToList();
            var names = studentsPage.FindAll(NameDataCellsSelector).ToList();
            var gpas = studentsPage.FindAll(GPADataCellsSelector).ToList();

            sids.Should().HaveCount(1);
            names.Should().HaveCount(1);
            gpas.Should().HaveCount(1);

            var actualSIds = sids.Select(x => x.GetInnerText()).ToList();
            var actualNames = names.Select(x => x.GetInnerText()).ToList();
            var actualGpas = gpas.Select(x => x.GetInnerText()).ToList();

            actualSIds.Should().BeEquivalentTo([expectedStudent.SId.ToString()]);
            actualNames.Should().BeEquivalentTo([expectedStudent.Name]);
            actualGpas.Should().BeEquivalentTo([expectedStudent.GPA.ToString()]);
        }

        private void ShouldSeeExpectedStudentsInGrid(IRenderedComponent<Students> studentsPage)
        {
            var sids = studentsPage.FindAll(IdDataCellsSelector).ToList();
            var names = studentsPage.FindAll(NameDataCellsSelector).ToList();
            var gpas = studentsPage.FindAll(GPADataCellsSelector).ToList();

            sids.Should().HaveCount(3);
            names.Should().HaveCount(3);
            gpas.Should().HaveCount(3);

            var actualSIds = sids.Select(x => x.GetInnerText()).ToList();
            var actualNames = names.Select(x => x.GetInnerText()).ToList();
            var actualGpas = gpas.Select(x => x.GetInnerText()).ToList();

            actualSIds.Should().BeEquivalentTo(_expectedSIds);
            actualNames.Should().BeEquivalentTo(_expectedNames);
            actualGpas.Should().BeEquivalentTo(_expectedGpas);
        }
    }
}
