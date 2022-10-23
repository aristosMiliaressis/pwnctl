namespace pwnwrk.infra.MediatR;

using System.Linq;
using System.Collections.Generic;

public class MediatorResponse
{
    public object Result { get; set; }

    public bool IsSuccess => ErrorMessages == null || ErrorMessages.Count == 0;
    public List<string> ErrorMessages { get; init; }

    protected MediatorResponse(string errorMsg = null)
    {
        ErrorMessages = new List<string> { errorMsg };
    }

    protected MediatorResponse(List<string> errorMsg)
    {
        ErrorMessages = errorMsg;
    }

    public static MediatorResponse Error(string template, params string[] args)
    {
        return new MediatorResponse(string.Format(template, args));
    }

    public static MediatorResponse ValidationFailure(IEnumerable<FluentValidation.Results.ValidationFailure> validationFailures)
    {
        return new MediatorResponse(validationFailures.Select(fail => fail.ErrorMessage).ToList());
    }

    public static MediatorResponse Success()
    {
        return new MediatorResponse();
    }
}

public class MediatorResponse<TResult> : MediatorResponse
{
    public new TResult Result { get; set; }

    private MediatorResponse(TResult result, string errorMsg = null)
        : base(errorMsg)
    {
        Result = result;
    }

    public new static MediatorResponse<TResult> Error(string template, params string[] args)
    {
        return new MediatorResponse<TResult>(default, string.Format(template, args));
    }

    public static MediatorResponse<TResult> Success(TResult result)
    {
        return new MediatorResponse<TResult>(result);
    }

    public new static MediatorResponse Success()
    {
        return new MediatorResponse<TResult>(default);
    }
}
