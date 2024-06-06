using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public EditStudentPage(IPage page) : base(page)
        {
        }

        public async Task<string> GetStudentName()
        {
            return await StudentNameInput.InputValueAsync();
        }

        public async Task EnterStudentName(string studentName)
        {
            await StudentNameInput.FillAsync(studentName);
        }

        public async Task ClickSave()
        {
            await SaveButton.ClickAsync();
        }

        public async Task AssertCurrentPage()
        {
            //assert name and gpa input
            await Assertions.Expect(StudentNameInput).ToBeVisibleAsync();
            await Assertions.Expect(StudentNameLabel).Not.ToBeEmptyAsync();
            //tobe visble also

            await Assertions.Expect(StudentGPAInput).ToBeVisibleAsync();
            await Assertions.Expect(StudentGPALabel).Not.ToBeEmptyAsync();
        }
    }
}
