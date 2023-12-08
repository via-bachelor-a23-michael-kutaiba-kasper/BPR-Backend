using MediatR;
using Microsoft.Extensions.Logging;
using UserManagementService.Infrastructure.PubSub;

namespace UserManagementService.Application.V1.ProcessExpProgress;

public record ProcessExpProgressRequest : IRequest;

public class ProcessExpProgressHandler : IRequestHandler<ProcessExpProgressRequest>
{
    private readonly ILogger<ProcessExpProgressHandler> _logger;
    private readonly IEventBus _eventBus;

    public ProcessExpProgressHandler(ILogger<ProcessExpProgressHandler> logger, IEventBus eventBus)
    {
        _logger = logger;
        _eventBus = eventBus;
    }

    public Task Handle(ProcessExpProgressRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}