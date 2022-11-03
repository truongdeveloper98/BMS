using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Utils
{
    public static class ErrorLoggingExtensions
    {
        public static IApplicationBuilder UseErrorLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorLogging>();
        }
    }

    public class ErrorLogging
    {
        private readonly RequestDelegate _next;
        private static UsageDbContext _contextDB;

        public ErrorLogging(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {e.Message}");
                //Ghi lỗi exception vào db
                _contextDB.Add<BS_Log>(new BS_Log()
                {
                    Date = DateTime.Now,
                    UserName = context.User.Identity.Name,
                    Message = e.Message,
                    Exception = e.StackTrace
                });
                await _contextDB.SaveChangesAsync();

                throw;
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            _contextDB = serviceProvider.GetRequiredService<UsageDbContext>();
        }
    }

}
