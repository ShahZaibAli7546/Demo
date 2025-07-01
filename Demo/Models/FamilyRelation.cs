using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class FamilyRelation
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Relation")]
        public string RelationName { get; set; } = string.Empty;
        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // Default: Active
    }
}
