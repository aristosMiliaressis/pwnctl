namespace pwnctl.kernel.Extensions;

using Microsoft.EntityFrameworkCore;

public static class ExceptionExtensions
{
    public static string ToRecursiveExInfo(this Exception ex)
    {
        string exceptionInfo = $"--> {ex.GetType().Name}: {ex.Message}{Environment.NewLine}";

        if (ex is DbUpdateException)
        {
            var dbUpdateEx = ex as DbUpdateException;

            var entriesInfo = string.Join(Environment.NewLine, dbUpdateEx.Entries.Select(e => e.ToString()));

            exceptionInfo += "Entries:" + Environment.NewLine + entriesInfo + Environment.NewLine;
        }

        exceptionInfo += "StackTrace:" + Environment.NewLine + ex.StackTrace;
        
        if (ex.InnerException != null)
            exceptionInfo += Environment.NewLine + ex.InnerException.ToRecursiveExInfo();
        
        return exceptionInfo;
    }
}