using System.ComponentModel.DataAnnotations;

namespace WebAPI.Models
{
    public class BS_Department : BaseModels
    {
        [Key]
        public int DepartmentId { get; set; }

        [Required]
        [StringLength(255)]
        public string DepartmentName { get; set; }
    }
}
