using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace UsageHelper
{
    public class PaginationVm
    {
        private int recordsPerPage = 10;

        public int Page { get; set; } = 1;

        public int RecordsPerPage
        {
            get { return recordsPerPage; }
            set { recordsPerPage = (value > Const.MAX_ITEM_PER_PAGE) ? Const.MAX_ITEM_PER_PAGE : value; }
        }
    }

    public static class Pagination
    {
        public static IQueryable<T> Paginage<T>(this IQueryable<T> queryable, PaginationVm pagination)
        {
            return queryable.Skip((pagination.Page - 1) * pagination.RecordsPerPage)
                            .Take((pagination.RecordsPerPage));
        }

        public static void InsertParametersPaginationInHeader<T>(this HttpContext httpContext, IQueryable<T> query)
        {
            if (httpContext == null) { throw new ArgumentException(nameof(httpContext)); }
            double count = query.Count();

            httpContext.Response.Headers.Add("totalAmountOfRecords", count.ToString());

        }
    }
}
