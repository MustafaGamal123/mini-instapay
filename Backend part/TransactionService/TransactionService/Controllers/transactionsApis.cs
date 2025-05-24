using BusinessLayer;
using DbAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static BusinessLayer.TransactionHandler;
using static DbAccessLayer.TransactionRepo;

namespace TransactionServices.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class transactionsApis : ControllerBase
    {
        [HttpPost()]
        public ActionResult SendMoney(TransactionDTO transactionDTO)
        {
            TransactionHandler transactionHandler = new TransactionHandler();
            TransferResult result = transactionHandler.SendMoney(transactionDTO);

            if (result.Success)
                return Ok(result.Message); // 200 OK with success message
            else
                return BadRequest(result.Message); // 400 BadRequest with failure reason
        }



        [HttpGet("GetLogs/{id}")]
        public ActionResult<LogsDTO> GetLogs(int id)
        {


            TransactionRepo transaction = new TransactionRepo();
           LogsDTO logs = transaction.GetLogs(id); // call the function that gets logs
                                                   // Optional: Return a custom message if no logs found
            if ((logs.ReceivedTransactions.Count == 0) && (logs.SentTransactions.Count == 0))
            {
                return Ok(new { message = "No transactions found for this user.", logs });
            }
            return Ok(logs);
        }


        // GET api/user/balance/{id}
        [HttpGet("balance/{id}")]
        public ActionResult GetUserBalanceApi(int id)
        {
            try
            {
                TransactionRepo transaction = new TransactionRepo();

                // Call the function to get user balance
                decimal balance = transaction.GetUserBalance(id);

                // If the balance is found (i.e., user exists), return it
                if (balance >= 0)
                {
                    return Ok(new { balance = balance });
                }
                else
                {
                    return NotFound(new { message = "User not found" });
                }
            }
            catch (Exception ex)
            {
                // If there's an error, return a 500 status with the error message
                return StatusCode(500, new { message = "An error occurred", error = ex.Message });
            }
        }






    }
}