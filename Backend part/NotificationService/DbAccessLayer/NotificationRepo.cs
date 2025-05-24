using System.Data.SqlClient;

namespace DbAccessLayer
{

    public class NotificationRepo
    {
        //  string connectionString = "Server=localhost,1433;Database=UserManagemntServiceDB;User Id=sa;Password=YourStrong!Passw0rd;";
        //  string connectionString = "Server=sqlserver-dev3;Database=UserManagemntServiceDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";
        string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        public bool GetTriggerFlag(int userId)
        {
            bool triggerValue = false; // Default to false if not found

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT [Trigger] FROM TriggerFlag WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        triggerValue = (bool)result;
                    }
                }
            }

            return triggerValue;
        }



       
     

        public decimal GetLatestNotificationValue(int userId)
        {
            decimal lastTransactionValue = 0;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT TOP 1 Value
            FROM [Transaction]
            WHERE ReceiverID = @UserID
            ORDER BY ID DESC";  // Changed from TransactionDate to ID

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        lastTransactionValue = Convert.ToDecimal(result);
                    }
                }
            }

            return lastTransactionValue;
        }

        public void SetTriggerFlagTrue(int userId)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                // Check if the UserID exists in the TriggerFlag table
                string checkQuery = @"
            SELECT COUNT(*) 
            FROM TriggerFlag 
            WHERE UserID = @UserID";

                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();
                    int userCount = (int)checkCommand.ExecuteScalar();

                    if (userCount > 0)
                    {
                        // If the UserID exists, update the Trigger flag
                        string updateQuery = @"
                    UPDATE TriggerFlag
                    SET [Trigger] = 1
                    WHERE UserID = @UserID";

                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            updateCommand.Parameters.AddWithValue("@UserID", userId);
                            updateCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // If the UserID does not exist, insert a new row with Trigger = 1
                        string insertQuery = @"
                    INSERT INTO TriggerFlag (UserID, [Trigger])
                    VALUES (@UserID, 1)";

                        using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                        {
                            insertCommand.Parameters.AddWithValue("@UserID", userId);
                            insertCommand.ExecuteNonQuery();
                        }
                    }
                }
            }
        }


        public void SetTriggerFlagFalse(int userId)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            UPDATE TriggerFlag
            SET [Trigger] = 0
            WHERE UserID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }


    }



}
