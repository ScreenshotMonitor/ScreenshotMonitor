using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SsmClient.Dto
{
    // ReSharper disable InconsistentNaming
    public class CommonDataDto
    {
        public CompanyDto[] companies { get; set; }
        public int currentCompanyId { get; set; }
        public int accountId { get; set; }
        public int now { get; set; }
        [XmlIgnore]
        public Dictionary<string, string> values { get; set; }
    }

    public class CompanyDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public bool isManager { get; set; }
        public EmploymentDto[] employments { get; set; }
        public ProjectDto[] projects { get; set; }
        // manager-only
        // unpaid, over-free
        public string limit { get; set; }
        // settings
        //[Obsolete]
        //public int autoPauseMinutes { get; set; }
        //[Obsolete]
        //public bool disableScreenshots { get; set; }
        public ConfigDto config { get; set; }
        public SubscriptionDto subscription { get; set; }
    }

    public class ConfigDto
    {
        public int? autoPauseMinutes { get; set; }
        public int? screenshotsPerHour { get; set; }
        public int screenshotsPerHourMax { get; set; }
        public bool? disableOfflineTime { get; set; }
        public bool? disableScreenshotNotification { get; set; }
        public bool? disableActivityLevel { get; set; }
        public string currency { get; set; }
        public int? weeklyLimit { get; set; }
        public byte? weekStartDay { get; set; }
    }

    public class SubscriptionDto
    {
        public int subscriptionType { get; set; }
    }

    public class ProjectDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
    }

    public class EmploymentDto
    {
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        //public int? started { get; set; }
        public bool registered { get; set; }
        public int? lastActive { get; set; }
        //public bool deleted { get; set; }
        public bool canEdit { get; set; }
        public decimal? payRate { get; set; }
        public int? endDate { get; set; }
        // offline online away 
        public string activityStatus { get; set; }

        public  ConfigDto config { get; set; }
        public string defaultName { get; set; }
    }

    public class ActivityDto
    {
        public Guid id { get; set; }
        public int employmentId { get; set; }
        public string note { get; set; }
        public bool offline { get; set; }
        #region from/to

        [XmlIgnore]
        public DateTime fromSetter { private get; set; }
        [XmlIgnore]
        public DateTime toSetter { private get; set; }
        
        public int from { get { return ToUnixTimeStamp(fromSetter); } set {} }
        public int to { get { return ToUnixTimeStamp(toSetter); } set {} }
        #endregion
        public Guid? projectId { get; set; }


        private static int ToUnixTimeStamp(DateTime dateTime)
        {
            return (int)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }

    public class ActivityAddOfflineDto
    {
        public Guid id { get; set; }
        public int employmentId { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string note { get; set; }
        public Guid? projectId { get; set; }
    }

    public class ProjectChangeDto
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public int companyId { get; set; }
    }

    public class ProjectDeleteDto
    {
        public Guid id { get; set; }
    }

    public class EmploymentSetPayRateDto
    {
        public int id { get; set; }
        public decimal? payRate { get; set; }
    }

    public class EmploymentSetNameDto
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class IniviteEmployeeDto
    {
        public string email { get; set; }
    }

    public class DeleteEmploymentDto
    {
        public int id { get; set; }
        public bool archive { get; set; }
        public bool isPermanentlyDeleted { get; set; }
    }

    public class ResendEmployeeInvitationEmailDto
    {
        public int id { get; set; }
    }

    public class SplitActivityDto
    {
        public Guid id { get; set; }
        public ActivityChangeItemDto[] items { get; set; }
    }

    public class ActivityChangeItemDto
    {
        public Guid? id { get; set; }
        public int from { get; set; }
        public int to { get; set; }
        public string note { get; set; }
        public Guid? projectId { get; set; }
    }

    public class EmploymentRangeDto
    {
        public int employmentId { get; set; }
        public int from { get; set; }
        public int to { get; set; }
    }

    public class ScreenshotDto
    {
        public int id { get; set; }
        public Guid activityId { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public string url { get; set; }
        public string thumbUrl { get; set; }
        public int taken { get; set; }
        public byte? activityLevel { get; set; }
    }

    public class SetConfigValueDto
    {
        public int? companyId { get; set; }
        public int? employmentId { get; set; }

        /// <summary>
        /// "autoPauseMinutes", "screenshotsPerHour", "screenshotsPerHourMax", "disableOfflineTime", "disableScreenshotNotification", "currency"
        /// </summary>
        public string key { get; set; }
        public object value { get; set; }
    }
}