using Microsoft.Playwright;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks;
using SpecFlow.Actions.Playwright;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.PageModels
{
    public abstract class BasePage
    {
        protected readonly IPage _page;
        public IPage Page => _page;
        public NavigationActions Navigation { get; set; }

        ///<remarks> See <see cref="BrowserDriverHook"/> for IPage injection</remarks>
        public BasePage(IPage page, SchoolDemoBaseUrlSetting baseUrlSetting, PlaywrightConfiguration config)
        {
            _page = page;
            
            Navigation = new NavigationActions(_page, baseUrlSetting, config);
        }
    }
}
