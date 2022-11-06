namespace pwnctl.dto.Mediator;

using MediatR;

public interface IBaseMediatedRequest
{
    static abstract string Route { get; }
    static abstract HttpMethod Verb { get; }
}

public interface IMediatedRequest : IBaseMediatedRequest, IRequest<MediatedResponse>
{

}

public interface IMediatedRequest<TResult> : IBaseMediatedRequest, IRequest<MediatedResponse<TResult>>
{

}
