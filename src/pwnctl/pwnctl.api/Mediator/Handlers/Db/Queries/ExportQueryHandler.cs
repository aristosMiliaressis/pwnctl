using pwnctl.dto.Mediator;
using pwnctl.dto.Db.Queries;
using pwnctl.dto.Db.ViewModels;
using pwnwrk.infra.Persistence;

using Microsoft.EntityFrameworkCore;
using MediatR;

namespace pwnctl.api.Mediator.Handlers.Targets.Commands
{
    public sealed class ExportQueryHandler : IRequestHandler<ExportQuery, MediatedResponse<ExportViewModel>>
    {
        public async Task<MediatedResponse<ExportViewModel>> Handle(ExportQuery command, CancellationToken cancellationToken)
        {
            var viewModel = new ExportViewModel();

            PwnctlDbContext context = new();

            throw new NotImplementedException();

            return MediatedResponse<ExportViewModel>.Success(viewModel);
        }
    }
}