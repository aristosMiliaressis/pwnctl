namespace pwnctl.dto.Mediator;

using System.Net;
using System.Linq;
using System.Collections.Generic;
using FluentValidation.Results;

public class MediatedResponse
{
    public object Result { get; set; }

    public bool IsSuccess => Errors == null || Errors.Count == 0;
    public List<MediatorError> Errors { get; init; }

    public MediatedResponse() {}

    protected MediatedResponse(params MediatorError[] errors)
    {
        Errors = errors.ToList();
    }

    public static MediatedResponse Error(string template, params string[] args)
    {
        return new MediatedResponse(MediatorError.GenericClientError(string.Format(template, args)));
    }

    public static MediatedResponse ValidationFailure(IEnumerable<ValidationFailure> validationFailures)
    {
        return new MediatedResponse(validationFailures
                                    .Select(fail => new MediatorError(MediatorError.ErrorType.ValidationError, fail.ErrorMessage))
                                    .ToArray());
    }

    public static MediatedResponse Success()
    {
        return new MediatedResponse();
    }

    public static MediatedResponse Create(HttpStatusCode status)
    {
        return status switch
        {
            HttpStatusCode.InternalServerError => new MediatedResponse(MediatorError.InternalServerError),
            HttpStatusCode.Unauthorized => new MediatedResponse(MediatorError.Unauthorized),
            _ => throw new NotImplementedException()
        };
    }
}

public class MediatedResponse<TResult> : MediatedResponse
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

    private MediatedResponse(TResult result)
    {
        Result = result;
    }

    public new static MediatedResponse<TResult> Error(string template, params string[] args)
    {
        return (MediatedResponse<TResult>) MediatedResponse.Error(string.Format(template, args));
    }

    public static MediatedResponse<TResult> Success(TResult result)
    {
        return new MediatedResponse<TResult>(result);
    }

    public new static MediatedResponse Success()
    {
        return new MediatedResponse<TResult>(default);
    }
}