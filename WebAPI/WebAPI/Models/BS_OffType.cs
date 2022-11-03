using System;
using System.Collections.Generic;

namespace WebAPI.Models
{
    public class BS_OffType: BaseModels
    {
        public Byte Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<BS_ReportOff> ReportOffs { get; set; }
    }
}
