namespace pwnctl.dto.Assets.Queries;

using pwnctl.dto.Assets.Models;
using pwnctl.dto.Mediator;

public sealed class ListDnsRecordsQuery : MediatedRequest<DnsRecordListViewModel>
{
    public static string Route => "/assets/records";
    public static HttpMethod Verb => HttpMethod.Get;
}