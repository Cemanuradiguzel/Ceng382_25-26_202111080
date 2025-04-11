using System.ComponentModel.DataAnnotations;

namespace MyRazorApp.Models
{
    public static class ClassData
    {
        // This static list will hold all class information
        public static List<ClassInformationModel> Classes { get; set; } = new List<ClassInformationModel>();

        // You can also add methods to add, update, or delete classes if needed
    }
    
    public class ClassInformationModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Class Name is required.")] 
        public string? ClassName { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Student Count must be a positive number.")]
        public int StudentCount { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string? Description { get; set; }

        // Static variable to simulate auto-increment behavior
        private static int _nextId = 1;

        // Constructor that sets the ID to the next available number
        public ClassInformationModel()
        {
            Id = _nextId++;
        }
    }
    public class ClassInformationTable
    {
        public string? ClassName { get; set; }
        public int StudentCount { get; set; }
        public string? Description { get; set; }
    }
}
