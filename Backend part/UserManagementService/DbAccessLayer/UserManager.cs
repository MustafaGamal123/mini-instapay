using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DbAccessLayer
{
    public class UserManager
    {
        private readonly string connectionString;
        
        public UserManager()
        {
            connectionString  = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
        }

        public bool AddUser(string username, string password)
        {
            if (GetUser(username) != null)
            {
                return false;
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "INSERT INTO [User] (Name, Password) VALUES (@Name, @Password)";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", username);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    int result = command.ExecuteNonQuery();
                    return result > 0;
                }
            }
        }

        public UserDTO AuthUser(string userName, string password)
        {
            bool credentialsValid = false;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT COUNT(1) FROM [User] WHERE Name = @Name AND Password = @Password";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", userName);
                    command.Parameters.AddWithValue("@Password", password);

                    connection.Open();
                    int count = (int)command.ExecuteScalar();
                    credentialsValid = (count > 0);
                }
            }

            return credentialsValid ? GetUser(userName) : null;
        }

        public UserDTO GetUser(string userName)
        {
            UserDTO userDTO = null;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT ID, Name, Password, Balance FROM [User] WHERE Name = @Name";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", userName);
                    connection.Open();

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userDTO = new UserDTO
                            {
                                id = (int)reader["ID"],
                                UserName = reader["Name"].ToString(),
                                Password = reader["Password"].ToString(),
                                Balance = (decimal)reader["Balance"]
                            };
                        }
                    }
                }
            }

            return userDTO;
        }




    }
}
