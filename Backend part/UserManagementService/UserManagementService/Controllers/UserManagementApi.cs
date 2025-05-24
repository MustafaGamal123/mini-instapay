using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Win32;
using BusinessLayer;
using DbAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
namespace UserManagementService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserManagementApi : ControllerBase
    {

        [HttpPost("SignUp")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult Registration(UserAuthDTO user)
        {
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Fields can't be empty");

            Register register = new Register();
            bool done = register.SignUp(user);

            if (done)
                return Ok("Registration Done Successfully");
            else
                return BadRequest("Failed to Register");
        }



        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<UserDTO> LoginingIn(UserAuthDTO user)
        {
            LoginHandler loginHandler = new LoginHandler();
            UserDTO userDTO = loginHandler.SignIn(user);
            if (string.IsNullOrWhiteSpace(user.UserName) || string.IsNullOrWhiteSpace(user.Password))
                return BadRequest("Fields can't be empty");
            if (userDTO == null)
                return NotFound("Wrong username or password");

            return Ok(userDTO);
        }


      /*  [HttpGet("checkuser/{id}")]
        public ActionResult<bool> CheckUser(int id)
        {
            UserManager usermanger=new UserManager();
            bool exists = usermanger.UserExists(id); // replace _yourService with your actual service instance
            return Ok(exists);
        }*/

    }

}
