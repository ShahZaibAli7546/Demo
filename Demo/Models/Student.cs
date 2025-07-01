using Microsoft.AspNetCore.Mvc.Rendering;

namespace Demo.Models
{
    public class Student
    {
        // Academic Info
        public int AcademicYearId { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string? AdmissionNo { get; set; }
        public int CourseId { get; set; }
        public int StudentCategoryId { get; set; }

        // Personal Info
        public string? StudentName { get; set; }
        public string? PersonalEmail { get; set; }
        public string? Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? Religion { get; set; }
        public string? StudentMobile { get; set; }
        public string? BirthPlace { get; set; }
        public string? MotherTongue { get; set; }

        // Family Info
        public string? CNIC { get; set; }
        public string? FatherName { get; set; }
        public string? RelationToStudent { get; set; } = "Father";
        public string? FatherOccupation { get; set; }
        public string? FatherMobile { get; set; }
        public string? FatherEmail { get; set; }
        public string? FatherAddressBuilding { get; set; }
        public string? FatherStreetAddress { get; set; }
        public string? FatherCity { get; set; }
        public string? StudentMaritalStatus { get; set; }

        // Fee Info
        public int TuitionFeeId { get; set; }
        public int? DiscountId { get; set; }
        public decimal AfterDiscountAmount { get; set; }
        public decimal? TransportFee { get; set; }
        public decimal? HostelFee { get; set; }
        public decimal TotalFee { get; set; }

        // Dropdowns
        public List<SelectListItem> AcademicYears { get; set; } = new();
        public List<SelectListItem> Courses { get; set; } = new();
        public List<SelectListItem> StudentCategories { get; set; } = new();
        public List<SelectListItem> Nationalities { get; set; } = new();
        public List<SelectListItem> Genders { get; set; } = new();
        public List<SelectListItem> FeeServices { get; set; } = new();
        public List<SelectListItem> FeeDiscounts { get; set; } = new();
    }
}
