
using DbAccessLayer;

namespace BusinessLayer
{
    public class ReportHandler
    {
        private readonly ReportRepo _reportRepo;

        public ReportHandler()
        {
            _reportRepo = new ReportRepo();
        }

        // Get a list of accounts from which money was received by the user
        public List<AccountDTO> GetAccountsReceivedMoneyFrom(int userId)
        {
            // Calling the ReportRepo method that fetches accounts who sent money to the given user
            return _reportRepo.GetAccountsReceivedMoneyFromMe(userId); // Use your ReportRepo function
        }

        // Get a list of accounts to which money was sent by the user
        public List<AccountDTO> GetAccountsSentMoneyToMe(int userId)
        {
            // Calling the ReportRepo method that fetches accounts the user sent money to
            return _reportRepo.GetAccountsSentMoneyToMe(userId); // Use your ReportRepo function
        }

        // Get the total amount of money received by the user
        public decimal GetTotalMoneyReceived(int userId)
        {
            // Calling the ReportRepo method that fetches the total money received by the user
            return _reportRepo.GetTotalMoneyReceived(userId); // Use your ReportRepo function
        }

        // Get the total amount of money sent by the user
        public decimal GetTotalMoneySent(int userId)
        {
            // Calling the ReportRepo method that fetches the total money sent by the user
            return _reportRepo.GetTotalMoneySent(userId); // Use your ReportRepo function
        }
    }
}
