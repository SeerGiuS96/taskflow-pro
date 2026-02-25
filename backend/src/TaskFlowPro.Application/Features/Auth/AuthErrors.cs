using TaskFlowPro.Domain.Common;

namespace TaskFlowPro.Application.Features.Auth;

public static class AuthErrors
{
    public static readonly Error EmailAlreadyInUse =
        new("auth.email_already_in_use", "An account with this email already exists.");

    public static readonly Error InvalidCredentials =
        new("auth.invalid_credentials", "Email or password is incorrect.");

    public static readonly Error InvalidRefreshToken =
        new("auth.invalid_refresh_token", "The refresh token is invalid or has expired.");
}
