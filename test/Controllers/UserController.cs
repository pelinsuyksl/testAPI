using System;
using System.Linq;
using System.Web.Http;
using test.Models;

namespace test.Controllers
{
    public class RegisterDto
    {
        public string FullName {  get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone {  get; set; }
    } 
    public class RemindIdDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    
    public class ChangePasswordDto
    {
        public string Email { get; set; }
        public string p_old {  get; set; }
        public string p_new { get; set; }
    }
    public class UserController : ApiController
    {
        private readonly AppDbContext context;
        public UserController()
        {
            context = new AppDbContext();
        }


        [HttpPost]
        [Route("api/user/register")]
        public IHttpActionResult Register(RegisterDto request)
        {
            if (string.IsNullOrEmpty(request.FullName))
                return Ok(new 
                {    Message= "Full name cannot be empty!!",
                     Status=false
                });
            if(string.IsNullOrEmpty(request.Phone))
                return Ok(new 
                {   Message= "Phone cannot be empty!!",
                    Status = false
                });
            if(string.IsNullOrEmpty(request.Email))
                return Ok(new
                {
                    Message = "Email cannot be empty!!",
                    Status = false
                });
            if (string.IsNullOrEmpty(request.Password))
                return Ok(new
                {
                    Message = "Password cannot be empty!!",
                    Status = false
                });
            var x=context.Users.FirstOrDefault(y=> y.Email == request.Email);
            if (x == null)
            {
                string passwordHash, passwordSalt;
                MakePassword.createPasswordHash(request.Password, out passwordHash, out passwordSalt);

                var user = new User
                {
                    FullName = request.FullName,
                    Phone = request.Phone,
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    PasswordSalt = passwordSalt,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UserDevCount = 0
                };
                context.Users.Add(user);
                context.SaveChanges();

                return Ok(new
                {
                    Message = "User has been created succesfully!",
                    UserId = user.Id,
                    Status = true
                });
            }
            else
            {
                return Ok(new
                {
                    Message= "Already have an account. Please log in!",
                    UserId = x.Id,
                    Status = false
                });
            }
        }
        
    
        [HttpPost]
        [Route("api/user/remindId")]
        public IHttpActionResult RemindId(RemindIdDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return Ok(new
                {
                    Message = "Email is required!!",
                    Status = false
                });
            if (string.IsNullOrEmpty(request.Password))
                return Ok(new
                {
                    Message = "Password is required!!",
                    Status = false
                });

            var user = context.Users.FirstOrDefault(u => u.Email == request.Email);
            if (user == null)
                return Ok(new
                {
                    Message = "Invalid Email!!",
                    Status = false
                });
            if (!user.IsActive)
                return Ok(new
                {
                    Message = "Your account is passive or suspended!!",
                    Status = false
                });

            bool p_correct = MakePassword.VerifyPasswordHash(
                request.Password,
                user.PasswordHash,
                user.PasswordSalt
            );
            if (!p_correct)
                return Ok(new
                {
                    Message = "Invalid Password!!",
                    Status = false
                });

            return Ok(new 
            {
                Message = "Login successful",
                UserId= user.Id,
                Status = true
             });
        }


        [HttpPost]
        [Route("api/user/password_change")]
        public IHttpActionResult ChangePassword(ChangePasswordDto request)
        {
            if (string.IsNullOrEmpty(request.Email))
                return Ok(new
                {
                    Message = "Email is required!!",
                    Status = false
                });
            if (string.IsNullOrEmpty(request.p_old))
                return Ok(new
                {
                    Message = "Old Password is required!!",
                    Status = false
                });

            if (string.IsNullOrEmpty(request.p_new))
                return Ok(new
                {
                    Message = "New Password is required!!",
                    Status = false
                });

            if (request.p_new == request.p_old)
                return Ok(new
                {
                    Message = "New password can not be the same as the old password!!",
                    Status = false
                });

            var user = context.Users.FirstOrDefault(u => u.Email == request.Email);

            if (user == null)
                return Ok(new
                {
                    Message = "No user with this email!",
                    Status = false
                });

            bool isOld_pcorrect = MakePassword.VerifyPasswordHash(
                request.p_old, 
                user.PasswordHash, 
                user.PasswordSalt
            );

            if (!isOld_pcorrect)
                return Ok(new
                {
                    Message = "Current password written is incorrect.",
                    Status = false
                });

            string newPwHash, newPwSalt;
            MakePassword.createPasswordHash(request.p_new,out  newPwHash,out newPwSalt);

            user.PasswordHash= newPwHash;
            user.PasswordSalt=newPwSalt;
            user.UpdatedAt= DateTime.UtcNow;

            context.SaveChanges();
            return Ok(new
            {
                Message = "Password has been changed correctly!",
                Status = true
            });
        }
        


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                context.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}