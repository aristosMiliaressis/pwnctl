namespace pwnctl.kernel.Extensions;

using Microsoft.EntityFrameworkCore;

public static class ExceptionExtensions
{
    public static string ToRecursiveExInfo(this Exception ex)
    {
        string exceptionInfo = $"--> {ex.GetType().Name}: {ex.Message}{Environment.NewLine}";

        var dbUpdateEx = ex as DbUpdateException;
        if (dbUpdateEx is not null && dbUpdateEx.Entries is not null)
        {
            var entriesInfo = string.Join(Environment.NewLine, dbUpdateEx.Entries.Select(e => e.ToString()));

            exceptionInfo += "Entries:" + Environment.NewLine + entriesInfo + Environment.NewLine;
        }

        exceptionInfo += "StackTrace:" + Environment.NewLine + ex.StackTrace;
        
        if (ex.InnerException is not null)
            exceptionInfo += Environment.NewLine + ex.InnerException.ToRecursiveExInfo();
        
        return exceptionInfo;
    }
}