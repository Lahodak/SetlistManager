using Microsoft.AspNetCore.Identity;
using SetlistManager.Common.Models;
using SetlistManager.Data.Entities;

namespace SetlistManager.Business.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IMailService _mailService;    

    public AuthService(UserManager<User> userManager, IMailService mailService)
    {
        _userManager = userManager;
        _mailService = mailService;
    }

    public async Task<RegisterResultModel> RegisterAsync(RegisterRequestModel model)
    {
        var existingUser = await _userManager.FindByEmailAsync(model.Email);

        if (existingUser != null)
        {
            return new RegisterResultModel
            {
                Success = false,
                Message = "User with this email already exists."
            };
        }

        var existingUserByName = await _userManager.FindByNameAsync(model.UserName);
        if (existingUserByName != null)
        {
            return new RegisterResultModel
            {
                Success = false,
                Message = "User with this username already exists."
            };
        }

        User user = new()
        {
            Email = model.Email,
            UserName = model.UserName,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (!result.Succeeded)
        {
            return new RegisterResultModel
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        var createdUser = await _userManager.FindByEmailAsync(user.Email);

        var confirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(createdUser!);
        await _mailService.SendVerificationEmailAsync(user.Email, confirmationToken);

        return new RegisterResultModel
        {
            Success = true,
            Message = "User registered successfully. Please check your email to verify your account."
        };
    }

    public async Task<bool> VerifyEmailAsync(VerifyModel verifyModel)
    {
        var user = await _userManager.FindByEmailAsync(verifyModel.Email);
        
        if (user == null)
        {
            return false;
        }
        
        var result = await _userManager.ConfirmEmailAsync(user, verifyModel.Token);
        
        return result.Succeeded;
    }

    public async Task<bool> TryResetPasswordAsync(ResetPasswordModel resetModel)
    {
        var user = await _userManager.FindByEmailAsync(resetModel.Email);

        if (user is null)
            return false;

        var result = await _userManager.ResetPasswordAsync(user, resetModel.Token, resetModel.NewPassword);

        return result.Succeeded;
    }

    public async Task<bool> RequestPasswordResetAsync(PasswordResetRequestModel resetRequestModel)
    {
        var user = await _userManager.FindByEmailAsync(resetRequestModel.Email);
        
        if (user is null)
            return false;
        
        var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

        await _mailService.SendPasswordResetEmailAsync(user.Email!, resetToken);
        
        return true;
    }
}