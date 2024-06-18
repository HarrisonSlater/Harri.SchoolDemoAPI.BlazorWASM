using BoDi;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Hooks
{
    [Binding]
    public class ConfigHook
    {
        private readonly IObjectContainer _objectContainer;

        private static SchoolDemoBaseUrlSetting? _baseUrlSetting;
        public ConfigHook(IObjectContainer objectContainer)
        {
            _objectContainer = objectContainer;
        }

        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            var baseUrl = config["SchoolDemoBaseUrl"];
            _baseUrlSetting = new SchoolDemoBaseUrlSetting(baseUrl);

        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            _objectContainer.RegisterInstanceAs(_baseUrlSetting);
        }
    }

    public class SchoolDemoBaseUrlSetting
    {
        public SchoolDemoBaseUrlSetting(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
