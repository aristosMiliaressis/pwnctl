namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.ViewModels;
using pwnctl.dto.Mediator;

using MediatR;

public sealed class ListDnsRecordsQuery : IMediatedRequest<DnsRecordListViewModel>
{
    public static string Route => "/assets/records";
    public static HttpMethod Verb => HttpMethod.Get;
}