namespace pwnctl.dto.Mediator;

using MediatR;

public interface IBaseMediatedRequest
{
    static abstract string Route { get; }
    static abstract HttpMethod Method { get; }
    string ReflectedConcreteRoute => (string)GetType().GetProperty(nameof(IBaseMediatedRequest.Route)).GetValue(null);
    HttpMethod ReflectedConcreteMethod => (HttpMethod)GetType().GetProperty(nameof(IBaseMediatedRequest.Method)).GetValue(null);
}

public interface IMediatedRequest : IBaseMediatedRequest, IRequest<MediatedResponse>
{

}

public interface IMediatedRequest<TResult> : IBaseMediatedRequest, IRequest<MediatedResponse<TResult>>
{

}
