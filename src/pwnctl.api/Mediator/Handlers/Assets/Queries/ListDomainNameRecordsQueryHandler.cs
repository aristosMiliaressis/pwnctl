using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;
using pwnctl.app.Common;
using pwnctl.app;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.Models;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListDomainNameRecordsQueryHandler : IRequestHandler<ListDomainNameRecordsQuery, MediatedResponse<DomainNameRecordListViewModel>>
    {
        public async Task<MediatedResponse<DomainNameRecordListViewModel>> Handle(ListDomainNameRecordsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var records = await repository.ListDomainNameRecordsAsync(query.Page);

            var viewModel = new DomainNameRecordListViewModel(records);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().DomainNameRecords.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<DomainNameRecordListViewModel>.Success(viewModel);
        }
    }
}