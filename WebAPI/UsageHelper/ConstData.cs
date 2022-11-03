using System.Collections.Generic;

namespace UsageHelper
{
    public static class ConstData
    {
        public static List<string> positions = new List<string> { "Product Manager", "Developer", "Brse", "Comtor", "Tester", "Other" };
        public static string[] roles = new string[] { "Manager", "Leader", "Developer" };
        public static List<string>  projectTypes = new List<string> { "Project Base", "Labo", "Onsite", "Maintenance", "Other" };
        public static List<string> LanguageData = new List<string>()
        {
           "Tiếng Anh" ,
            "Tiếng Nhật" ,
            "Tiếng Hàn" ,
            "Không Yêu Cầu",
        };

        public static List<string> FrameworkData = new List<string>()
        {
            "C#" ,
            "Java" ,
            "PHP" ,
            "React JS",
            "Python" ,
        };

        public static List<string> LevelData = new List<string>()
        {
            "TTS",
            "Intern",
           "Fresher",
            "Junior",
            "Senior",
           "Teachlead"
        };       
    }
}
