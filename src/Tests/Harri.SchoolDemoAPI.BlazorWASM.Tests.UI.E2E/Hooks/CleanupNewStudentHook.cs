using BoDi;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.Steps.Common;
using Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.TestContext;
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
    public class CleanupNewStudentHook
    {
        private readonly IObjectContainer _objectContainer;
        private readonly SetupSteps _setupSteps;
        private readonly CreatedTestStudent _createdTestStudent;

        public CleanupNewStudentHook(IObjectContainer objectContainer, SetupSteps setupSteps, CreatedTestStudent createdTestStudent)
        {
            _objectContainer = objectContainer;
            _setupSteps = setupSteps;
            _createdTestStudent = createdTestStudent;
        }

        [Scope(Tag = "cleanupNewStudent")]
        [AfterScenario(Order = 1)]
        public async Task AfterScenario()
        {
            await _setupSteps.DeleteAStudent(_createdTestStudent.StudentId);
        }
    }
}
