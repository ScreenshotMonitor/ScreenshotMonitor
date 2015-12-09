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

        public CommonDataDto GetCommonData()
        {
            return DoRequest<CommonDataDto>("GetCommonData", "nodata");
        }

        #endregion

        #region Config
        
        public object SetConfigValue(SetConfigValueDto dto)
        {
            return DoRequest<object>("SetConfigValue", dto);
        }

        #endregion

        #region Projects

        public object EditProject(ProjectChangeDto dto)
        {
            return DoRequest<object>("EditProject", dto);
        }

        public object DeleteProject(ProjectDeleteDto dto)
        {
            return DoRequest<object>("DeleteProject", dto);
        }

        #endregion
        
        #region Employments

        public object DeleteEmployment(DeleteEmploymentDto dto)
        {
            return DoRequest<CommonDataDto>("DeleteEmployment", dto);
        }

        public object InviteEmployee(IniviteEmployeeDto dto)
        {
            return DoRequest<CommonDataDto>("InviteEmployee", dto);
        }

        public object ResendEmployeeInvitationEmail(ResendEmployeeInvitationEmailDto dto)
        {
            return DoRequest<object>("ResendEmployeeInvitationEmail", dto);
        }

        public object SetEmploymentPayRate(EmploymentSetPayRateDto dto)
        {
            return DoRequest<object>("SetEmploymentPayRate", dto);
        }

        public object SetEmploymentName(EmploymentSetNameDto dto)
        {
            return DoRequest<object>("SetEmploymentName",dto);
        }

        #endregion

        #region Activities

        public object AddOfflineActivity(ActivityAddOfflineDto dto)
        {
            return DoRequest<object>("AddOfflineActivity", dto);
        }

        public List<ActivityDto> GetActivities(EmploymentRangeDto[] dto)
        {
            return DoRequest<List<ActivityDto>>("GetActivities", dto);
        }

        public object SplitActivityExact(SplitActivityDto dto)
        {
            return DoRequest<object>("SplitActivityExact", dto);
        }
        
        #endregion

        #region Screenshots

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
