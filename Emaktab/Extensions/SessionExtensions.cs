namespace Emaktab.Extensions;

public static class SessionExtensions
{
    public static int? GetUserId(this HttpContext context)
    {
        return context.Session.GetInt32("Id");
    }

    public static int? GetUserType(this HttpContext context)
    {
        return context.Session.GetInt32("UserType");
    }

    public static string? GetUserFullName(this HttpContext context)
    {
        return context.Session.GetString("user_fio");
    }

    public static string? GetUserTypeName(this HttpContext context)
    {
        return context.Session.GetString("user_type_name");
    }

    public static string? GetUserIcon(this HttpContext context)
    {
        return context.Session.GetString("iconPath");
    }

    public static bool IsLoggedIn(this HttpContext context)
    {
        return context.Session.GetInt32("Id").HasValue;
    }
}