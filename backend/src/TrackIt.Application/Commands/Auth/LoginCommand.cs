using AutoMapper;
using FluentValidation;
using MediatR;
using TrackIt.Application.Common.Exceptions;
using TrackIt.Application.DTOs;
using TrackIt.Application.Interfaces;
using TrackIt.Domain.Interfaces;

namespace TrackIt.Application.Commands.Auth;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordService passwordService,
    IJwtService jwtService,
    IMapper mapper
) : IRequestHandler<LoginCommand, AuthResponseDto>
{
    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userRepository.GetByEmailAsync(request.Email, ct)
            ?? throw new UnauthorizedException("Invalid email or password.");

        if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedException("Invalid email or password.");

        var accessToken = jwtService.GenerateAccessToken(user.Id, user.Email.Value, ["User"]);
        var refreshToken = jwtService.GenerateRefreshToken();

        return new AuthResponseDto(accessToken, refreshToken, mapper.Map<UserProfileDto>(user));
    }
}
