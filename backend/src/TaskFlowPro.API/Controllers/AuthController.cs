using MediatR;
using Microsoft.AspNetCore.Mvc;
using TaskFlowPro.Application.Features.Auth.Commands.Register;

namespace TaskFlowPro.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Register a new user account
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request.Email, request.Password, request.DisplayName);
        var result = await _mediator.Send(command);

        if (result.IsFailure)
            return Conflict(new { error = result.Error.Code, message = result.Error.Message });

        return CreatedAtAction(nameof(Register), result.Value);
    }
}

// Request DTO — what the HTTP body must contain
public record RegisterRequest(string Email, string Password, string DisplayName);
