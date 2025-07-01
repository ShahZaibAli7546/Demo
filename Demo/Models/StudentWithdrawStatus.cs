using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class StudentWithdrawStatus
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Withdraw Status")]
        public string StatusName { get; set; } = string.Empty;

        [Display(Name = "Description")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }
    }
}
