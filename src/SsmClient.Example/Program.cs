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
            const string xSsmToken = "xSsmToken";

            var client = new SsmClient(xSsmToken);

            var commonData = client.GetCommonData();

            foreach (var companyDto in commonData.companies)
                Console.WriteLine(companyDto.name);
            
            // Can be one or zero companies where Account is manager
            var companyManager = commonData.companies.SingleOrDefault(c => c.isManager);

            //Only manager  can edit data
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
                //client.InviteEmployee(new IniviteEmployeeDto {email = "mail@gmail.com"});

                // Set Alias for employment
                //client.SetEmploymentName(new EmploymentSetNameDto {id = companyManager.employments.First(e => e.endDate == null).id, name = "Alias"});
            }

            // Can be many or zero companies where Account is manager
            var companyEmployee = commonData.companies.FirstOrDefault(c => !c.isManager);
            
            if (companyEmployee != null)
            {
                var twoDaysAgo =  DateTime.UtcNow.AddDays(-2);

                //Only employee can add activities
                client.AddOfflineActivity(new ActivityAddOfflineDto
                {
                    employmentId = companyEmployee.employments.First().id,
                    id = Guid.NewGuid(),
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

                var activities = client.GetActivities(ranges.ToArray());

                var subActivities = new List<ActivityChangeItemDto>
                {
                    new ActivityChangeItemDto
                    {
                       @from = ToUnixTimeStamp(twoDaysAgo),
                       to = ToUnixTimeStamp(twoDaysAgo.AddMinutes(15)),
                        id = activities.First().id,
                        projectId = null,
                        note = "Firts part"
                    },

                    new ActivityChangeItemDto
                    {
                       @from = ToUnixTimeStamp(twoDaysAgo.AddMinutes(15)),
                       to = ToUnixTimeStamp(twoDaysAgo.AddMinutes(45)),
                        id = null,
                        projectId = null,
                        note = "Trim part"
                    },
                    
                    new ActivityChangeItemDto
                    {
                       @from = ToUnixTimeStamp(twoDaysAgo.AddMinutes(45)),
                       to = ToUnixTimeStamp(twoDaysAgo.AddMinutes(60)),
                        id = Guid.NewGuid(),
                        projectId = null,
                        note = "Second part"
                    }
                };

                client.SplitActivityExact(new SplitActivityDto {id = activities.First().id, items = subActivities.ToArray()});
                    
                var screenshots = client.GetScreenshots(activities.Select(a => a.id).ToArray());

            }
            
            Console.WriteLine("End...");

            Console.ReadLine();
        }


        private static int ToUnixTimeStamp(DateTime dateTime)
        {
            return (int) dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }
    }
}
