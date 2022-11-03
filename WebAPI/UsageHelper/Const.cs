using System.Collections.Generic;

namespace UsageHelper
{
    public class Const
    {
        public const int MAX_ITEM_PER_PAGE = 100;

        #region SYSTEM ROLE
        public static readonly List<string> ROLE_LIST = new() { "SysAdmin", "Manager", "Developer", "HR", "Partner" };

        public const string ROLE_MEMBER = "Member";

        public const string ROLE_DEV = "Developer";

        public const string ROLE_HR = "HR";

        public const string ROLE_OTHER = "Other";

        public const string ROLE_PARTNER = "Partner";
        #endregion

        #region CACHE KEY
        public const string KEY_LEVEL = "LevelsCacheKey";

        public const string KEY_LANGUAGE = "LanguagesCacheKey";

        public const string KEY_FRAMEWORK = "FrameworksCacheKey";

        public const string KEY_POSITION = "PositionsCacheKey";

        public const string KEY_DEPARTMENT = "DepartmentsCacheKey";
        #endregion

        /// <summary>Số phòng ban sản xuất lớn nhất</summary>
        public const int MAX_DEP_MANUFACTORY = 10;

        /// <summary>Vị trí của user trong project</summary>
        public const int POSITION_PROJECT_PM = 1;
        public const int POSITION_PROJECT_MEMBER = 2;

        /// <summary>Kiểu nhân viên</summary>
        public const int USER_TYPE_INTERN = 1;
        public const int USER_TYPE_PROBATION = 2;
        public const int USER_TYPE_OFFICIAL = 3;

        public const int REPORT_STATUS_WAITING = 0;
        public const int REPORT_STATUS_APPROVED = 1;
        public const int REPORT_STATUS_REJECTED = 2;

        public const int REPORT_TYPE_NORMAL = 0;
        public const int REPORT_TYPE_OT = 1;

        /// <summary>Thời gian làm việc</summary>
        public const int TIME_WORKING_HOUR = 8;
        public const int TIME_BEGIN_HOUR = 8;
        public const int TIME_BEGIN_MINUTE = 0;
        public const int TIME_END_HOUR = 17;
        public const int TIME_END_MINUTE = 0;

        #region DEFAULT COMPANY
        public const int DEFAULT_COMPANY_ID = 0;
        public const string DEFAULT_COMPANY_NAME = "BeetSoft";
        #endregion
    }
}
