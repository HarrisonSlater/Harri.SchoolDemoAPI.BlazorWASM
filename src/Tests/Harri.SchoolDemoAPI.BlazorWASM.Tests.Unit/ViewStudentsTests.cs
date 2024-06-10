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


namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.Unit
{
    /// <summary>
    /// These tests are written entirely in C#.
    /// Learn more at https://bunit.dev/docs/getting-started/writing-tests.html#creating-basic-tests-in-cs-files
    /// </summary>
    [TestFixture]
    public class ViewStudentsTests : BunitTestContext
    {
        //private const string NameInputSelector = "#student-name";
        //private const string GpaInputSelector = "#student-gpa";
        //private const string SubmitButtonSelector = "#submit-button";

        //private const string ErrorAlertSelector = "#student-error-alert";
        private const string SuccessAlertSelector = "#student-success-alert";
        public const string IdDataCellsSelector = "td[data-label=\"SId\"]";
        public const string NameDataCellsSelector = "td[data-label=\"Name\"]";
        public const string GPADataCellsSelector = "td[data-label=\"GPA\"]";


        private Mock<IStudentApiClient> _mockStudentApiClient;

        private List<string?> _expectedSIds = [];
        private List<string?> _expectedNames = [];
        private List<string?> _expectedGpas = [];

        [SetUp] 
        public void SetUp()
        {
            _mockStudentApiClient = new Mock<IStudentApiClient>();

            Services.AddSingleton(_mockStudentApiClient.Object);
            Services.AddMudServices();
            //Services.AddSingleton<MudPopoverProvider>();
            //Services.AddSingleton<MudDialogProvider>();
            //Services.AddSingleton<MudSnackbarProvider>();
            //Services.AddSingleton<MudThemingProvider>();
            //Services.AddSingleton<IKeyInterceptorFactory, KeyInterceptorFactory>();

            //Services.AddSingleton<MudRTLProvider>();


            JSInterop.SetupVoid("mudKeyInterceptor.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.connect", _ => true);
            JSInterop.SetupVoid("mudPopover.initialize", _ => true);

        }

        private List<StudentDto>? SetUpMockExistingStudents()
        {
            var mockExistingStudents = new List<StudentDto>() { 
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
                    Data = mockExistingStudents
                }));
            _expectedSIds = mockExistingStudents.Select(x => x.SId.ToString()).ToList();
            _expectedNames = mockExistingStudents.Select(x => x.Name).ToList();
            _expectedGpas = mockExistingStudents.Select(x => x.GPA.ToString()).ToList();

            return mockExistingStudents;
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
