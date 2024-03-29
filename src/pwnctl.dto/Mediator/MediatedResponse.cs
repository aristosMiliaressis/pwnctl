namespace pwnctl.dto.Mediator;

using System.Net;
using System.Linq;
using System.Collections.Generic;
using FluentValidation.Results;

public class MediatedResponse
{
    public object Result { get; set; }

    public bool IsSuccess => Errors is null || Errors.Count() == 0;
    public IEnumerable<MediatorError> Errors { get; init; }

    public MediatedResponse() {}

    private MediatedResponse(params MediatorError[] errors)
    {
        Errors = errors;
    }

    public static MediatedResponse Error(string template, params string[] args)
    {
        return new(MediatorError.GenericClientError(string.Format(template, args)));
    }

    public static MediatedResponse ValidationFailure(IEnumerable<ValidationFailure> validationFailures)
    {
        return new(validationFailures
                    .Select(fail => new MediatorError(MediatorError.ErrorType.ValidationError, fail.ErrorMessage))
                    .ToArray());
    }

    public static MediatedResponse Success()
    {
        return new();
    }

    public static MediatedResponse Create(HttpStatusCode status)
    {
        return status switch
        {
            HttpStatusCode.InternalServerError => new(MediatorError.InternalServerError),
            HttpStatusCode.Unauthorized => new(MediatorError.Unauthorized),
            _ => throw new NotImplementedException()
        };
    }
}

public sealed class MediatedResponse<TResult> : MediatedResponse
{
    public new TResult Result
    {
        get
        {
            return (TResult) base.Result;
        }
        set
        {
            base.Result = value;
        }
    }

    public MediatedResponse() {}

    private MediatedResponse(TResult result)
    {
        Result = result;
    }

    private MediatedResponse(params MediatorError[] errors)
    {
        Errors = errors;
    }

    public new static MediatedResponse<TResult> Error(string template, params string[] args)
    {
        return new(MediatorError.GenericClientError(string.Format(template, args)));
    }

    public static MediatedResponse<TResult> Success(TResult result)
    {
        return new(result);
    }

    public new static MediatedResponse Success()
    {
        return new MediatedResponse<TResult>(default(TResult));
    }
}
