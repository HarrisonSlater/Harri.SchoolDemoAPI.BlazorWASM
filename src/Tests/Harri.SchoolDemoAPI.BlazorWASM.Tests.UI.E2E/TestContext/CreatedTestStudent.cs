namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.TestContext
{
    public class CreatedTestStudent
    {
        private string? studentId;

        public string StudentId { 
            get 
            {
                if (studentId is null) throw new ArgumentException("CreatedTestStudent.StudentId has not been set by a previous step");
                return studentId;
            } 
            set => studentId = value;
        }
    }
}
