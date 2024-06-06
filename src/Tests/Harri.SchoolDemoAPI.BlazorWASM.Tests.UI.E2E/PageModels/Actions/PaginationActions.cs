using Microsoft.Playwright;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions
{
    public class PaginationActions
    {
        private readonly IPage _page;

        public PaginationActions(IPage page)
        {
            _page = page;
        }

        public ILocator PaginationActionsLocator => _page.Locator(".mud-table-pagination-actions");

        public ILocator GoToFirstPageButton => PaginationActionsLocator.Locator("button").Nth(0);
        public ILocator PreviousPageButton => PaginationActionsLocator.Locator("button").Nth(1);
        public ILocator NextPageButton => PaginationActionsLocator.Locator("button").Nth(2);
        public ILocator GoToLastPageButton => PaginationActionsLocator.Locator("button").Nth(3);
    }
}
