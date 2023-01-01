namespace pwnctl.kernel.Extensions;

public static class ExceptionExtensions
{
    public static string ToRecursiveExInfo(this Exception ex)
    {
        string exceptionInfo = $"--> {ex.GetType().Name}: {ex.Message}{Environment.NewLine}{ex.StackTrace}";
        if (ex.InnerException == null)
            return exceptionInfo;
        else
            return exceptionInfo + Environment.NewLine + ex.InnerException.ToRecursiveExInfo();
    }
}