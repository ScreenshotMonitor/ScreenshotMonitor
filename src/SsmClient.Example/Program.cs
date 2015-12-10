using System;
using System.Collections.Generic;
using System.Linq;
using SsmClient.Dto;

namespace SsmClient.Example
{
    internal class Program
    {
        private static void Main()
        {
            // https://screenshotmonitor.com/account
            const string xSsmToken = "xSsmToken";

            var client = new SsmClient(xSsmToken);

            var commonData = client.GetCommonData();
            
            // Can be one or zero companies where Account is manager
            var companyManager = commonData.companies.SingleOrDefault(c => c.isManager);

            //Only manager can edit this data
            if (companyManager != null)
            {
                var projectId = Guid.NewGuid();
                
                // Create new project
                client.EditProject(new ProjectChangeDto
                {
                    companyId = companyManager.id,
                    id = projectId,
                    name = "Test project"
                });

                //Edit exists project
                client.EditProject(new ProjectChangeDto
                {
                    companyId = companyManager.id,
                    id = projectId,
                    name = "Test project 2"
                });
                
                // Delete exist project
                client.DeleteProject(new ProjectDeleteDto {id = projectId});
                
                //Set setting for company
                client.SetConfigValue(new SetConfigValueDto {companyId = companyManager.id, key = "autoPauseMinutes", value = 10});

                //Set for employeee
                client.SetConfigValue(new SetConfigValueDto {employmentId = companyManager.employments.Last(e=>e.endDate ==null).id, key = "autoPauseMinutes", value = 15});

                // Invite new employee to your team
                client.InviteEmployee(new IniviteEmployeeDto {email = "mail@gmail.com"});

                // Set Alias for employment
                client.SetEmploymentName(new EmploymentSetNameDto {id = companyManager.employments.First(e => e.endDate == null).id, name = "Alias"});
            }

            // Can be many or zero companies where Account is employee
            var companyEmployee = commonData.companies.FirstOrDefault(c => !c.isManager);
            
            if (companyEmployee != null)
            {
                var twoDaysAgo =  DateTime.UtcNow.AddDays(-2);
                var activitiId = Guid.NewGuid();

                //Only employee can add activities
                client.AddOfflineActivity(new ActivityAddOfflineDto
                {
                    employmentId = companyEmployee.employments.First().id,
                    id = activitiId ,
                    projectId = null,
                    note = "Offline Activity",
                    from = ToUnixTimeStamp(twoDaysAgo),
                    to = ToUnixTimeStamp(twoDaysAgo.AddHours(1))
                });

                var ranges = new List<EmploymentRangeDto>
                {
                    new EmploymentRangeDto
                    {
                        employmentId = companyEmployee.employments.First().id,
                        @from = ToUnixTimeStamp(twoDaysAgo),
                        to = ToUnixTimeStamp(twoDaysAgo.AddHours(2))
                    }
                };

                // Get activities for ranges
                var activities = client.GetActivities(ranges.ToArray());
                
                var subActivities = new List<ActivityChangeItemDto>
                {
                    new ActivityChangeItemDto
                    {
                       @from = ToUnixTimeStamp(twoDaysAgo),
                       to = ToUnixTimeStamp(twoDaysAgo.AddMinutes(15)),
                        id = activitiId, // Original activity id
                        projectId = null,
                        note = "Firts part"
                    },

                    new ActivityChangeItemDto
                    {
                       @from = ToUnixTimeStamp(twoDaysAgo.AddMinutes(15)),
                       to = ToUnixTimeStamp(twoDaysAgo.AddMinutes(45)),
                        id = null, // If id is null then trim this time range from original activity
                        projectId = null,
                        note = "Trim part"
                    },
                    
                    new ActivityChangeItemDto
                    {
                       @from = ToUnixTimeStamp(twoDaysAgo.AddMinutes(45)),
                       to = ToUnixTimeStamp(twoDaysAgo.AddMinutes(60)),
                        id = Guid.NewGuid(), // New activity
                        projectId = null,
                        note = "Second part"
                    }
                };

                client.SplitActivityExact(new SplitActivityDto {id = activitiId, items = subActivities.ToArray()});
                    
                var screenshots = client.GetScreenshots(activities.Select(a => a.id).ToArray());
            }

            Console.ReadLine();
        }


        private static int ToUnixTimeStamp(DateTime dateTime)
        {
            return (int) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
