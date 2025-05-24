using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbAccessLayer;

namespace BusinessLayer
{
    public class Register
    {
        UserManager addUser;

        public Register()
        {
            addUser = new UserManager();

        }
        public bool SignUp(UserAuthDTO user)
        {
            return addUser.AddUser(user.UserName, user.Password);

        }
    }
}
