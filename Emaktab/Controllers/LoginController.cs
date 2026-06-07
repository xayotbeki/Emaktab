using Emaktab.Models;
using Emaktab.Service;
using Emaktab.Database;
using Microsoft.AspNetCore.Mvc;

namespace Emaktab.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoginController(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(LoginRequest model)
        {
            if (string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
            {
                ModelState.AddModelError("", "Login yoki parol bo‘sh bo‘lmasligi kerak");
                return View(model);
            }

            string hashedPassword = HashService.HashPassword(model.Password);

            var user = _context.Users
                .FirstOrDefault(u => u.user_login == model.Username && u.user_password == hashedPassword);

            if (user == null)
            {
                ModelState.AddModelError("", "Login yoki parol noto‘g‘ri");
                return View(model);
            }

            HttpContext.Session.SetString("UserLogin", user.user_login ?? "");
            HttpContext.Session.SetString("user_fio", user.user_fio ?? "");
            HttpContext.Session.SetString("user_type_name", user.user_type_name ?? "");
            HttpContext.Session.SetString("iconPath", user.IconPath ?? "");
            HttpContext.Session.SetInt32("Id", user.Id);
            HttpContext.Session.SetInt32("UserType", (int)user.UsertType);

            if (user.UsertType == UsertType.Admin)
            {
                return RedirectToAction("Index1", "Home");
            }
            else if (user.UsertType == UsertType.Pedagok)
            {
                return RedirectToAction("Index2", "Home");
            }

            return View();
        }

        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(RegisterRequest model)
        {
            if (string.IsNullOrWhiteSpace(model.FullName) ||
                string.IsNullOrWhiteSpace(model.Username) ||
                string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("", "F.I.O, login va parol kiritilishi shart");
                return View(model);
            }

            var loginExists = _context.Users.FirstOrDefault(x => x.user_login == model.Username);
            if (loginExists != null)
            {
                ModelState.AddModelError("", "Bu login allaqachon mavjud");
                return View(model);
            }

            if (!string.IsNullOrWhiteSpace(model.Email))
            {
                var emailExists = _context.Users.FirstOrDefault(x => x.user_email == model.Email);
                if (emailExists != null)
                {
                    ModelState.AddModelError("", "Bu email allaqachon mavjud");
                    return View(model);
                }
            }

            string hashedPassword = HashService.HashPassword(model.Password);

            var user = new User
            {
                user_fio = model.FullName,
                firstname = model.FullName,
                lastname = "",

                user_login = model.Username,
                user_password = hashedPassword,
                user_email = model.Email,
                phone_number = model.PhoneNumber ?? "",

                UsertType = UsertType.Pedagok,
                user_typeId = (int)UsertType.Pedagok,
                user_type_name = UsertType.Pedagok.ToString(),

                active = true,
                create_date = DateTime.Now,

                birthday = DateTime.Now,
                user_workplace = null,

                Pinfl = "",
                Pasport_seria = "",
                Pasport_number = 0,

                user_location = "",
                user_level = "",
                Biography = "",

                uzb_lang = 1,
                eng_lang = 0,
                rus_lang = 0,
                other_lang = 0,
                tajribasi = 0,

                IconPath = @"D:\\Emaktab\\Emaktab\\wwwroot\\images\\iconFace.png"
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Ro‘yxatdan o‘tish muvaffaqiyatli bajarildi";

            return RedirectToAction("Index");
        }
    }
}