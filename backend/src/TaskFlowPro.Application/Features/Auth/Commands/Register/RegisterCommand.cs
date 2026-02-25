using MediatR;
using TaskFlowPro.Domain.Common;

namespace TaskFlowPro.Application.Features.Auth.Commands.Register;

// The command — just data, zero logic
// record = immutable, value equality, perfect for commands
public record RegisterCommand(
    string Email,
    string Password,
    string DisplayName
) : IRequest<Result<RegisterResponse>>;

// What we return on success
public record RegisterResponse(
    Guid UserId,
    string Email,
    string DisplayName
);
