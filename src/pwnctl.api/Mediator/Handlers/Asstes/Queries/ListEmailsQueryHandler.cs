using pwnctl.dto.Mediator;
using pwnctl.infra.Persistence;

using MediatR;
using pwnctl.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListEmailsQueryHandler : IRequestHandler<ListEmailsQuery, MediatedResponse<EmailListViewModel>>
    {
        public async Task<MediatedResponse<EmailListViewModel>> Handle(ListEmailsQuery command, CancellationToken cancellationToken)
        {
            PwnctlDbContext context = new();
            AssetDbRepository repository = new(context);

            var emails = await repository.ListEmailsAsync();

            return MediatedResponse<EmailListViewModel>.Success(new EmailListViewModel(emails));
        }
    }
}