using Data.Entities;
using Data.Repositories;
using Domain.Dtos.UserDtos;
using Domain.Extentions;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Domain.Services;

public class UserService
{
    private readonly IUserRepository _userRepository;
    private readonly UserManager<UserEntity> _userManager;
    private readonly PostalCodeRepository _postalCodeRepository;

    public UserService(IUserRepository userRepository, UserManager<UserEntity> userManager, PostalCodeRepository postalCodeRepository)
    {
        _userRepository = userRepository;
        _userManager = userManager;
        _postalCodeRepository = postalCodeRepository;
    }

    public async Task<RepositoryResult<UserModel>> GetByIdAsync(string userId)
    {
        return await _userRepository.GetAsync(u => u.Id == userId);
    }

    public async Task<RepositoryResult> RegisterUserAsync(UserRegistrationFormData formData)
    {
        var existingUser = await _userManager.FindByEmailAsync(formData.Email);
        if (existingUser != null)
            return new RepositoryResult { Succeeded = false, StatusCode = 409, Error = "Användaren finns redan" };

        var postalCodeExists = await _postalCodeRepository.ExistsAsync(pc => pc.PostalCode == formData.PostalCode);
        if (!postalCodeExists.Succeeded || postalCodeExists.StatusCode == 404)
        {
            var postalCodeEntity = new PostalCodeEntity
            {
                PostalCode = formData.PostalCode,
                City = formData.City,
                Country = formData.Country
            };

            var createPostalCodeResult = await _postalCodeRepository.AddAsync(postalCodeEntity);
            if (!createPostalCodeResult.Succeeded)
                return createPostalCodeResult;
        }

        var userEntity = new UserEntity
        {
            UserName = formData.Email,
            Email = formData.Email,
            Profile = new UserProfileEntity
            {
                FirstName = formData.FirstName,
                LastName = formData.LastName,
                PhoneNumber = formData.PhoneNumber
            },
            Address = new UserAddressEntity
            {
                StreetName = formData.StreetName,
                StreetNumber = formData.StreetNumber,
                PostalCodeId = formData.PostalCode
            }
        };

        return await _userRepository.AddAsync(userEntity, formData.Password);
    }

    public async Task<RepositoryResult> UpdateProfileAsync(string userId, UserProfileUpdateFormData formData)
    {
        var result = await _userRepository.GetAsync(u => u.Id == userId);
        if (!result.Succeeded) return result;

        var user = result.Result!;
        if (user.Profile == null) user.Profile = new UserProfileModel();

        user.Profile.FirstName = formData.FirstName;
        user.Profile.LastName = formData.LastName;
        user.Profile.PhoneNumber = formData.PhoneNumber;
        if (!string.IsNullOrEmpty(formData.Image))
            user.Profile.Image = formData.Image;

        return await _userRepository.UpdateAsync(user.MapTo<UserEntity>());
    }

    public async Task<RepositoryResult> UpdateAddressAsync(string userId, UserAddressUpdateFormData formData)
    {
        var userResult = await _userRepository.GetAsync(u => u.Id == userId);
        if (!userResult.Succeeded) return userResult;

        var user = userResult.Result!;
        var postalCodeExists = await _postalCodeRepository.ExistsAsync(pc => pc.PostalCode == formData.PostalCode);
        if (!postalCodeExists.Succeeded || postalCodeExists.StatusCode == 404)
        {
            var postalCodeEntity = new PostalCodeEntity
            {
                PostalCode = formData.PostalCode,
                City = formData.City,
                Country = formData.Country
            };

            var createPostalCodeResult = await _postalCodeRepository.AddAsync(postalCodeEntity);
            if (!createPostalCodeResult.Succeeded) return createPostalCodeResult;
        }

        if (user.Address == null) user.Address = new UserAddressModel();

        user.Address.StreetName = formData.StreetName;
        user.Address.StreetNumber = formData.StreetNumber;

        return await _userRepository.UpdateAsync(user.MapTo<UserEntity>());
    }

    public async Task<RepositoryResult> DeleteUserAsync(string userId)
    {
        var result = await _userRepository.GetAsync(u => u.Id == userId);
        if (!result.Succeeded) return result;

        var user = result.Result!;
        return await _userRepository.DeleteAsync(user.MapTo<UserEntity>());
    }
}
