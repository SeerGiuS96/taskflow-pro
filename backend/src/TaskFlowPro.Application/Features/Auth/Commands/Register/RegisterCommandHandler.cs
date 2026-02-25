using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlowPro.Application.Common.Interfaces;
using TaskFlowPro.Domain.Common;
using TaskFlowPro.Domain.Entities;

namespace TaskFlowPro.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<RegisterResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public RegisterCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<RegisterResponse>> Handle(
        RegisterCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Check if email is already taken
        var emailExists = await _context.Users
            .AnyAsync(u => u.Email == request.Email.ToLowerInvariant(), cancellationToken);

        if (emailExists)
            return Result<RegisterResponse>.Failure(AuthErrors.EmailAlreadyInUse);

        // 2. Hash the password — never store plain text
        // IPasswordHasher is defined here in Application, implemented with BCrypt in Infrastructure
        var passwordHash = _passwordHasher.Hash(request.Password);

        // 3. Create the user entity via factory method (encapsulation)
        var user = User.Create(request.Email, passwordHash, request.DisplayName);

        // 4. Persist
        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // 5. Return success with response data
        return Result<RegisterResponse>.Success(new RegisterResponse(
            user.Id,
            user.Email,
            user.DisplayName
        ));
    }
}
