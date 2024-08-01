using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    public class EditStudentPage : BasePage
    {
        public ILocator StudentNameInput => _page.Locator("#student-name");
        public ILocator StudentNameLabel => _page.Locator("label[for=\"student-name\"]");
        public ILocator StudentGPAInput => _page.Locator("#student-gpa");
        public ILocator StudentGPALabel => _page.Locator("label[for=\"student-gpa\"]");
        public ILocator SaveButton => _page.Locator("#submit-button");

        public const string ErrorClass = "mud-input-error";
        public ILocator InputContainersWithError => _page.Locator(".mud-input-control.mud-input-error");
        public ILocator ErrorText => _page.Locator(".mud-input-helper-text.mud-input-error");

        public ILocator DeleteButton => _page.Locator("#delete-student-button");
        public ILocator DeleteDialogCancelButton => _page.Locator("#dialog-cancel");
        public ILocator DeleteDialogButton => _page.Locator("#dialog-delete");

        public ILocator DeleteDialog => _page.GetByRole(AriaRole.Dialog);


        public EditStudentPage(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting, PlaywrightConfiguration config) : base(page, baseUrlSetting, config)
        {
        }

        public async Task<string> GetStudentName()
        {
            return await StudentNameInput.InputValueAsync();
        }

        public async Task<string> GetStudentGPA()
        {
            return await StudentGPAInput.InputValueAsync();
        }

        public async Task EnterStudentName(string studentName)
        {
            await StudentNameInput.FillAsync(studentName);
        }

        public async Task EnterStudentGPA(string studentName)
        {
            await StudentGPAInput.FillAsync(studentName);
        }

        public async Task ClickSave()
        {
            await SaveButton.ClickAsync();
        }

        public async Task ClickDelete()
        {
            await DeleteButton.ClickAsync();
        }
        
        public async Task ClickDeleteInDialog()
        {
            await DeleteDialogButton.ClickAsync();
        }

        public async Task ClickCancelInDialog()
        {
            await DeleteDialogCancelButton.ClickAsync();
        }

        public async Task CreateNewStudent(string name, string? gpa = null)
        {
            await EnterStudentName(name);
            if (gpa is not null)
            {
                await EnterStudentGPA(gpa);
            }
            await ClickSave();
        }

        // Assertions
        public async Task ShouldHaveValidationErrorForName()
        {
            var errorContainerName = InputContainersWithError.Filter(new LocatorFilterOptions() { Has = StudentNameInput });

            await Assertions.Expect(StudentNameLabel).ToHaveClassAsync(new Regex(ErrorClass));
            await Assertions.Expect(errorContainerName.Locator(ErrorText)).ToBeVisibleAsync();
            await Assertions.Expect(errorContainerName.Locator(ErrorText)).ToContainTextAsync(new Regex("Name"));
        }

        public async Task ShouldHaveValidationErrorForGPA()
        {
            var errorContainerGPA = InputContainersWithError.Filter(new LocatorFilterOptions() { Has = StudentGPAInput });

            await Assertions.Expect(StudentGPALabel).ToHaveClassAsync(new Regex(ErrorClass));
            await Assertions.Expect(errorContainerGPA.Locator(ErrorText)).ToBeVisibleAsync();
            await Assertions.Expect(errorContainerGPA.Locator(ErrorText)).ToContainTextAsync(new Regex("GPA"));
        }

        public async Task AssertEditStudentPageIsVisible()
        {
            await Navigation.AssertPageLoaded();

            await Assertions.Expect(StudentNameInput).ToBeVisibleAsync();
            await Assertions.Expect(StudentNameLabel).Not.ToBeEmptyAsync();

            await Assertions.Expect(StudentGPAInput).ToBeVisibleAsync();
            await Assertions.Expect(StudentGPALabel).Not.ToBeEmptyAsync();
        }

        public async Task AssertFormEmpty()
        {
            await AssertNameEmpty();
            await AssertGPAEmpty();
        }

        public async Task AssertNameEmpty()
        {
            await Assertions.Expect(StudentNameInput).ToBeEmptyAsync();
        }

        public async Task AssertGPAEmpty()
        {
            await Assertions.Expect(StudentGPAInput).ToBeEmptyAsync();
        }

        public async Task AssertFormNotEmpty()
        {
            await AssertNameNotEmpty();
            await AssertGPANotEmpty();
        }

        public async Task AssertNameNotEmpty()
        {
            await Assertions.Expect(StudentNameInput).Not.ToBeEmptyAsync();
        }

        public async Task AssertGPANotEmpty()
        {
            await Assertions.Expect(StudentGPAInput).Not.ToBeEmptyAsync();
        }

        public async Task AssertSaveButtonDisabled()
        {
            await Assertions.Expect(SaveButton).ToBeDisabledAsync();
        }
    }
}
