using System.Text.RegularExpressions;
using TaskManagementTool.Application.Common.Models;
using TaskManagementTool.Application.DTOs;
using TaskManagementTool.Application.Interfaces;

namespace TaskManagementTool.Application.Services
{
    public class AccountService : IAccountService
{
    private readonly IIdentityRepository _identityRepository;

    public AccountService(IIdentityRepository identityRepository)
    {
        _identityRepository = identityRepository;
    }

    public async Task<RegistrationResult> RegisterAsync(RegisterDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.Email))
            errors.Add("Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            errors.Add("Password is required.");

        if (dto.Password != dto.ConfirmPassword)
            errors.Add("Passwords do not match.");

        if (errors.Any())
        {
            return new RegistrationResult
            {
                Succeeded = false,
                Errors = errors
            };
        }

        // if (await _identityRepository.FindByEmailAsync(dto.Email) != null)
        // {
        //     return new RegistrationResult
        //     {
        //         Succeeded = false,
        //         Errors = new[] { "Email is already registered." }
        //     };
        // }

        return await _identityRepository.CreateUserAsync(dto);
    }
}
}