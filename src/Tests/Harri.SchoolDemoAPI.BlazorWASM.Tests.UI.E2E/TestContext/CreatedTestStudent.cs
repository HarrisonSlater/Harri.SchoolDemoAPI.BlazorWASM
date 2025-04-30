namespace Harri.SchoolDemoAPI.BlazorWASM.Tests.UI.E2E.TestContext
{
    // This is the object for storing test state about a created student
    public class CreatedTestStudent
    {
        private string? studentId;
        private string? studentName;
        private string? studentGPA;

        public string StudentId { 
            get 
            {
                if (studentId is null) throw new ArgumentException("CreatedTestStudent.StudentId has not been set by a previous step");
                return studentId;
            } 
            set => studentId = value;
        }

        public string StudentName { 
            get 
            {
                if (studentName is null) throw new ArgumentException("CreatedTestStudent.StudentName has not been set by a previous step");
                return studentName;
            } 
            set => studentName = value;
        }

        public string? StudentGPA { 
            get 
            {
                return studentGPA;
            } 
            set => studentGPA = value;
        }
    }
}
