using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UsageHelper;

namespace WebAPI.Models.ViewModels
{
    public class PositionViewModel
    {
        public int PositionId { get; set; }
        public string PositionName { get; set; }
    }

    public class LanguageViewModel
    {
        public int Id { get; set; }
        public string LanguageName { get; set; }
    }
    public class LevelViewModel
    {
        public int Id { get; set; }
        public string LevelName { get; set; }
    }

    public class FrameViewModel
    {
        public int Id { get; set; }
        public string FrameworkName { get; set; }
    }
    public class SalaryDTO
    {
        public int? Id { get; set; }

        [Required]
        public double HourlySalary { get; set; }

        [Required]
        public DateTime EffectiveDate { get; set; }

        public List<string> User_Id { get; set; }
    }

    public class SalaryViewModel
    {
        public int Id { get; set; }

        public string User_Id { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public double HourlySalary { get; set; }

        public DateTime EffectiveDate { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class SalaryQueryParameters : QueryParametersBS
    {
    }
}
