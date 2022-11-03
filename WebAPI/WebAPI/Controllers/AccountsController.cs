//Controller chưa các function liên quan tới thiết lập tài khoản

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UsageHelper;
using WebAPI.JwtFeatures;
using WebAPI.Models;
using WebAPI.Models.ViewModels;
using WebAPI.Utils;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBaseBS
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly JwtHandler _jwtHandler;
        public IConfiguration Configuration { get; set; }

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration,
            JwtHandler jwtHandler,
            UsageDbContext context) : base(context)
        {
            _userManager = userManager;
            _configuration = configuration;
            _jwtHandler = jwtHandler;
            Configuration = configuration;
        }

        //Dăng ký account mới
        [HttpPost]
        [Route("Register")]
        [Authorize(Roles = BSRole.MANAGER + "," + BSRole.SYSADMIN)]
        public async Task<ActionResult> PostApplicationUser([FromBody] ResgisterViewModel model)
        {
            var userid = User.Claims.Where(c => c.Type.Equals("UserID")).FirstOrDefault().Value;

            //khởi tạo thông tin accnount
            var user = new ApplicationUser()
            {
                UserName = model.Username,
                Email = model.Email,
                DisplayName = model.First_Name + " " + model.Last_Name,
                First_Name = model.First_Name,
                Last_Name = model.Last_Name,
                Birth_Date = Usage.Date2Utc(model.Birth_Date),
                Start_Date = Usage.Date2Utc(model.Start_Date),
                End_Date = Usage.Date2Utc(model.End_Date),
                Address = model.Address,
                Avatar = model.Avatar,
                PhoneNumber = model.Phone,
                IsDeleted = model.IsDeleted,
                Date_Created = DateTime.Now,
                Last_Updated = DateTime.Now,
                Created_By = userid,
                Updated_By = userid,
                LockoutEnabled = false,
            };

            //thêm account và role vào DB
            var result = await _userManager.CreateAsync(user, model.Password);

            model.Role = model.Role == Const.ROLE_MEMBER ? Const.ROLE_DEV : model.Role == Const.ROLE_OTHER ? Const.ROLE_HR : model.Role;

            await _userManager.AddToRoleAsync(user, model.Role);

            user = await _userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                BS_UserInfo info = new BS_UserInfo
                {
                    Position = model.Info.Position,
                    UserId = user.Id,
                    ThisYearLeaveDay = 0,
                    LastYearLeaveDay = 0,
                    OccupiedLeaveDay = 0,
                    Department = model.Info.Department,
                    Level = model.Info.Level,
                    Team = model.Info.Team,
                    IsPending = model.Info.IsPending,
                    TypeId = model.Info.TypeId > 0 ? model.Info.TypeId : Const.USER_TYPE_INTERN,
                };

                if (model.Info.IsPending)
                {
                    info.PendingStart = Usage.Date2Utc(model.Info.PendingStart);
                    info.PendingEnd = Usage.Date2Utc(model.Info.PendingEnd);
                    info.EffortFree = model.Info.EffortFree;
                }

                if (info.TypeId == Const.USER_TYPE_OFFICIAL)
                {
                    //Check xem member vào cty đủ 5 năm chưa
                    bool over5Year = (DateTime.Now.Year - user.Start_Date.Value.Year > 5 ||
                             (DateTime.Now.Year - user.Start_Date.Value.Year == 5 &&
                              DateTime.Now.Month >= user.Start_Date.Value.Month)
                              );

                    //Update dữ liệu ngày nghỉ phép
                    double thisYearOff = over5Year ? 13 : 12;
                    if (model.Info.TotalLeaveDay >= thisYearOff)
                    {
                        info.ThisYearLeaveDay = thisYearOff;
                        info.LastYearLeaveDay = Math.Round(model.Info.TotalLeaveDay - thisYearOff, 2);
                    }
                    else info.ThisYearLeaveDay = model.Info.TotalLeaveDay;

                    info.OccupiedLeaveDay = model.Info.OccupiedLeaveDay;
                }
                //Các tác vụ khởi tạo cho account mới
                _db.Add(info);
                var checkProject = _db.Projects.Where(x => x.ProjectName == "Other").FirstOrDefault();
                if (checkProject == null)
                {
                    var project = new BS_Project()
                    {
                        ProjectName = "Other"
                    };
                    _db.Add(project);
                    await _db.SaveChangesAsync();
                };

                //Thêm vào project "Other" , mặc định mọi member được tham gia project này
                var OtherPjId = (from p in _db.Projects where p.ProjectName == "Other" select p.ProjectId).FirstOrDefault();
                if (info.Department < Const.MAX_DEP_MANUFACTORY) //Chỉ dành cho team sản xuất
                    _db.Add(new BS_UserProject
                    {
                        IsDeleted = false,
                        Date_Created = DateTime.Now,
                        Last_Updated = DateTime.Now,
                        ProjectId = OtherPjId,
                        PositionId = Const.POSITION_PROJECT_MEMBER, //Member = 2, PM = 1
                        UserId = user.Id
                    });

                await _db.SaveChangesAsync();
            }

            return Ok(result);
        }

        ////Đăng nhập bằng username và password
        ////Chức năng này tạm thời bị loại bỏ
        //[HttpPost]
        //[Route("Login")]
        //[Authorize(Roles = "SysAdmin")]
        //public async Task<IActionResult> Login(LoginViewModel model)
        //{
        //    var user = await _userManager.FindByNameAsync(model.Username);
        //    var checkPassword = await _userManager.CheckPasswordAsync(user, model.Password);

        //    if (user != null && checkPassword)
        //    {
        //        if (user.IsDeleted == true)
        //        {
        //            return BadRequest(new { message = "Account has been blocked" });
        //        }
        //        string token = await _jwtHandler.GenerateToken(user);

        //        //await UpdateDayOffData();
        //        return Ok(new { token });
        //    }
        //    else
        //    {
        //        //await UpdateDayOffData();
        //        return BadRequest(new { message = "Username and Password is incorrect" });
        //    }
        //}


        //Đăng nhập bằng tài khoản mail beetsoft
        [HttpPost("ExternalLogin")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuth)
        {
            //Xác minh token từ Google
            var payload = await _jwtHandler.VerifyGoogleToken(externalAuth);
            if (payload == null)
                return BadRequest(new { message = Message.EXTERNAL_AUTHEN });

            //Kiểm tra mail có phải từ Beetsoft
            Regex regex = new Regex(@"^([\w\.\-]+)@beetsoft\.com\.vn$");
            Match match = regex.Match(payload.Email);
            if (!match.Success)
                return BadRequest(new { message = Message.EMAIL_NOT_BEETSOFT });

            //Tìm kiếm phiên đăng nhập hiện có
            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);
            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                //Tìm account theo email
                user = await _userManager.FindByEmailAsync(payload.Email);

                //Nếu email không tồn tại => tạo account mới
                if (user == null)
                {
                    user = new ApplicationUser()
                    {
                        UserName = payload.Email.Substring(0, payload.Email.IndexOf('@')),
                        Email = payload.Email,
                        DisplayName = payload.FamilyName + " " + payload.GivenName,
                        First_Name = payload.FamilyName,
                        Last_Name = payload.GivenName,
                        IsDeleted = false,
                        Date_Created = DateTime.Now,
                        Last_Updated = DateTime.Now,
                        LockoutEnabled = false,
                    };


                    //Thêm account vào DB
                    var result = await _userManager.CreateAsync(user, "123456Aa@");
                    if (!result.Succeeded)
                        return BadRequest(new { message = Message.CREATE_FAIL });

                    await _userManager.AddToRoleAsync(user, Const.ROLE_PARTNER);
                    user = await _userManager.FindByEmailAsync(payload.Email);
                    if (user != null)
                    {
                        _db.Add(new BS_UserInfo
                        {
                            UserId = user.Id,
                            ThisYearLeaveDay = 0,
                            LastYearLeaveDay = 0,
                            OccupiedLeaveDay = 0,
                            Department = 0,
                            Team = "Other",
                            Level = "Fresher",
                            IsPending = false,
                            TypeId = Const.USER_TYPE_INTERN,
                            EffortFree = 0,
                        });
                        //Thêm vào project "Other" , mặc định mọi member được tham gia project này
                        var OtherPjId = (from p in _db.Projects where p.ProjectName == "Other" select p.ProjectId).FirstOrDefault();
                        _db.Add(new BS_UserProject
                        {
                            IsDeleted = false,
                            Date_Created = DateTime.Now,
                            Last_Updated = DateTime.Now,
                            ProjectId = OtherPjId,
                            PositionId = Const.POSITION_PROJECT_MEMBER, //Member = 2, PM = 1
                            UserId = user.Id
                        });

                    }

                }
                user.LockoutEnabled = false;

                await _db.SaveChangesAsync();

                //Đăng nhập 
                await _userManager.AddLoginAsync(user, info);
                var token = await _jwtHandler.GenerateToken(user);

                //Refresh dữ liệu ngày nghỉ của mọi member
                //await UpdateDayOffData();
                return Ok(new AuthResponseDto { Token = token, IsAuthSuccessful = true, IsNewUser = true });
            }

            //Nếu email tồn tại nhưng bị khoá
            else if (user.IsDeleted == true)
                return BadRequest(new { message = Message.ACCOUNT_BLOCKED });

            //Email tồn tại và account không bị khoá => cho đăng nhập
            var token2 = await _jwtHandler.GenerateToken(user);

            user.LockoutEnabled = false;

            await _db.SaveChangesAsync();

            //Refresh dữ liệu ngày nghỉ phép của mọi member
            //await UpdateDayOffData();
            return Ok(new AuthResponseDto { Token = token2, IsAuthSuccessful = true, IsNewUser = false });
        }

        [HttpGet("CheckLogout/{id}")]
        public async Task<bool> CheckLogout([FromRoute] string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
                return user.LockoutEnabled;

            return false;
        }

        //Refresh dữ liệu ngày nghỉ của mọi member
        private async Task<IActionResult> UpdateDayOffData() //run after every login event
        {
            var info = await _db.UserInfos.ToListAsync();

            var appSettingsPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "appsettings.json");
            var jsonString = System.IO.File.ReadAllText(appSettingsPath);
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.Converters.Add(new ExpandoObjectConverter());
            jsonSettings.Converters.Add(new StringEnumConverter());
            dynamic appSettingObject = JsonConvert.DeserializeObject<ExpandoObject>(jsonString, jsonSettings);
            appSettingObject.DebugEnabled = true;

            //Lấy thông tin ngày reset
            CultureInfo cultureInfo = CultureInfo.InvariantCulture;
            var newYearRef = DateTime.ParseExact(appSettingObject.DayOffClearDate.NewYearRef, Message.FORMAT_DATE, cultureInfo);
            var resetDateRef = DateTime.ParseExact(appSettingObject.DayOffClearDate.ResetDateRef, Message.FORMAT_DATE, cultureInfo);
            var today = DateTime.Now;

            //Thêm ngay phép cho năm mới
            if (newYearRef <= today)
            {
                foreach (BS_UserInfo v in info)
                {
                    if (v.TypeId == Const.USER_TYPE_OFFICIAL) //nv chính thức mới có phép
                    {
                        v.LastYearLeaveDay = v.ThisYearLeaveDay;
                        var user = _db.Users.Where(u => u.Id == v.UserId)
                                            .FirstOrDefault();
                        if (today.Year - user.Start_Date.Value.Year > 5 ||
                            (today.Year - user.Start_Date.Value.Year == 5 &&
                            today.Month >= user.Start_Date.Value.Month)
                            ) //đủ ngày đủ tháng 5 năm => 13 ngày phép năm
                            v.ThisYearLeaveDay = 13;
                        else
                            v.ThisYearLeaveDay = 12;
                    }
                    else
                    {
                        v.ThisYearLeaveDay = 0;
                        v.LastYearLeaveDay = 0;
                    }

                    v.OccupiedLeaveDay = 0;
                }
                appSettingObject.DayOffClearDate.NewYearRef = newYearRef.AddYears(1).ToString(Message.FORMAT_DATE);
            }

            //Huỷ phép năm cũ
            if (today >= resetDateRef)
            {
                foreach (BS_UserInfo v in info)
                {
                    v.LastYearLeaveDay = 0;
                }
                appSettingObject.DayOffClearDate.ResetDateRef = resetDateRef.AddYears(1).ToString(Message.FORMAT_DATE);
            }
            _db.UpdateRange(info);
            await _db.SaveChangesAsync();

            var newJson = JsonConvert.SerializeObject(appSettingObject, Formatting.Indented, jsonSettings);
            System.IO.File.WriteAllText(appSettingsPath, newJson);

            return Ok();
        }

    }
}
