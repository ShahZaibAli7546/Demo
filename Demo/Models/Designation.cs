using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Designation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Designation Name")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // options: Active, Inactive
    }
}
