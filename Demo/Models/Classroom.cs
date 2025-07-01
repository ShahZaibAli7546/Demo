using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Classroom
    {
        public int ClassId { get; set; }

        [Required(ErrorMessage = "Class Name is required")]
        [Display(Name = "Class Name")]
        public string ClassName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select the statuse")]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Default: Active
        [Display(Name = "Remarks (QR Code Link)")]
        public string? QRCodeLink { get; set; }


    }
}
