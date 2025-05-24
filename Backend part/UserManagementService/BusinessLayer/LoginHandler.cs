

using DbAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer

{
    
        public class LoginHandler
        {
            public UserDTO SignIn(UserAuthDTO user)
            {
                UserManager userManager = new UserManager();

                return userManager.AuthUser(user.UserName, user.Password);
            }
        }
    
}
