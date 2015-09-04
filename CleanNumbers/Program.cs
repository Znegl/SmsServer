using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.WindowsAzure;
using System.Data.SqlClient;

namespace CleanNumbers
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            var connString = CloudConfigurationManager.GetSetting("dbcontext");
            var dateToCheck = DateTime.Now.AddHours(4);
            var queryString = "SELECT * FROM TeamMembers WHERE Id IN (SELECT TeamMembers.Id FROM TeamMembers INNER JOIN Teams ON TeamMembers.Team_Id = Teams.Id INNER JOIN Races ON Teams.Race_Id = Races.Id WHERE Races.[End] < '" + dateToCheck.ToString("yyyy-MM-dd") + "')";
            var updateRacesQuery = "UPDATE RACES SET ContactNumber='+4512312312', GatewayNumber='+4512312312' WHERE [End] < '" + dateToCheck.ToString("yyyy-MM-dd") + "'";
            var updateSmsQuery = "UPDATE Sms SET Sender='+4512312312' WHERE [Received] < '" + dateToCheck.ToString("yyyy-MM-dd") + "'";
            var updateTeamMembers = "UPDATE TeamMembers SET Number='+4512312312' WHERE Id IN (SELECT TeamMembers.Id FROM TeamMembers INNER JOIN Teams ON TeamMembers.Team_Id = Teams.Id INNER JOIN Races ON Teams.Race_Id = Races.Id WHERE Races.[End] < '" + dateToCheck.ToString("yyyy-MM-dd") + "')";
            using (SqlConnection connection = new SqlConnection(connString))
            {
                connection.Open();
                Console.WriteLine("State: {0}", connection.State);
                Console.WriteLine("Updating Races");
                SqlCommand command = new SqlCommand(updateRacesQuery, connection);
                var updatedRaces = command.ExecuteNonQuery();
                Console.WriteLine("Updated {0} races", updatedRaces);
                Console.WriteLine("Updating Smses");
                command = new SqlCommand(updateSmsQuery, connection);
                var updatedSmses = command.ExecuteNonQuery();
                Console.WriteLine("Updated {0} smses", updatedSmses);
                Console.WriteLine("Updating Team members");
                command = new SqlCommand(updateTeamMembers, connection);
                var updatedTeamMembers = command.ExecuteNonQuery();
                Console.WriteLine("Updated {0} team members", updatedTeamMembers);
            }
        }
    }
}
