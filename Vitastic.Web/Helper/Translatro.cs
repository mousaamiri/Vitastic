using Vitastic.Web.Areas.Admin.Models.Course;
using Vitastic.Web.Areas.Admin.Models.Users;

namespace Vitastic.Web.Helper;

public static class Translatro
{
    public static string CrudTitle(this CrudState crudState) => crudState switch
    {
        CrudState.Create => "جدید ",
        CrudState.Read => "خواندن",
        CrudState.Update => "ویرایش ",
        CrudState.Delete => "حذف ",
        _ => crudState.ToString()
    };

    public static string LevelTranslator(this CourseUiLevel courseLevel) => courseLevel switch
    {
        CourseUiLevel.Beginner => "مقدماتی",
        CourseUiLevel.Intermediate => "متوسط",
        CourseUiLevel.Advanced => "پیشرفته",
        _ => courseLevel.ToString()
    };
}
