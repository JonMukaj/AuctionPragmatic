using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Repository.Contracts;
using Service.Contracts;
using Shared.DTO;
using Shared.Utility;
using Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Service;
public class UserService : IUserService
{
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IRepositoryManager _repositoryManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ICryptoUtils _cryptoUtils;
    public UserService(ILoggerManager logger, IMapper mapper, IRepositoryManager repositoryManager, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, ICryptoUtils cryptoUtils)
    {
        _logger = logger;
        _mapper = mapper;
        _repositoryManager = repositoryManager;
        _userManager = userManager;
        _signInManager = signInManager;
        _cryptoUtils = cryptoUtils;
    }

    public async Task<bool> CreateUser(RegisterUserDTO request)
    {
        var user = _mapper.Map<ApplicationUser>(request);
        user.DateCreated = DateTime.Now;

        _repositoryManager.UserRepository.CreateRecord(user);
        await _repositoryManager.SaveAsync();
        return true;
    }

    public async Task<bool> UpdateUser(int userId, UpdateUserDTO request)
    {
        var existingUser = await _repositoryManager.UserRepository.GetRecordById(userId);
        if (existingUser is null)
            throw new NotFoundException($"No user were found with id {userId}");

        var mapp = _mapper.Map(request, existingUser);
        mapp.PhoneNumber = request.Mobile;
        mapp.DateModified = DateTime.Now;

        _repositoryManager.UserRepository.UpdateRecord(mapp);
        await _repositoryManager.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteUser(int userId)
    {
        var existingUser = await _repositoryManager.UserRepository.GetRecordById(userId);
        if (existingUser is null)
            throw new NotFoundException($"No user were found with id {userId}");

        _repositoryManager.UserRepository.DeleteRecord(existingUser);
        await _repositoryManager.SaveAsync();
        return true;
    }

    public async Task<GetUserDTO> GetUserById(int userId)
    {
        var existingUser = await _repositoryManager.UserRepository.GetRecordById(userId);
        if (existingUser is null)
            throw new NotFoundException($"No user were found with id {userId}");

        return _mapper.Map<GetUserDTO>(existingUser);
    }

    public async Task<IEnumerable<GetUserDTO>> GetListOfUsers()
    {
        var list = await _repositoryManager.UserRepository.GetAllUsers();
        if (list is null) throw new NotFoundException("No lis of users");

        return _mapper.Map<IEnumerable<GetUserDTO>>(list);
    }

    public async Task<ClaimsIdentity> SignUpUserAsync(RegisterUserDTO signUp)
    {
        var user = _mapper.Map<ApplicationUser>(signUp);
        user.Email = signUp.Username;
        user.DateCreated = DateTime.Now;
        user.WalletBalance = 1000;


        var foundEmail = _userManager.Users.FirstOrDefault(x => x.Email == signUp.Username);
        if (foundEmail != null)
            throw new BadRequestException("EmailExists");


        if (!signUp.ConfirmPassword.Equals(signUp.Password))
            throw new BadRequestException("Password doesnt match");

        var tokenHash = _cryptoUtils.Encrypt($"{user.Id}{user.Email}{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}");

        IdentityResult result = null;

        result = await _userManager.CreateAsync(user, signUp.Password);

        if (!result.Succeeded)
        {
            var errorDetailsStr = string.Join("|", result.Errors.Select(x => x.Description));
            throw new BadRequestException(errorDetailsStr);
        }
        var claims = await GetClaims(user, tokenHash);
        await _userManager.AddToRoleAsync(user, "User");

        return claims;
    }

    public async Task<ClaimsIdentity> Login(LoginUserDTO userLogin)
    {
        ApplicationUser currentUser = null;

        currentUser = _userManager.Users.FirstOrDefault(u => u.Email == userLogin.Username || u.UserName == userLogin.Username);

        if (currentUser == null)
            throw new BadRequestException("WrongEmailPassword");

        var tokenHash = _cryptoUtils.Encrypt($"{currentUser.Id}{currentUser.Email}{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}");

        var claims = await GetClaims(currentUser, tokenHash);

        var validateUser = await _signInManager.PasswordSignInAsync(currentUser, userLogin.Password, false, lockoutOnFailure: true);

        if (!validateUser.Succeeded)
        {
            _logger.LogWarn(string.Format("AuthenticationFailed"));

            throw new BadRequestException("WrongEmailPassword");
        }

        return claims;

    }


    #region  private

    private async Task<ClaimsIdentity> GetClaims(ApplicationUser currentUser, string tokenHash)
    {
        var claims = new List<Claim>
             {
                new Claim("Id", currentUser.Id.ToString()),
                new Claim("Username", !string.IsNullOrWhiteSpace(currentUser.Email)?currentUser.Email : ""),
                new Claim("PhoneNumber", currentUser.PhoneNumber !=null?currentUser.PhoneNumber  :""),
                new Claim("FirstName", !string.IsNullOrWhiteSpace(currentUser.FirstName)?currentUser.FirstName : ""),
                new Claim("LastName", !string.IsNullOrWhiteSpace(currentUser.LastName)? currentUser.LastName : ""),
                new Claim("TokenHash", tokenHash),
                new Claim("WalletBalance",!string.IsNullOrWhiteSpace(currentUser.WalletBalance.ToString())? currentUser.WalletBalance.ToString() ?? "" : "" ),
             //   new Claim("Image", currentUser != null && !string.IsNullOrWhiteSpace(currentUser.Image) ? $"{_defaultConfig.APIUrl}{currentUser.Image}" : ""),
                 };

        await _userManager.AddClaimsAsync(currentUser, claims);

        var roles = await _userManager.GetRolesAsync(currentUser);
        if (roles is null)
            throw new NotFoundException(string.Format("NoRoles", currentUser.Id));

        return new ClaimsIdentity(claims, "MyCookieAuthenticationScheme");

    }

    #endregion

}

