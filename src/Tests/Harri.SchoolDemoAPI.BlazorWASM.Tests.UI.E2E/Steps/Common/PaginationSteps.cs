using FluentAssertions;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps.Common
{
    [Binding]
    public class PaginationSteps
    {
        private readonly IPage _page;
        private readonly PaginationActions _paginationActions;

        public PaginationSteps(IPage page)
        {
            _page = page;
            _paginationActions = new PaginationActions(page);
        }

        [When("I click next page")]
        public async Task IClickNext()
        {
            await _paginationActions.NextPageButton.ClickAsync();
        }

        [When("I click previous page")]
        public async Task IClickPrevious()
        {
            await _paginationActions.PreviousPageButton.ClickAsync();

        }

        [When("I click first page")]
        public async Task IClickFirst()
        {
            await _paginationActions.GoToFirstPageButton.ClickAsync();

        }

        public int? LastPage { get; set; }
        [When("I click last page")]
        public async Task IClickLast()
        {
            await _paginationActions.GoToLastPageButton.ClickAsync();
            //await _studentsPage.Page.WaitForURLAsync(new Regex(".*"));

            LastPage = _paginationActions.GetPage();
        }

        [Then("(I )see page {int} in the url")]
        public async Task ThenISeePageInTheUrl(int pageNumber)
        {
            await _page.WaitForURLAsync(new Regex(".*"));
            await Assertions.Expect(_page).ToHaveURLAsync(new Regex($".*/page/{pageNumber}"));
        }

        [Then("I see the last page in the url")]
        public async Task ThenISeePageTheLastPageInTheUrl()
        {
            if (LastPage is null) throw new ArgumentException("_lastPage has not been set by a previous step");

            await Assertions.Expect(_page).ToHaveURLAsync(new Regex($".*/page/{LastPage}"));
        }
    }
}
