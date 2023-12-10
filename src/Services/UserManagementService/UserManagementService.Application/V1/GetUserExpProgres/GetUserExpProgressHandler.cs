using MediatR;
using UserManagementService.Domain.Models;

namespace UserManagementService.Application.V1.GetUserExpProgres;

public record GetUserExpProgressRequest(string userId) : IRequest<Progress>;

public class GetUserExpProgressHandler: IRequestHandler<GetUserExpProgressRequest, Progress>
{
    public Task<Progress> Handle(GetUserExpProgressRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}