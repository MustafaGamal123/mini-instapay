using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DbAccessLayer
{
    public class ReportRepo
    {
       //string connectionString = "Server=localhost,1433;Database=UserManagemntServiceDB;User Id=sa;Password=YourStrong!Passw0rd;";
        string connectionString = "Server=sqlserver-dev3;Database=UserManagemntServiceDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

        public List<AccountDTO> GetAccountsReceivedMoneyFromMe(int userId)
        {
            List<AccountDTO> receivers = new List<AccountDTO>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Query to get distinct receivers who received money from this user
                // and join with User table to get their names
                string query = @"
            SELECT DISTINCT u.ID, u.Name
            FROM [Transaction] t
            INNER JOIN [User] u ON t.ReceiverID = u.ID
            WHERE t.SenderID = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            receivers.Add(new AccountDTO
                            {
                                Id = reader.GetInt32(0),      // ID column
                                UserName = reader.GetString(1) // Name column
                            });
                        }
                    }
                }
            }

            // Return null if no receivers found
            if (receivers.Count == 0)
            {
                return null;
            }

            return receivers;
        }
        public List<AccountDTO> GetAccountsSentMoneyToMe(int userId)
        {
            List<AccountDTO> senders = new List<AccountDTO>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Query to get distinct senders who sent money to this user
                // and join with User table to get their names
                string query = @"
            SELECT DISTINCT u.ID, u.Name
            FROM [Transaction] t
            INNER JOIN [User] u ON t.SenderID = u.ID
            WHERE t.ReceiverID = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            senders.Add(new AccountDTO
                            {
                                Id = reader.GetInt32(0),      // ID column
                                UserName = reader.GetString(1) // Name column
                            });
                        }
                    }
                }
            }

            // Return null if no senders found
            if (senders.Count == 0)
            {
                return null;
            }

            return senders;
        }

        public decimal GetTotalMoneyReceived(int userId)
        {
            decimal totalReceived = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT SUM(Value) FROM [Transaction] WHERE ReceiverID = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();

                    // ExecuteScalar might return DBNull if no transactions exist
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        totalReceived = Convert.ToDecimal(result);
                    }
                    // If no transactions found or sum is null, totalReceived remains 0
                }
            }

            return totalReceived;
        }

        public decimal GetTotalMoneySent(int userId)
        {
            decimal totalSent = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT SUM(Value) FROM [Transaction] WHERE SenderID = @UserId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    connection.Open();

                    // ExecuteScalar might return DBNull if no transactions exist
                    object result = command.ExecuteScalar();

                    if (result != null && result != DBNull.Value)
                    {
                        totalSent = Convert.ToDecimal(result);
                    }
                    // If no transactions found or sum is null, totalSent remains 0
                }
            }

            return totalSent;
        }
    }
}
