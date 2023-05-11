using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Nodes;
using Auth.Entities.DataTransferObjects;
using Auth.Features;
using Auth.Services.EmailService;
using AutoMapper;
using DAL.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly JwtHandler _jwtHandler;
        private readonly IEmailSender _emailSender;

        public AccountsController(UserManager<User> userManager, IMapper mapper, JwtHandler jwtHandler, IEmailSender emailSender)
        {
            _userManager = userManager;
            _mapper = mapper;
            _jwtHandler = jwtHandler;
            _emailSender = emailSender;
        }

        [HttpPost("registration")]
        public async Task<IActionResult> Register([FromBody] UserToRegisterDto userToRegisterDto)
        {
            var user = _mapper.Map<User>(userToRegisterDto);
            var result = await _userManager.CreateAsync(user, userToRegisterDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = errors,
                    Data = null
                });
            }

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserToLoginDto userToLoginDto)
        {
            var user = await _userManager.FindByEmailAsync(userToLoginDto.Email);

            if (user == null || !await _userManager.CheckPasswordAsync(user, userToLoginDto.Password))
            {
                return Unauthorized(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid authentication" },
                    Data = null
                });
            }

            //var signingCredentials = _jwtHandler.GetSigningCredentials();
            //var claims = _jwtHandler.GetClaims(user);
            //var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            //var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var token = await _jwtHandler.GenerateToken(user);

            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Errors = null,
                Data = token
            });
        }

        [HttpGet("accountinfo/{userName}")]
        [Authorize]
        public async Task<IActionResult> AccountInfo(string userName)
        {
            var user = await _userManager.FindByEmailAsync(userName);

            if (user == null)
            {
                return Unauthorized(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid authentication" },
                    Data = null
                });
            }

            var jsonObject = new JsonObject();
            jsonObject["Nickname"] = user.Nickname;
            jsonObject["Email"] = user.Email;

            return Ok(new AccountInfoResponseDto
            {
                IsSuccess = true,
                Errors = null,
                Data = jsonObject
            });
        }

        [HttpPost("changesettings")]
        [Authorize]
        public async Task<IActionResult> ChangeSettings([FromBody] ChangeSettingsDto changeSettingsDto)
        {
            var user = await _userManager.FindByEmailAsync(changeSettingsDto.Email);

            if (user == null)
            {
                return Unauthorized(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid authentication" },
                    Data = null
                });
            }

            IdentityResult result;

            if (!changeSettingsDto.OldPassword.IsNullOrEmpty() && !changeSettingsDto.Password.IsNullOrEmpty() &&
                !changeSettingsDto.ConfirmPassword.IsNullOrEmpty())
            {
                if (!await _userManager.CheckPasswordAsync(user, changeSettingsDto.OldPassword))
                {
                    return BadRequest(new ResponseDto
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Invalid password" },
                        Data = null
                    });
                }

                result = await _userManager.ChangePasswordAsync(user, changeSettingsDto.OldPassword, changeSettingsDto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new ResponseDto
                    {
                        IsSuccess = false,
                        Errors = errors,
                        Data = null
                    });
                }
            }

            user.Nickname = changeSettingsDto.Nickname;
            result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = errors,
                    Data = null
                });
            }

            //var signingCredentials = _jwtHandler.GetSigningCredentials();
            //var claims = _jwtHandler.GetClaims(user);
            //var tokenOptions = _jwtHandler.GenerateTokenOptions(signingCredentials, claims);
            //var token = new JwtSecurityTokenHandler().WriteToken(tokenOptions);

            var token = await _jwtHandler.GenerateToken(user);

            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Errors = null,
                Data = token
            });
        }

        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
            {
                return Unauthorized(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid authentication" },
                    Data = null
                });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var param = new Dictionary<string, string?>
            {
                { "token", token },
                { "email", forgotPasswordDto.Email }
            };

            var callback = QueryHelpers.AddQueryString(forgotPasswordDto.ClientURI, param);

            var message = new Message(new string[] { user.Email }, "Reset password token", callback, null);
            await _emailSender.SendEmailAsync(message);

            return Ok();
        }

        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);

            if (user == null)
            {
                return Unauthorized(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid authentication" },
                    Data = null
                });
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = errors,
                    Data = null
                });
            }

            return Ok();
        }

        [HttpPost("externallogin")]
        public async Task<IActionResult> ExternalLogin([FromBody] ExternalAuthDto externalAuthDto)
        {
            UserLoginInfo? info = null;
            var email = String.Empty;

            if (externalAuthDto.Provider == "FACEBOOK")
            {
                var facebookAccountDto = await _jwtHandler.VerifyFacebookToken(externalAuthDto);

                if (facebookAccountDto == null)
                    return BadRequest(new ResponseDto
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Invalid External Authentication." },
                        Data = null
                    });

                info = new UserLoginInfo(externalAuthDto.Provider, facebookAccountDto.Id, externalAuthDto.Provider);
                email = facebookAccountDto.Email;

            }

            if (externalAuthDto.Provider == "GOOGLE")
            {
                var payload = await _jwtHandler.VerifyGoogleToken(externalAuthDto);
                if (payload == null)
                {
                    return BadRequest(new ResponseDto
                    {
                        IsSuccess = false,
                        Errors = new List<string> { "Invalid External Authentication." },
                        Data = null
                    });
                }

                info = new UserLoginInfo(externalAuthDto.Provider, payload.Subject, externalAuthDto.Provider);
                email = payload.Email;
            }

            if (info == null && email == String.Empty)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid External Authentication." },
                    Data = null
                });
            }

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    user = new User { Email = email, UserName = email, Nickname = email };
                    await _userManager.CreateAsync(user);
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            if (user == null)
            {
                return BadRequest(new ResponseDto
                {
                    IsSuccess = false,
                    Errors = new List<string> { "Invalid External Authentication." },
                    Data = null
                });
            }

            var token = await _jwtHandler.GenerateToken(user);

            return Ok(new ResponseDto
            {
                IsSuccess = true,
                Errors = null,
                Data = token
            });
        }

        [HttpGet("getall")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users
                .Select(x => new { Id = x.Id, Email = x.Email, Nickname = x.Nickname })
                .ToListAsync();

            return Ok(new
            {
                IsSuccess = true,
                Errors = "",
                Data = users
            });
        }
    }
}
