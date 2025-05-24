using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BusinessLayer;

namespace NotifcationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotifcationAPI : ControllerBase
    {
        [HttpGet("GetTriggerFlag/{userId}")]
        public ActionResult<bool> GetTriggerFlagApi(int userId)
        {
          NotificationHandler notificationHandler = new NotificationHandler();

            bool flag = notificationHandler.CheckFlag(userId);
            return Ok(flag);
        }

        [HttpGet("LatestAddedValue/{userId}")]
        public ActionResult<decimal> GetLatestNotificationValueApi(int userId)
        {
            NotificationHandler notificationHandler = new NotificationHandler();

            decimal value = notificationHandler.GetLatestAddedValue(userId);
            return Ok(value);
        }

        [HttpPost("SetTriggerTrue/{userId}")]
        public IActionResult SetTriggerTrue(int userId)
        {
            try
            {
                // Call your function
                NotificationHandler notificationHandler = new NotificationHandler();
                notificationHandler.SetNotifTriggerTrue(userId);

                return Ok(new { message = "Trigger flag set to true successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred.", error = ex.Message });
            }
        }


    }
}
