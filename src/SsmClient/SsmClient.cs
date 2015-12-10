using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using SsmClient.Dto;

namespace SsmClient
{
    public class SsmClient
    {
        private readonly string _authKey;
        private readonly string _baseUrl = "https://screenshotmonitor.com/api/v2/";

        public SsmClient(string authKey)
        {
            _authKey = authKey;
        }

        #region Common

        /// <summary>
        /// Return all common information for current user
        /// </summary>
        /// <returns></returns>
        public CommonDataDto GetCommonData()
        {
            return DoRequest<CommonDataDto>("GetCommonData", "nodata");
        }

        #endregion

        #region Config

        /// <summary>
        /// Set settings for all company or for individual employee (Only if user isManager for company). 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object SetConfigValue(SetConfigValueDto dto)
        {
            return DoRequest<object>("SetConfigValue", dto);
        }

        #endregion

        #region Projects

        /// <summary>
        /// Create new or edit existing project. If project 'id' exists in database, then edit project name, otherwise create new project for company
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object EditProject(ProjectChangeDto dto)
        {
            return DoRequest<object>("EditProject", dto);
        }

        /// <summary>
        /// Delete existing project
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object DeleteProject(ProjectDeleteDto dto)
        {
            return DoRequest<object>("DeleteProject", dto);
        }

        #endregion

        #region Employments

        /// <summary>
        /// Delete employemt from company or hide it from user interface
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object DeleteEmployment(DeleteEmploymentDto dto)
        {
            return DoRequest<CommonDataDto>("DeleteEmployment", dto);
        }

        /// <summary>
        /// Invite new employee by email
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object InviteEmployee(IniviteEmployeeDto dto)
        {
            return DoRequest<CommonDataDto>("InviteEmployee", dto);
        }

        /// <summary>
        /// Resend invitation for new user
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object ResendEmployeeInvitationEmail(ResendEmployeeInvitationEmailDto dto)
        {
            return DoRequest<object>("ResendEmployeeInvitationEmail", dto);
        }

        /// <summary>
        /// Set employment payrate
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object SetEmploymentPayRate(EmploymentSetPayRateDto dto)
        {
            return DoRequest<object>("SetEmploymentPayRate", dto);
        }

        /// <summary>
        /// Set employment name
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object SetEmploymentName(EmploymentSetNameDto dto)
        {
            return DoRequest<object>("SetEmploymentName",dto);
        }

        #endregion

        #region Activities

        /// <summary>
        /// Create new offline activity without screenshots (Only if setting 'disableOfflineTime' set to 'false'
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object AddOfflineActivity(ActivityAddOfflineDto dto)
        {
            return DoRequest<object>("AddOfflineActivity", dto);
        }

        /// <summary>
        /// Returns activities intersecting with given ranges array
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public List<ActivityDto> GetActivities(EmploymentRangeDto[] dto)
        {
            return DoRequest<List<ActivityDto>>("GetActivities", dto);
        }

        /// <summary>
        /// Split activity
        /// Important - time ranges for activities in array should not intersect and should combine into single range without time breaking
        /// https://screenshotmonitor.com/apidoc/#api-Activities-SplitActivityExact
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public object SplitActivityExact(SplitActivityDto dto)
        {
            return DoRequest<object>("SplitActivityExact", dto);
        }

        #endregion

        #region Screenshots

        /// <summary>
        /// Returns screenshots for given activity IDs
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public List<ScreenshotDto> GetScreenshots(Guid[] dto)
        {
            return DoRequest<List<ScreenshotDto>>("GetScreenshots", dto);
        }

        #endregion

        #region Network request

        private T DoRequest<T>(string url, object requestOdject)
        {
            var request = (HttpWebRequest) WebRequest.Create(_baseUrl + url);
            var query = JsonConvert.SerializeObject(requestOdject);
            
            request.Method = "POST";
            request.Accept = "application/json";
            request.Headers.Add("X-SSM-Token", _authKey);
            
            var data = Encoding.UTF8.GetBytes(query);
            request.ContentType = "application/json";
            request.ContentLength = data.Length;
            var dataStream = request.GetRequestStream();
            dataStream.Write(data, 0, data.Length);
            dataStream.Close();
            
            string responseString;
            try
            {
                var response = (HttpWebResponse) request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            }
            catch (WebException we)
            {
                throw we;
            }

            return JsonConvert.DeserializeObject<T>(responseString);
        }

        #endregion
    }
}
