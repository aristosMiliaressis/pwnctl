namespace pwnctl.dto.Mediator;

using MediatR;

#pragma warning disable CA2252
public interface Request
{
    static abstract string Route { get; }
    static abstract HttpMethod Verb { get; }
}
#pragma warning restore CA2252

public interface MediatedRequest : Request, IRequest<MediatedResponse>
{

}

public interface MediatedRequest<TResult> : Request, IRequest<MediatedResponse<TResult>>
{

}