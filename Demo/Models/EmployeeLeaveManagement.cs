using System;
using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    // ----------------------------
    // Leave Allotment Model
    // ----------------------------
    public class LeaveAllotment
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Campus")]
        public int CampusId { get; set; }

        [Required]
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }

        [Required]
        [Display(Name = "Leave Type")]
        public int LeaveTypeId { get; set; }

        [Required]
        [Display(Name = "Total Days Allotted")]
        public int TotalDays { get; set; }

        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }
    }

    // ----------------------------
    // Leave Year Model
    // ----------------------------
    public class LeaveYear
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Leave Year Title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Active"; // or "Inactive"

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
    }
}
