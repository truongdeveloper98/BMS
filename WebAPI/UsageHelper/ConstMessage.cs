using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsageHelper
{
    public class BSRole
    {
        public const string SYSADMIN = "SysAdmin";
        public const string MANAGER = "Manager";
        public const string SALE = "Sale";
        public const string HR = "HR";
        public const string MEMBER = "Member";

        public const string PRODUCT_MANAGER = "Product Manage";
    }

    public class Message
    {
        #region Common
        public const string APP_NAME = "Beetsoft Management System";

        public const string FORMAT_DATE = "dd/MM/yyyy";
        public const string FORMAT_DATETIME = "yyyyMMdd_HHmmss";
        public const string FILE_TYPE_EXCEL = ".xlsx";
        public const string CONTENT_TYPE_FILE_EXPORT = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

        public const string HEADER_EMAIL = "<h2>Đây là header email</h2> Đây là nội dung";
        //public const string FEMALE_NAME = "[ĐƠN XIN NGHỈ]";
        #endregion Common

        #region Authentication
        public const string NOT_FOUND = "Không tìm thấy";
        public const string EXTERNAL_AUTHEN = "Xác thực từ Google không hợp lệ.";
        public const string EMAIL_NOT_BEETSOFT = "Email của bạn không phải của Beetsoft.";
        public const string CREATE_FAIL = "Bạn không thể đăng ký tài khoản, vui lòng liên hệ admin để biết thêm thông tin.";
        public const string ACCOUNT_BLOCKED = "Tài khoản của bạn đã bị khóa, vui lòng liên hệ admin để biết thêm thông tin.";
        public const string PASS_SIZE_IMAGE = "Dung lượng ảnh đã vượt quá 800KB.";
        public const string INVALID_FORMAT_IMAGE = "Không đúng định dạng file ảnh.";
        #endregion Authentication

        #region Report
        public const string EXCEEDED_HOUR_REPORT = "Số giờ bạn báo cáo đã vượt quá 8h.";
        public const string REPORTED_LEAVE = "Bạn đã cáo báo ngày nghỉ rồi";
        public const string REPORTED_WORKING = "Bạn đã cáo báo cho ngày làm việc này rồi";
        public const string REPORTED_WORKING_AND_OFF = "Bạn đã cáo báo làm việc và nghỉ cho ngày này rồi";
        public const string REPORTED_DELETED = "Báo cáo này đã bị xóa.";
        public const string DONT_CRETAE_REPORT_FOR_WEEKEN = "Bạn không thể báo cáo cho ngày cuối tuần";
        public const string DONT_UPDATE_APPROVED = "Bạn không cập nhập báo cáo đã được duyệt";
        public const string DONT_DELETE_APPROVED = "Bạn không xóa báo cáo đã được duyệt";

        public const string EXPORT_REPORT_NAME = "Report_TimeSheet_";
        public const string EXPORT_PROJECT_NAME = "DanhSachProject_";
        #endregion Report

        #region Project
        public const string DONT_HASPERMISSION_UPDATE_PROJECT = "Bạn không có quyền cập nhật dự án";
        #endregion Project

    }

}
