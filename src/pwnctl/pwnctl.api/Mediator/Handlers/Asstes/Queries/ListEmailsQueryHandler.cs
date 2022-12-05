using pwnctl.dto.Mediator;
using pwnwrk.infra.Persistence;

using MediatR;
using pwnwrk.infra.Repositories;
using pwnctl.dto.Assets.Queries;
using pwnctl.dto.Assets.ViewModels;

namespace pwnctl.api.Mediator.Handlers.Targets.Queries
{
    public sealed class ListEmailsQueryHandler : IRequestHandler<ListEmailsQuery, MediatedResponse<EmailListViewModel>>
    {
        private readonly PwnctlDbContext _context = new PwnctlDbContext();

        public async Task<MediatedResponse<EmailListViewModel>> Handle(ListEmailsQuery command, CancellationToken cancellationToken)
        {
            AssetDbRepository repository = new();

            var emails = await repository.ListEmailsAsync();

            return MediatedResponse<EmailListViewModel>.Success(new EmailListViewModel(emails));
        }
    }
}