using Microsoft.AspNetCore.Identity;
using SetlistManager.Common.Exceptions;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IMailService _mailService;
    private readonly IJwtService _jwtService;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public AuthService(UserManager<User> userManager, IMailService mailService, SignInManager<User> signInManager, IJwtService jwtService)
    {
        _userManager = userManager;
        _mailService = mailService;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    public async Task RegisterAsync(RegisterRequestModel model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);

        if (existingUser is not null)        
            throw new DuplicateEntryException();

        var existingUserByName = await _userManager.FindByNameAsync(model.UserName);

        if (existingUserByName is not null)
            throw new DuplicateEntryException();

        User user = new()
        {
            Email = model.Email,
            UserName = model.UserName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
            throw new InvalidOperationException();

        var createdUser = await _userManager.FindByEmailAsync(user.Email);

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(createdUser!);
        await _mailService.SendVerificationEmailAsync(user.Email, confirmationToken);
    }

    public async Task<LoginResultModel> LoginAsync(LoginRequestModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);

        if (user is null)        
            throw new UnauthorizedAccessException("User with provided email does not exist.");        

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
        
        if (result.IsLockedOut)        
            throw new UnauthorizedAccessException("User account is locked out.");
        
        else if (result.IsNotAllowed)        
            throw new UnauthorizedAccessException("User is not allowed to sign in.");
        
        else if (!result.Succeeded)        
            throw new UnauthorizedAccessException("Invalid login attempt.");
        
        var token = await _jwtService.GenerateTokenAsync(user);
        
        return new LoginResultModel
        {
            Token = token
        };
    }

    public async Task VerifyEmailAsync(VerifyModel verifyModel)
    {
        var user = await _userManager.FindByEmailAsync(verifyModel.Email);
        
        if (user is null)
            throw new EntryNotFoundException();

        var result = await _userManager.ConfirmEmailAsync(user, verifyModel.Token);

        if (!result.Succeeded)
            throw new InvalidOperationException();
    }

    public async Task TryResetPasswordAsync(ResetPasswordModel resetModel)
    {
        var user = await _userManager.FindByEmailAsync(resetModel.Email);

        if (user is null)
            throw new EntryNotFoundException();

        var result = await _userManager.ResetPasswordAsync(user, resetModel.Token, resetModel.NewPassword);
        
        if (!result.Succeeded)
            throw new InvalidOperationException();
    }

    public async Task RequestPasswordResetAsync(PasswordResetRequestModel resetRequestModel)
    {
        var user = await _userManager.FindByEmailAsync(resetRequestModel.Email);
        
        if (user is null)
            throw new EntryNotFoundException();
        
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _mailService.SendPasswordResetEmailAsync(user.Email!, resetToken);
    }
}