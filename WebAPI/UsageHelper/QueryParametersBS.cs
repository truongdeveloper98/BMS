using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsageHelper
{
    public class QueryParametersBS
    {
        public int Page { get; set; }
        public int RecordsPerPage { get; set; }
        public PaginationVm PaginationVm
        {
            get { return new PaginationVm() { Page = Page, RecordsPerPage = RecordsPerPage }; }
        }
        public string Search { get; set; }
    }
}
