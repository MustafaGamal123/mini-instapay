using DbAccessLayer;

using static DbAccessLayer.TransactionRepo;
using System.Net.Http;
using System.Threading.Tasks;

namespace BusinessLayer
{
    public class TransactionHandler
    {
        public class TransferResult
        {
            public bool Success { get; set; }
            public string Message { get; set; } // e.g. "Invalid receiver", "Insufficient balance"
        }
        public TransferResult SendMoney(TransactionDTO transactionDTO)
        {
            TransactionRepo transactionRepo = new TransactionRepo();

            if (!transactionRepo.UserExists(transactionDTO.ReceiverId))
                return new TransferResult { Success = false, Message = "Invalid receiver" };
            
            if (transactionDTO.SenderId == transactionDTO.ReceiverId)
                return new TransferResult { Success = false, Message = "U can't send to yourself" };
            if (transactionDTO.MoneyValue <= 0)
                return new TransferResult { Success = false, Message = "Add a valid value for money" };
            // 1. Check if receiver is valid thriugh API


            // 2. Check sender balance
            if (transactionRepo.HasEnoughBalance(transactionDTO.SenderId, transactionDTO.MoneyValue) == false)
                return new TransferResult { Success = false, Message = "Insufficient balance" };

            // 3. Decrease sender balance
            bool senderUpdated = transactionRepo.DecreaseUserBalance(transactionDTO.SenderId, transactionDTO.MoneyValue);
            if (!senderUpdated)
                return new TransferResult { Success = false, Message = "Failed to deduct sender balance" };

            // 4. Increase receiver balance
            bool receiverUpdated = transactionRepo.IncreaseUserBalance(transactionDTO.ReceiverId, transactionDTO.MoneyValue);
            if (!receiverUpdated)
            {
                // Undo sender decrease
                transactionRepo.IncreaseUserBalance(transactionDTO.SenderId, transactionDTO.MoneyValue);
                return new TransferResult { Success = false, Message = "Failed to credit receiver. Transaction canceled" };
            }

            //save Logs
            transactionRepo.SaveLogs(transactionDTO);


            // Notify the Receiver through API
            Task.Run(async () =>
            {
                await CallSetTriggerTrueApi(transactionDTO.ReceiverId); // Call your async API function
            }).Wait(); // Wait for the async operation to complete


            // 5. All good
            return new TransferResult { Success = true, Message = "Transfer completed successfully" };
        }


        private async Task CallSetTriggerTrueApi(int userId)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Define the API endpoint
                    string apiUrl = $"http://localhost:5065/api/NotifcationAPI/SetTriggerTrue/{userId}";

                    // Make the POST request
                    HttpResponseMessage response = await client.PostAsync(apiUrl, null);

                    // Check if the request was successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response
                        string responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine("Response: " + responseContent);
                    }
                    else
                    {
                        // If the request failed, handle accordingly
                        Console.WriteLine("Error: " + response.ReasonPhrase);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
        }


    }
}
