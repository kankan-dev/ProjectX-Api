using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectX.Controllers
{
    [ApiController]
    [Route("root")]
    public class MainController : ControllerBase
    {
        public readonly projectContext _context;

        public MainController(projectContext context)
        {
            this._context = context;
        }
       [HttpGet]
       [Route("users")]

       public async Task<IActionResult> Users()
       {
            try
            {
                var query = from s in _context.Mains select s; //linq
                var simple = await _context.Mains.ToListAsync(); //shorthand
                var result = await query.ToListAsync();
                return Ok(result);
            }
            catch(Exception e)
            {
                return BadRequest(); // e.Message + e.stacktrace
            }
            

       }
       [HttpGet]
       [Route("users/{email}")]
       public async Task<IActionResult> GetUser([FromRoute] string email)
       {
            try
            {
                var query = from s in _context.Mains where s.Email == email select new 
                {
                s.Id,
                s.Uname,
                s.Address,
                s.Email,
                s.Gender,
                s.Phone,
                s.Dob,
                s.Password
                }; // or just select s
                var result = await query.ToListAsync();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
       }
        [HttpGet]
        [Route("success")]
        public async Task<IActionResult> Success([FromBody] Main m)
        {
            try
            {
                var query = from s in _context.Mains
                            where s.Email == m.Email && 
                                  s.Password == m.Password
                            select new
                            {
                                s.Id,
                                s.Uname,
                                s.Address,
                                s.Email,
                                s.Gender,
                                s.Phone,
                                s.Dob,
                                s.Password
                            }; // or just select s
                var result = await query.ToListAsync();
                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        [Route("newuser")]

        public async Task<IActionResult> CreateUser([FromBody] Main main)
        {
            if (ExistingUsername(main.Uname) || ExistingEmail(main.Email))
            {
                return BadRequest(); //"User " + main.Uname + " or "+ main.Email +" already exists"
            }
            else
            {
                await _context.Mains.AddAsync(main);
                await _context.SaveChangesAsync();
                var post_uemail = main.Email;
                var post_msg = "New user" + main.Uname + " added successfully";
                var message = new
                {
                    Msg = post_msg,
                    loggedEmail = post_uemail
                };
                return Ok(message);//"New User " + main.Uname + " added succesfully"


            }

        }
        [HttpPost]
        [Route("login")]
        public  IActionResult Login([FromBody] Main main)
        {
            if (ExistingEmail(main.Email))
            {
                if(MyLogin(main.Email, main.Password))
                {
                    var post_email = main.Email;
                    var data = new
                    {
                        uemail = post_email
                    };
                    return Ok(data);
                }
                else
                {
                    return Unauthorized();
                }
            }
            else
            {
                return NotFound();
            }
        }
        private bool ExistingUsername(string username)
        {
            bool result = _context.Mains.Any(s => s.Uname == username); //Count *
            return result;
        }
        private bool ExistingEmail(string email)
        {
            bool result = _context.Mains.Any(s => s.Email == email);
            return result;
        }
        private bool MyLogin(string email, string password)
        {
            string pwd = _context.Mains.Where(s => s.Email == email).Select(s => s.Password).First().ToString();
            if (pwd.Equals(password))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
