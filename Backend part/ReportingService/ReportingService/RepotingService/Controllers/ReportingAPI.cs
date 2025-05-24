using BusinessLayer;
using DbAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RepotingService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportingAPI : ControllerBase
    {
        [HttpGet("Total money sent")]
        public ActionResult<decimal> GetTotalMoneySent(int id)
        {
            // Check if ID is valid
            if (id < 1)
            {
                return BadRequest("Invalid ID. It must be greater than 0.");
            }

            ReportHandler reportHandler = new ReportHandler();

            // Get the total money sent
            decimal totalMoneySent = reportHandler.GetTotalMoneySent(id);

            // If money sent is found, return it
            if (totalMoneySent >= 0)
            {
                return Ok(totalMoneySent);
            }
            else
            {
                // If not found, return Not Found
                return NotFound("No transaction history found for this user.");
            }
        }

        [HttpGet("Total money received")]
        public ActionResult<decimal> GetTotalMoneyReceived(int id)
        {
            // Check if ID is valid
            if (id < 1)
            {
                return BadRequest("Invalid ID. It must be greater than 0.");
            }

            ReportHandler reportHandler = new ReportHandler();

            // Get the total money received
            decimal totalMoneyReceived = reportHandler.GetTotalMoneyReceived(id);

            // If money received is found, return it
            if (totalMoneyReceived >= 0)
            {
                return Ok(totalMoneyReceived);
            }
            else
            {
                // If not found, return Not Found
                return NotFound("No transaction history found for this user.");
            }
        }

        [HttpGet("Accounts sent money to me")]
        public ActionResult<List<AccountDTO>> GetAccountsSentMoneyToMe(int userId)
        {
            // Check if userId is valid
            if (userId < 1)
            {
                return BadRequest("Invalid ID. It must be greater than 0.");
            }

            ReportHandler reportHandler = new ReportHandler();

            // Get all accounts the user has sent money to
            List<AccountDTO> accountsSentTo = reportHandler.GetAccountsSentMoneyToMe(userId);

            // If accounts are found, return them
            if (accountsSentTo != null && accountsSentTo.Count >= 0)
            {
                return Ok(accountsSentTo);
            }
            else
            {
                // If no accounts are found, return Not Found
                return NotFound("No accounts found that the user has sent money to.");
            }
        }
        [HttpGet("Accounts received money from Me")]
        public ActionResult<List<AccountDTO>> GetAccountsReceivedMoneyFromMe(int userId)
        {
            // Check if userId is valid
            if (userId < 1)
            {
                return BadRequest("Invalid ID. It must be greater than 0.");
            }

            ReportHandler reportHandler = new ReportHandler();

            // Get all accounts the user has received money from
            List<AccountDTO> accountsReceivedFrom = reportHandler.GetAccountsReceivedMoneyFrom(userId);

            // If accounts are found, return them
            if (accountsReceivedFrom != null && accountsReceivedFrom.Count > 0)
            {
                return Ok(accountsReceivedFrom);
            }
            else
            {
                // If no accounts are found, return Not Found
                return NotFound("No accounts found that the user has received money from.");
            }
        }


    }
}
