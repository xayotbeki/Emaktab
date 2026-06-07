using Emaktab.Models;
using Emaktab.Service;
using Emaktab.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Emaktab.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext dbContext,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _context = dbContext;
            _env = env;
        }

        private bool LoadSessionData()
        {
            try
            {
                ViewBag.user_fio = HttpContext.Session.GetString("user_fio");
                ViewBag.usertype_name = HttpContext.Session.GetString("user_type_name");
                ViewBag.iconPath = HttpContext.Session.GetString("iconPath");
                ViewBag.user_id = HttpContext.Session.GetInt32("Id");
                ViewBag.usertype_id = HttpContext.Session.GetInt32("UserType");

                return ViewBag.user_id != null && ViewBag.usertype_id != null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Session o‘qishda xatolik");
                return false;
            }
        }
        private IActionResult? CheckLogin()
        {
            if (!LoadSessionData())
                return RedirectToAction("Login", "Login");

            return null;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Index1()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            ViewBag.user_fio = HttpContext.Session.GetString("user_fio");
            ViewBag.usertype_name = HttpContext.Session.GetString("user_type_name");

            return View();
        }

        public IActionResult Index2()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;

            var totalScore = _context.Documents
                .Where(x => x.user_id == userId && x.status_id == (int)DocStatus.QabulQilindi)
                .Sum(x => (int?)x.Score) ?? 0;

            ViewBag.TotalScore = totalScore;

            return View();
        }

        public async Task<IActionResult> ListUser()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int? userType = ViewBag.usertype_id;
            if (userType == (int)UsertType.Admin || userType == (int)UsertType.SuperAdmin)
            {
                var users = await _context.Users.ToListAsync();
                return View(users);
            }

            TempData["errorMessage"] = "Sizda bu sahifaga kirish huquqi yo‘q.";
            return RedirectToAction(nameof(Index1));
        }

        public IActionResult MyData()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                TempData["errorMessage"] = "Foydalanuvchi topilmadi.";
                return RedirectToAction(nameof(Index1));
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditMydata(User model)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            var user = _context.Users.FirstOrDefault(u => u.Id == model.Id);
            if (user == null)
            {
                TempData["errorMessage"] = "Foydalanuvchi topilmadi.";
                return RedirectToAction(nameof(MyData));
            }

            try
            {
                user.user_fio = model.user_fio;
                user.user_email = model.user_email;
                user.firstname = model.firstname;
                user.lastname = model.lastname;
                user.birthday = model.birthday;
                user.Pinfl = model.Pinfl;
                user.uzb_lang = model.uzb_lang;
                user.rus_lang = model.rus_lang;
                user.eng_lang = model.eng_lang;
                user.other_lang = model.other_lang;
                user.user_location = model.user_location;
                user.user_level = model.user_level;
                user.phone_number = model.phone_number;
                user.tajribasi = model.tajribasi;
                user.Pasport_seria = model.Pasport_seria;
                user.Pasport_number = model.Pasport_number;
                user.Biography = model.Biography;

                _context.SaveChanges();

                TempData["successMessage"] = "Ma'lumotlar muvaffaqiyatli yangilandi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "MyData update xatolik");
                TempData["errorMessage"] = "Ma'lumotlarni yangilashda xatolik yuz berdi.";
            }

            return RedirectToAction(nameof(MyData));
        }

        public IActionResult Profile()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var user = _context.Users.FirstOrDefault(x => x.Id == userId);

            if (user == null)
            {
                TempData["errorMessage"] = "Foydalanuvchi topilmadi.";
                return RedirectToAction(nameof(Index1));
            }

            var model = new UpdateUserViewModel
            {
                Id = user.Id,
                Firstname = user.firstname,
                Lastname = user.lastname,
                UserFio = user.user_fio,
                UserEmail = user.user_email,
                UserLogin = user.user_login
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UpdateUserViewModel model)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
            {
                TempData["errorMessage"] = "Foydalanuvchi topilmadi.";
                return RedirectToAction(nameof(Profile));
            }

            try
            {
                user.firstname = model.Firstname;
                user.lastname = model.Lastname;
                user.user_fio = model.UserFio;
                user.user_email = model.UserEmail;
                user.user_login = model.UserLogin;

                if (!string.IsNullOrWhiteSpace(model.NewPassword))
                {
                    var currentHash = HashService.HashPassword(model.CurrentPassword ?? "");
                    if (currentHash == user.user_password)
                    {
                        user.user_password = HashService.HashPassword(model.NewPassword);
                    }
                    else
                    {
                        TempData["errorMessage"] = "Joriy parol noto‘g‘ri.";
                        return RedirectToAction(nameof(Profile));
                    }
                }

                if (model.Icon != null && model.Icon.Length > 0)
                {
                    string folder = Path.Combine(_env.WebRootPath, "uploads", "icons");
                    if (!Directory.Exists(folder))
                        Directory.CreateDirectory(folder);

                    string extension = Path.GetExtension(model.Icon.FileName).ToLower();
                    string[] allowed = { ".jpg", ".jpeg", ".png", ".webp" };

                    if (!allowed.Contains(extension))
                    {
                        TempData["errorMessage"] = "Faqat rasm fayllar yuklash mumkin.";
                        return RedirectToAction(nameof(Profile));
                    }

                    string fileName = Guid.NewGuid() + extension;
                    string fullPath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await model.Icon.CopyToAsync(stream);
                    }

                    user.IconPath = "/uploads/icons/" + fileName;
                    HttpContext.Session.SetString("iconPath", user.IconPath);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                HttpContext.Session.SetString("user_fio", user.user_fio ?? "");
                HttpContext.Session.SetString("user_type_name", user.user_type_name ?? "");

                TempData["successMessage"] = "Profil muvaffaqiyatli yangilandi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profile update xatolik");
                TempData["errorMessage"] = "Profilni yangilashda xatolik yuz berdi.";
            }

            return RedirectToAction(nameof(Profile));
        }

        public IActionResult ListBlog()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var blogs = _context.Blogs
                .Where(x => x.create_userId == userId)
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(blogs);
        }

        [HttpGet]
        public IActionResult CreateBlog()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            return View(new Blog());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateBlog(Blog model)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            try
            {
                int? userId = HttpContext.Session.GetInt32("Id");

                if (userId == null)
                {
                    TempData["errorMessage"] = "Login talab qilinadi";
                    return RedirectToAction("Index", "Login");
                }

                if (!ModelState.IsValid)
                {
                    TempData["errorMessage"] = "Ma'lumotlar noto‘g‘ri";
                    return View(model);
                }

                model.create_userId = userId.Value;
                model.CreateDate = DateOnly.FromDateTime(DateTime.Now);
                model.StatusId = (int)DocStatus.Saqlangan;

                _context.Blogs.Add(model);
                _context.SaveChanges();

                TempData["successMessage"] = "Blog saqlandi";

                return RedirectToAction(nameof(ListBlog));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateBlog error");
                TempData["errorMessage"] = "Xatolik: " + ex.Message;

                return View(model);
            }
        }

        public async Task<IActionResult> EditBlog(int blogId)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == blogId && b.create_userId == userId);

            if (blog == null)
            {
                TempData["errorMessage"] = "Blog topilmadi.";
                return RedirectToAction(nameof(ListBlog));
            }

            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditBlog(Blog model)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == model.Id && b.create_userId == userId);

            if (blog == null)
            {
                TempData["errorMessage"] = "Blog topilmadi.";
                return RedirectToAction(nameof(ListBlog));
            }

            blog.Title = model.Title;
            blog.Description = model.Description;
            blog.Comment = model.Comment;
            blog.LanguageType = model.LanguageType;

            await _context.SaveChangesAsync();

            TempData["successMessage"] = "Blog yangilandi.";
            return RedirectToAction(nameof(ListBlog));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int docId, int statusId)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == docId && b.create_userId == userId);

            if (blog == null)
            {
                TempData["errorMessage"] = "Blog topilmadi.";
                return RedirectToAction(nameof(ListBlog));
            }

            blog.StatusId = statusId;
            await _context.SaveChangesAsync();

            TempData["successMessage"] = "Blog statusi yangilandi.";
            return RedirectToAction(nameof(ListBlog));
        }

        public async Task<IActionResult> DeleteBlog(int id)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && b.create_userId == userId);

            if (blog == null)
            {
                TempData["errorMessage"] = "Blog topilmadi.";
                return RedirectToAction(nameof(ListBlog));
            }

            _context.Blogs.Remove(blog);
            await _context.SaveChangesAsync();

            TempData["successMessage"] = "Blog o‘chirildi.";
            return RedirectToAction(nameof(ListBlog));
        }

        public async Task<IActionResult> DetailsBlog(int id)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var blog = await _context.Blogs.FirstOrDefaultAsync(b => b.Id == id && b.create_userId == userId);

            if (blog == null)
            {
                TempData["errorMessage"] = "Blog topilmadi.";
                return RedirectToAction(nameof(ListBlog));
            }

            return View(blog);
        }

        public async Task<IActionResult> ImportDoc()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var docs = await _context.Documents
                .Where(x => x.user_id == userId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(docs);
        }

        public async Task<IActionResult> ImportDocPed()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var docs = await _context.Documents
                .Where(x => x.user_id == userId)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            return View(docs);
        }

        public IActionResult ImportDocPsix()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoc(Document100 model, int? status_id)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            string backAction = "ImportDoc";

            if (ViewBag.usertype_id != null && (int)ViewBag.usertype_id == (int)UsertType.Pedagok)
            {
                backAction = "ImportDocPed";
            }

            if (model.UploadDocument == null || model.UploadDocument.Length == 0)
            {
                TempData["errorMessage"] = "Fayl tanlanmagan.";
                return RedirectToAction(backAction);
            }

            try
            {
                string extension = Path.GetExtension(model.UploadDocument.FileName).ToLower();
                string[] allowedExtensions = { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png" };

                if (!allowedExtensions.Contains(extension))
                {
                    TempData["errorMessage"] = "Ruxsat etilmagan fayl turi.";
                    return RedirectToAction(backAction);
                }

                string uploadsFolder = Path.Combine(_env.WebRootPath, "Upload");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid() + extension;
                string fullPath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await model.UploadDocument.CopyToAsync(stream);
                }

                model.upload_date = DateTime.Now;
                model.create_date = model.create_date == default ? DateTime.Now : model.create_date;
                model.user_id = (int)ViewBag.user_id;
                model.FilePath = "/Upload/" + uniqueFileName;
                model.status_id = status_id ?? (int)DocStatus.Saqlangan;
                model.Status = (DocStatus)model.status_id.Value;

                _context.Documents.Add(model);
                await _context.SaveChangesAsync();

                TempData["successMessage"] = "Hujjat muvaffaqiyatli yuklandi.";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateDoc xatolik");
                TempData["errorMessage"] = ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(backAction);
        }

        public async Task<IActionResult> DetailsDoc(int id)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var doc = await _context.Documents.FirstOrDefaultAsync(x => x.Id == id && x.user_id == userId);

            if (doc == null)
            {
                TempData["errorMessage"] = "Hujjat topilmadi.";
                return RedirectToAction(nameof(ImportDoc));
            }

            return View(doc);
        }

        public async Task<IActionResult> EditDoc(int id)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var doc = await _context.Documents.FirstOrDefaultAsync(x => x.Id == id && x.user_id == userId);

            if (doc == null)
            {
                TempData["errorMessage"] = "Hujjat topilmadi.";
                return RedirectToAction(nameof(ImportDoc));
            }

            return View(doc);
        }

        public IActionResult Tajriba()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            return View();
        }

        public async Task<IActionResult> EditStatusDoc(int statusId, int docId)
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;
            var doc = await _context.Documents.FirstOrDefaultAsync(x => x.Id == docId && x.user_id == userId);

            if (doc == null)
            {
                TempData["errorMessage"] = "Hujjat topilmadi.";
                return RedirectToAction(nameof(ImportDoc));
            }

            doc.status_id = statusId;
            doc.Status = (DocStatus)statusId;

            await _context.SaveChangesAsync();

            TempData["successMessage"] = "Hujjat statusi yangilandi.";
            return RedirectToAction(nameof(ImportDoc));
        }

        public async Task<IActionResult> IshReja()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;

            var list = await _context.IshRejalar
                .Where(x => x.UserId == userId && x.Active == true)
                .OrderBy(x => x.Sana)
                .ToListAsync();

            return View(list);
        }

        public IActionResult Portfolio()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            int userId = (int)ViewBag.user_id;

            var user = _context.Users.FirstOrDefault(x => x.Id == userId);
            if (user == null)
            {
                TempData["errorMessage"] = "Foydalanuvchi topilmadi.";
                return RedirectToAction(nameof(Index2));
            }

            return View(user);
        }

        public IActionResult BlogApprove()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            var blogs = _context.Blogs
                .Include(x => x.User)
                .Where(x => x.StatusId == (int)DocStatus.Yuborilgan)
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(blogs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ApproveBlog(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(x => x.Id == id);

            if (blog == null)
            {
                TempData["errorMessage"] = "Blog topilmadi";
                return RedirectToAction("BlogApprove");
            }

            blog.StatusId = (int)DocStatus.QabulQilindi;

            _context.Blogs.Update(blog);
            _context.SaveChanges();

            TempData["successMessage"] = "Blog tasdiqlandi";
            return RedirectToAction("BlogApprove");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RejectBlog(int id)
        {
            var blog = _context.Blogs.FirstOrDefault(x => x.Id == id);

            if (blog == null)
            {
                TempData["errorMessage"] = "Blog topilmadi";
                return RedirectToAction("BlogApprove");
            }

            blog.StatusId = (int)DocStatus.Qaytarildi;

            _context.Blogs.Update(blog);
            _context.SaveChanges();

            TempData["successMessage"] = "Blog rad etildi";
            return RedirectToAction("BlogApprove");
        }

        public IActionResult KPI()
        {
            return View();
        }
        public IActionResult Opportunities()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }

        public IActionResult Users()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            var users = _context.Users
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(users);
        }

        public IActionResult AllDocuments()
        {
            var auth = CheckLogin();
            if (auth != null) return auth;

            var docs = _context.Documents
                .Include(x => x.User)
                .OrderByDescending(x => x.Id)
                .ToList();

            return View(docs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDocStatus(int id, int statusId, int? score)
        {
            var doc = _context.Documents.FirstOrDefault(x => x.Id == id);

            if (doc == null)
            {
                TempData["errorMessage"] = "Topilmadi";
                return RedirectToAction("AllDocuments");
            }

            doc.status_id = statusId;

            if (score.HasValue && score > 0)
            {
                doc.Score = score.Value;
            }

            _context.SaveChanges();

            TempData["successMessage"] = "Yangilandi";
            return RedirectToAction("AllDocuments");
        }

        public IActionResult DownloadFile(int id)
        {
            var doc = _context.Documents.FirstOrDefault(x => x.Id == id);

            if (doc == null || string.IsNullOrEmpty(doc.FilePath))
            {
                TempData["errorMessage"] = "Fayl topilmadi";
                return RedirectToAction("AllDocuments");
            }

            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", doc.FilePath);

            if (!System.IO.File.Exists(fullPath))
            {
                TempData["errorMessage"] = "Fayl mavjud emas";
                return RedirectToAction("AllDocuments");
            }

            var fileBytes = System.IO.File.ReadAllBytes(fullPath);

            var fileName = Path.GetFileName(fullPath);

            return File(fileBytes, "application/octet-stream", fileName);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateDoc(int docId, int statusId, int score)
        {
            var doc = _context.Documents.FirstOrDefault(x => x.Id == docId);

            if (doc == null)
            {
                TempData["errorMessage"] = "Topilmadi";
                return RedirectToAction("AllDocuments");
            }

            doc.status_id = statusId;
            doc.Score = score;

            _context.SaveChanges();

            TempData["successMessage"] = "Yangilandi";
            return RedirectToAction("AllDocuments");
        }
    }
}