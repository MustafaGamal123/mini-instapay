
using DbAccessLayer;

namespace BusinessLayer
{
    public class NotificationHandler
    {
        NotificationRepo notificationRepo = new NotificationRepo();

        public bool CheckFlag(int userId)
        {

            return notificationRepo.GetTriggerFlag(userId);
        }

        public decimal GetLatestAddedValue(int userId)
        {
           
             decimal value=   notificationRepo.GetLatestNotificationValue(userId);

            notificationRepo.SetTriggerFlagFalse(userId);
            return value;
        }

        public void SetNotifTriggerTrue(int userId)
        {
         notificationRepo.SetTriggerFlagTrue(userId);
        }
    }
}
