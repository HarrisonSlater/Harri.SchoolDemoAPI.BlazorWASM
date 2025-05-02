using Microsoft.Playwright;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels.Actions
{
    //Wraps actions for the MudBlazor drop down when filtering in a data grid
    public class FilterOperatorActions
    {
        private readonly IPage _page;
        private readonly ILocator _dropDownItems;

        public FilterOperatorActions(IPage page)
        {
            _page = page;
            _dropDownItems = _page.Locator(".mud-popover-open .mud-list .mud-menu-item");
        }

        //TODO click based on actual operator text not Nth
        public async Task ClickEquals()
        {
            await _dropDownItems.Nth(0).ClickAsync();
        }

        public async Task ClickGreaterThanAsync()
        {
            await _dropDownItems.Nth(1).ClickAsync();
        }

        public async Task ClickLessThanAsync()
        {
            await _dropDownItems.Nth(2).ClickAsync();
        }

        public async Task ClickIsEmpty()
        {
            await _dropDownItems.Nth(3).ClickAsync();
        }

        public async Task ClickIsNotEmpty()
        {
            await _dropDownItems.Nth(4).ClickAsync();
        }
    }
}
