using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using LabProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LabProject.Pages
{
    public class LoginModel : PageModel
    {
        private readonly IWebHostEnvironment _hostEnvironment;

        public LoginModel(IWebHostEnvironment hostEnvironment)
        {
            _hostEnvironment = hostEnvironment;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var usersFilePath = Path.Combine(_hostEnvironment.WebRootPath, "data", "users.json");
            var usersJson = await System.IO.File.ReadAllTextAsync(usersFilePath);
            var users = JsonConvert.DeserializeObject<List<User>>(usersJson);

           
            var user = users.FirstOrDefault(u => u.Username == Username && u.Password == Password && u.IsActive);

            if (user != null)
            {
                // Giriş başarılı, session ve cookie işlemleri
                var token = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("username", user.Username);
                HttpContext.Session.SetString("token", token);
                HttpContext.Session.SetString("session_id", HttpContext.Session.Id);

             
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddMinutes(30),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                };

                Response.Cookies.Append("username", user.Username, cookieOptions);
                Response.Cookies.Append("token", token, cookieOptions);
                Response.Cookies.Append("session_id", HttpContext.Session.Id, cookieOptions);

                
                return RedirectToPage("/Index"); 
            }
            else
            {

                ErrorMessage = "Username or password is incorrect.";
                return Page();
            }
        }
    }
}