using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using telegram_bot.Models.Register.Response;
using RegisterRequest = telegram_bot.Models.Register.Request.RegisterRequest;
using telegram_bot.Data.Enum;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using telegram_bot.Data;
using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using telegram_bot.Servises;

namespace telegram_bot.Controllers
{

    [Route("api/api/v1.0/authen")]
    [ApiController]
    public class AuthController : ControllerBase

    {
        private readonly Context _dbContext;
        private readonly IConfiguration _configuration;
        private readonly TelegramBotService _telegramService;
        private readonly string _chatId = "@ghazale_test_bot";

        public AuthController(Context dbContext, IConfiguration configuration, TelegramBotService telegramService)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _telegramService = telegramService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            
            var userStatus = CheckUserStatus(request.MobileNumber);

            // Handle different user statuses
            switch (userStatus)
            {
                case TokenStatus.NotRegisteredUser:
                    await _telegramService.SendMessage(_chatId, "User not registered");
                    return Ok(new RegisterResponse { Token = TokenStatus.NotRegisteredUser.ToString() });

                case TokenStatus.ActivationNeeded:
                    SendActivationCode(request.MobileNumber); 
                    await _telegramService.SendMessage(_chatId, "Activation code sent");
                    return Ok(new RegisterResponse { Token = TokenStatus.ActivationNeeded.ToString() });

                case TokenStatus.NotExists:
                    await _telegramService.SendMessage(_chatId, "User does not exist");
                    return BadRequest(new { message = "User does not exist" });

                case TokenStatus.NotRegisteredUserDevice:
                    return BadRequest(new { message = "Device is not registered for this user" });

                case TokenStatus.CodeExpired:
                    return BadRequest(new { message = "Activation code has expired, please request a new one" });

                case TokenStatus.ValidToken:
                    // Generate a JWT token
                    var jwtToken = GenerateJwtToken(request.MobileNumber);
                    await _telegramService.SendMessage(_chatId, "Registration successful");
                    return Ok(new RegisterResponse { Token = jwtToken });

                default:
                    await _telegramService.SendMessage(_chatId, "Unexpected error occurred");
                    return StatusCode(500, "Unexpected status");
            }
        }

        private TokenStatus CheckUserStatus(string mobileNumber)
        {
          
            return TokenStatus.ValidToken; 
        }

        
        private void SendActivationCode(string mobileNumber)
        {
            
            Console.WriteLine($"Activation code sent to {mobileNumber}");
        }



        private string GenerateJwtToken(string mobileNumber)
        {

            var issuer = _configuration["Jwt:Issuer"];
            var key = _configuration["Jwt:Key"];


            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, mobileNumber),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };


            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );


            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] Update update)
        {
            if (update == null || update.Message == null || update.Message.Text == null)
            {
                return BadRequest();
            }

            var chatId = update.Message.Chat.Id.ToString();
            var messageText = update.Message.Text;

            await _telegramService.SendMessage(chatId, $"You said: {messageText}");

            return Ok();

        }
    }
}



    //[HttpPost("register")]
    //public IActionResult Register([FromBody] RegisterRequest request)
    //{
    //    if (string.IsNullOrEmpty(request.MobileNumber))
    //        return BadRequest("MobileNumber is required");

//    string token = "ActivationNeeded"; 
//    return Ok(new RegisterResponse { Token = token });
//}

//[HttpPost("register")]
//public IActionResult Register(RegisterRequest request)
//{

//    var userStatus = CheckUserStatus(request.MobileNumber);

//    switch (userStatus)
//    {
//        case TokenStatus.NotRegisteredUser:
//            return Ok(new TokenResponse { Token = TokenStatus.NotRegisteredUser.ToString() });

//        case TokenStatus.ActivationNeeded:
//            SendActivationCode(request.MobileNumber); 
//            return Ok(new TokenResponse { Token = TokenStatus.ActivationNeeded.ToString() });

//        case TokenStatus.NotExists:
//            return BadRequest(new { message = "User does not exist." });

//        default:
//            return Ok(new TokenResponse { Token = TokenStatus.ValidToken.ToString() });
//    }
//}

//public AuthController(IConfiguration configuration)
//{
//    _configuration = configuration;
//}

// Validate the request
//if (string.IsNullOrEmpty(request.MobileNumber))
//{
//    return BadRequest(new { message = "MobileNumber is required" });
//}



