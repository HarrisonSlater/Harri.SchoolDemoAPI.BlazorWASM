using BoDi;
using Microsoft.Playwright;
using SpecFlow.Actions.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks
{
    [Binding]
    internal class BrowserDriverHook
    {
        private readonly IObjectContainer _objectContainer;
        private IPage? _page;
        private IBrowserContext? _browserContext;

        private BrowserDriver? _browserDriver;

        public BrowserDriverHook(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeFeature]
        public static void BeforeFeature()
        {

        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            var browserDriver = _objectContainer.Resolve<BrowserDriver>();
            _browserDriver = browserDriver;
            var browser = await browserDriver.Current;
            _browserContext = await browser.NewContextAsync();

            var page = await _browserContext.NewPageAsync();
            if (page is null)
            {
                throw new Exception("Page should not be null");
            }

            _page = page;

            _objectContainer.RegisterInstanceAs(page);
            
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            await _browserContext!.CloseAsync();

            var browser = await _browserDriver!.Current;
            await browser.CloseAsync();
        }
    }
}
