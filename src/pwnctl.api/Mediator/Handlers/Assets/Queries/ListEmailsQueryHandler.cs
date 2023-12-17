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
    public sealed class ListEmailsQueryHandler : IRequestHandler<ListEmailsQuery, MediatedResponse<EmailListViewModel>>
    {
        public async Task<MediatedResponse<EmailListViewModel>> Handle(ListEmailsQuery query, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var emails = await repository.ListEmailsAsync(query.Page);

            var viewModel = new EmailListViewModel(emails);

            viewModel.Page = query.Page;
            viewModel.TotalPages = new PwnctlDbContext().Emails.Count() / PwnInfraContext.Config.Api.BatchSize;

            return MediatedResponse<EmailListViewModel>.Success(viewModel);
        }
    }
}