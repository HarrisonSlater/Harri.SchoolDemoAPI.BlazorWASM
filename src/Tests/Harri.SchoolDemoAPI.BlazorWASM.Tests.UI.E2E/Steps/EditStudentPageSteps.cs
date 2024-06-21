using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.TestContext;
using Microsoft.Playwright;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps
{
    [Binding]
    public class EditStudentPageSteps
    {
        private readonly EditStudentPage _editStudentPage;
        private readonly CreatedTestStudent _createdTestStudent;

        public EditStudentPageSteps(EditStudentPage editStudentPage, CreatedTestStudent createdTestStudent)
        {
            _editStudentPage = editStudentPage;
            _createdTestStudent = createdTestStudent;
        }

        [Then("I see the create new student form")]
        public async Task ISeeTheCreateNewStudentForm()
        {
            await _editStudentPage.AssertEditStudentPageIsVisible();

            await _editStudentPage.AssertFormEmpty();
        }

        [Then("I see the edit student form")]
        public async Task ISeeTheEditStudentForm()
        {
            await _editStudentPage.AssertEditStudentPageIsVisible();

            await _editStudentPage.AssertNameNotEmpty();
        }

        [Then("I should be on the create new student page")]
        public async Task IShouldBeOnTheCreateNewStudentPage()
        {
            await ISeeTheCreateNewStudentForm();
            await _editStudentPage.Navigation.AssertCreateNewStudentPageUrlIsCorrect();
        }

        [Then("I should be on the edit student page")]
        public async Task IShouldBeOnTheCreateEditSudentPage()
        {
            await ISeeTheEditStudentForm();
            await _editStudentPage.Navigation.AssertEditStudentPageUrlIsCorrect();
        }

        [Then("I should be on the edit student page for an existing student with gpa")]
        public async Task IShouldBeOnTheCreateEditSudentPageForAnExistingStudent()
        {
            await ISeeTheEditStudentForm();
            await _editStudentPage.Navigation.AssertEditStudentPageUrlIsCorrect(_createdTestStudent.StudentId);

            await _editStudentPage.AssertGPANotEmpty();
        }

        [When("(I )enter a student name {string}")]
        public async Task IEnterAStudentName(string studentName)
        {
            await _editStudentPage.EnterStudentName(studentName);
        }

        [When("(I )enter a student GPA {string}")]
        public async Task IEnterStudentGPA(string gpa)
        {
            await _editStudentPage.EnterStudentGPA(gpa);
        }

        [When("(I )click save")]
        public async Task IClickSave()
        {
            await _editStudentPage.ClickSave();
        }

        [When("I click delete")]
        public async Task WhenIClickDelete()
        {
            await _editStudentPage.ClickDelete();
        }

        [When("I click delete on the delete dialog")]
        public async Task WhenIClickDeleteOnTheDeleteDialog()
        {
            await _editStudentPage.ClickDeleteInDialog();
        }

        [When("I click cancel on the delete dialog")]
        public async Task WhenIClickCancelOnTheDeleteDialog()
        {
            await _editStudentPage.ClickCancelInDialog();
        }

        [Then("I should see the delete student dialog")]
        public async Task ThenIShouldSeeTheDeleteStudentDialog()
        {
            await Assertions.Expect(_editStudentPage.DeleteDialog).ToBeVisibleAsync();
        }

        [Then("I should not see the delete student dialog")]
        public async Task ThenIShouldNotSeeTheDeleteStudentDialog()
        {
            await Assertions.Expect(_editStudentPage.DeleteDialog).Not.ToBeVisibleAsync();
        }

        [When("(I )create a new student")]
        public async Task WhenICreateANewStudent()
        {
            await _editStudentPage.CreateNewStudent("Tester StudentE2E-2");
        }

        [When("I remove the student's name")]
        [When("I enter a blank student name")]
        public async Task WhenIRemoveTheStudentsName()
        {
            await _editStudentPage.EnterStudentName("");
        }

        [Then("I should see a validation message for the Name")]
        public async Task ThenIShouldSeeAValidationMessageForTheName()
        {
            await _editStudentPage.ShouldHaveValidationErrorForName();
        }

        [Then("I should see a validation message for the GPA")]
        public async Task ThenIShouldSeeAValidationMessageForTheGPA()
        {
            await _editStudentPage.ShouldHaveValidationErrorForGPA();
        }

        [Then("I should see a validation message for the Name and GPA")]
        public async Task ThenIShouldSeeAValidationMessageForTheNameAndGPA()
        {
            await _editStudentPage.ShouldHaveValidationErrorForName();
            await _editStudentPage.ShouldHaveValidationErrorForGPA();
        }
    }
}
