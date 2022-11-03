using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class BaseModels
    {
        public bool IsDeleted { get; set; }
        public DateTime Date_Created { get; set; }
        public DateTime Last_Updated { get; set; }
        [StringLength(128)]
        public string Created_By { get; set; }
        [StringLength(128)]
        public string Updated_By { get; set; }
    }
}
