using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsageHelper;

namespace WebAPI.Models.ViewModels
{
    public class RecruitmentViewModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int PositionId { get; set; }
        public int FrameworkId { get; set; }
        public int LanguageId { get; set; }
        public double SalaryMin { get; set; }
        public double SalaryMax { get; set; }
        public int LevelId { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string PositionName { get; set; }
        public string LanguageName { get; set; }
        public string FrameworkName { get; set; }
        public DateTime DatePublish { get; set; }
        public DateTime DateOnBroad { get; set; }
        public int Priority { get; set; }
        public int Status { get; set; }
        public int Result { get; set; }

    }

    public class RecruitmentQueryParameters : QueryParametersBS
    {
        public int? Status { get; set; }
        public int? Result { get; set; }
        public int? Position { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
