namespace pwnctl.dto.Mediator;

using MediatR;

public interface Request
{
    static abstract string Route { get; }
    static abstract HttpMethod Verb { get; }
}

public interface MediatedRequest : Request, IRequest<MediatedResponse>
{

}

public interface MediatedRequest<TResult> : Request, IRequest<MediatedResponse<TResult>>
{

}