namespace pwnwrk.infra.MediatR;

using System.Linq;
using System.Collections.Generic;
using FluentValidation.Results;

public class MediatorResult
{
    public object Value { get; set; }

    public bool IsSuccess => ErrorMessages == null || ErrorMessages.Count == 0;
    public List<string> ErrorMessages { get; init; }

    protected MediatorResult(string errorMsg = null)
    {
        ErrorMessages = errorMsg == null 
                    ? null 
                    : new List<string> { errorMsg };
    }

    protected MediatorResult(List<string> errorMsg)
    {
        ErrorMessages = errorMsg;
    }

    public static MediatorResult Error(string template, params string[] args)
    {
        return new MediatorResult(string.Format(template, args));
    }

    public static MediatorResult ValidationFailure(IEnumerable<ValidationFailure> validationFailures)
    {
        return new MediatorResult(validationFailures.Select(fail => fail.ErrorMessage).ToList());
    }

    public static MediatorResult Success()
    {
        return new MediatorResult();
    }
}

public class MediatorResult<TResult> : MediatorResult
{
    public new TResult Value 
    { 
        get
        {
            return (TResult) base.Value;
        }
        set
        {
            base.Value = value;
        }
    }

    private MediatorResult(TResult result, string errorMsg = null)
        : base(errorMsg)
    {
        Value = result;
    }

    public new static MediatorResult<TResult> Error(string template, params string[] args)
    {
        return new MediatorResult<TResult>(default, string.Format(template, args));
    }

    public static MediatorResult<TResult> Success(TResult result)
    {
        return new MediatorResult<TResult>(result);
    }

    public new static MediatorResult Success()
    {
        return new MediatorResult<TResult>(default);
    }
}
