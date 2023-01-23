using MediatR;
using FluentValidation;
using pwnctl.dto.Mediator;
using pwnctl.app;
using System.Net;

namespace pwnctl.api.Mediator.Pipelines
{
    public sealed class ValidationPipeline<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
            where TResponse : MediatedResponse
            where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipeline(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public ValidationPipeline()
        {
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            TResponse response = null;

            if (_validators.Any())
            {
                // in case of multiple validators for the same model
                // this pipeline will trust any model that passes atleast one validator 
                // in this case you should explicitly validate the model with the 
                // apropriate validator before passing it to MediatR
                if (_validators.All(v => !v.Validate(request).IsValid))
                {
                    var errors = _validators.SelectMany(val => val.Validate(request).Errors);

                    return (TResponse)MediatedResponse.ValidationFailure(errors);
                }
            }

            try
            {
                response = await next();
            }
            catch (ValidationException ex)
            {
                response = (TResponse)MediatedResponse.Error(ex.Message);
            }
            catch (Exception ex)
            {
                PwnInfraContext.Logger.Exception(ex);
                response = (TResponse)MediatedResponse.Create(HttpStatusCode.InternalServerError);
            }

            return response;
        }
    }
}