using System.Reflection;

namespace Vitastic.App;

public static class ApplicationAssemblyReference
{
    public static Assembly Assembly
    {
        get => typeof(ApplicationAssemblyReference).Assembly;
        set => throw new NotImplementedException();
    }
}
