using System.ComponentModel.DataAnnotations;

namespace LabProject.Models
{
    public class ClassInformationModel
    {
        private static int _idCounter = 1;

        public ClassInformationModel()
        {
            Id = _idCounter++;
        }

        public int Id { get; }
        
        [Required]
        public string ClassName { get; set; } = string.Empty;

        [Range(1, 1000)]
        public int StudentCount { get; set; }

        public string? Description { get; set; }
    }
}
