using Emaktab.Database;
using Emaktab.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Emaktab.Controllers;

public class BaseController : Controller
{
    protected readonly ApplicationDbContext _context;
    protected readonly IWebHostEnvironment _env;
    protected readonly ILogger _logger;

    public BaseController(ApplicationDbContext context, IWebHostEnvironment env, ILogger logger)
    {
        _context = context;
        _env = env;
        _logger = logger;
    }

    protected bool LoadSessionData()
    {
        try
        {
            if (!HttpContext.IsLoggedIn())
                return false;

            ViewBag.user_id = HttpContext.GetUserId();
            ViewBag.usertype_id = HttpContext.GetUserType();
            ViewBag.user_fio = HttpContext.GetUserFullName();
            ViewBag.usertype_name = HttpContext.GetUserTypeName();
            ViewBag.iconPath = HttpContext.GetUserIcon();

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Session data yuklashda xatolik");
            return false;
        }
    }

    protected IActionResult RedirectToLoginIfNotAuthenticated()
    {
        if (!LoadSessionData())
            return RedirectToAction("Login", "Account");

        return null!;
    }
}