using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;

namespace WebAPI.Migrations
{
    public class DataSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<UsageDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            context.Database.EnsureCreated();

            int Id = 0;
            string data = "";
            Assembly assembly = Assembly.GetExecutingAssembly();

            if (!context.UserTypes.Any())
            {
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("UserTypes.json"));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        data = reader.ReadToEnd();
                        var jsData = JObject.Parse(data);
                        foreach (var r in jsData.Root["UserTypes"])
                        {
                            context.Add(new BS_UserType() { UserTypeId = (int)r["UserTypeId"], Name = r["Name"].ToString() });
                        }
                        //context.SaveChanges();

                    }
                }
            }

            if (!context.OffTypes.Any())
            {
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("OffTypes.json"));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        data = reader.ReadToEnd();
                        var jsData = JObject.Parse(data);
                        foreach (var r in jsData.Root["OffTypes"])
                        {
                            context.Add(new BS_OffType() { Id = (byte)r["Id"], Name = r["Name"].ToString() });
                        }
                        //context.SaveChanges();

                    }
                }
            }

            if (!context.Roles.Any())
            {
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Roles.json"));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        data = reader.ReadToEnd();
                        var jsData = JObject.Parse(data);

                        foreach (var r in jsData.Root["Roles"])
                        {
                            context.Add(new IdentityRole(r["RoleName"].ToString()));
                        }
                        //context.SaveChanges();

                    }
                }
            }

            if (!context.Positions.Any())
            {
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Positions.json"));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        data = reader.ReadToEnd();
                        var jsData = JObject.Parse(data);

                        Id = 0;
                        foreach (var r in jsData.Root["Positions"])
                        {
                            Id++;
                            context.Add(new BS_Position() { PositionId = Id, PositionName = r["PositionName"].ToString() });
                        }
                        //context.SaveChanges();

                    }
                }
            }

            if (!context.Levels.Any())
            {
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Levels.json"));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        data = reader.ReadToEnd();
                        var jsData = JObject.Parse(data);

                        Id = 0;
                        foreach (var r in jsData.Root["Levels"])
                        {
                            Id++;
                            context.Add(new BS_Level() { LevelId = Id, LevelName = r["LevelName"].ToString() });
                        }
                        //context.SaveChanges();

                    }
                }
            }

            if (!context.Frameworks.Any())
            {
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("Frameworks.json"));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                    {
                        data = reader.ReadToEnd();
                        var jsData = JObject.Parse(data);

                        Id = 0;
                        foreach (var r in jsData.Root["Frameworks"])
                        {
                            Id++;
                            context.Add(new BS_Framework() { FrameworkId = Id, FrameworkName = r["FrameworkName"].ToString() });
                        }
                        //context.SaveChanges();

                    }
                }
            }

            var user = new ApplicationUser
            {
                UserName = "SysAdmin",
                NormalizedUserName = "SysAdmin",
                Email = "noreply@BeetSoft.vn",
                NormalizedEmail = "noreply@BeetSoft.vn",
                DisplayName = "BeetSoft",
                First_Name = "Beetsoft",
                Last_Name = "Usage",
                Address = "Hà Nội",
                IsDeleted = false,
                Date_Created = DateTime.Now,
                Start_Date = DateTime.Now,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString("D")
            };

            if (!context.Users.Any(u => u.UserName == user.UserName))
            {
                var password = new PasswordHasher<ApplicationUser>();
                var hashed = password.HashPassword(user, "123456Aa@");
                user.PasswordHash = hashed;

                var userStore = new UserStore<ApplicationUser>(context);
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "Manager");
            }

            await context.SaveChangesAsync();
        }
    }
}
