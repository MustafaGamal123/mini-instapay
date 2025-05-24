using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DbAccessLayer
{
    public class TransactionRepo
    {
        // string connectionString = "Server=sqlserver-dev3;Database=UserManagemntServiceDB;User Id=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True;";

        // string connectionString = "Server=localhost,1433;Database=UserManagemntServiceDB;User Id=sa;Password=YourStrong!Passw0rd;";
      string  connectionString  = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        public bool DecreaseUserBalance(int senderId, decimal moneyValue)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [User] SET Balance = Balance - @MoneyValue WHERE ID = @SenderId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SenderId", senderId);
                    command.Parameters.AddWithValue("@MoneyValue", moneyValue);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        public bool HasEnoughBalance(int senderId, decimal moneyValue)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT Balance FROM [User] WHERE ID = @SenderId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SenderId", senderId);
                    connection.Open();

                    decimal balance = (decimal)command.ExecuteScalar();

                    return balance >= moneyValue;
                }
            }
        }
        public bool IncreaseUserBalance(int receiverId, decimal moneyValue)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "UPDATE [User] SET Balance = Balance + @MoneyValue WHERE ID = @ReceiverId";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ReceiverId", receiverId);
                    command.Parameters.AddWithValue("@MoneyValue", moneyValue);
                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        public void SaveLogs(TransactionDTO transactionDTO)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [Transaction] (SenderID, ReceiverID, Value) VALUES (@SenderID, @ReceiverID, @Value)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@SenderID", transactionDTO.SenderId);
                    command.Parameters.AddWithValue("@ReceiverID", transactionDTO.ReceiverId);
                    command.Parameters.AddWithValue("@Value", transactionDTO.MoneyValue);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        public bool UserExists(int id)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM [User] WHERE ID = @ID";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();

                    int count = (int)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        public decimal GetUserBalance(int userId)
        {
            decimal userBalance = 0;


            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT Balance
            FROM [User]
            WHERE ID = @UserID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    connection.Open();

                    object result = command.ExecuteScalar();
                    if (result != DBNull.Value && result != null)
                    {
                        userBalance = Convert.ToDecimal(result);
                    }
                }
            }

            return userBalance;
        }


        public LogsDTO GetLogs(int id)
        {

            var sentLogs = new List<logsSent>();
            var receivedLogs = new List<logsRecieved>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = @"
            SELECT SenderID, ReceiverID, Value, TransactionDate
            FROM [Transaction]
            WHERE SenderID = @ID OR ReceiverID = @ID";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", id);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int senderId = (int)reader["SenderID"];
                            int receiverId = (int)reader["ReceiverID"];
                            decimal value = (decimal)reader["Value"];
                            string date = ((DateTime)reader["TransactionDate"]).ToString("yyyy-MM-dd HH:mm:ss");

                            if (senderId == id)
                            {
                                sentLogs.Add(new logsSent
                                {
                                    receivedId = receiverId.ToString(),
                                    value = value,
                                    DateAndTime = date
                                });
                            }

                            if (receiverId == id)
                            {
                                receivedLogs.Add(new logsRecieved
                                {
                                    SenderID = senderId.ToString(),
                                    value = value,
                                    DateAndTime = date
                                });
                            }
                        }
                    }
                }
            }

            return new LogsDTO
            {
                SentTransactions = sentLogs,
                ReceivedTransactions = receivedLogs
            };
        }

        public class logsSent
        { 
            public string receivedId {  get; set; }
            public decimal value { get; set; }
            public string DateAndTime { get; set; }
        
        }


        public class LogsDTO
        {
            public List<logsSent> SentTransactions { get; set; } = new List<logsSent>();
            public List<logsRecieved> ReceivedTransactions { get; set; } = new List<logsRecieved>();


        }



        public class logsRecieved
        {
            public string SenderID { get; set; }
            public decimal value { get; set; }
            public string DateAndTime { get; set; }

        }



        public class TransactionDTO
        {
            public int SenderId { get; set; }
            public int ReceiverId { get; set; }
            public decimal MoneyValue { get; set; }

        }

    }
}