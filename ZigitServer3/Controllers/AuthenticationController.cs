using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZigitServer3.Models;
using ZigitServer3.Services;

namespace ZigitServer3.Controllers
{
    
    [ApiController]
    
    public class AuthenticationController : ControllerBase
    {
        private DBHandler _dbHandler { get; set; }
        private DataValidator _dataValidator { get; set; }
        private IConfiguration _config { get; set; }
        
        public AuthenticationController(DBHandler handler, DataValidator dataValidator, IConfiguration config)
        {
            this._dbHandler = handler;
            this._dataValidator = dataValidator;
            this._config = config;
        }
        [HttpPost("Authenticate")]
        public ActionResult Authenticate([FromBody] LoginCradentials Input)
        {
            bool isValid = _dataValidator.ValidateEmail(Input.Email) && _dataValidator.VaildatePassword(Input.Password);
            if (!isValid)
            {
                return BadRequest();
            }
            Dictionary<string, string> dataFromDB = _dbHandler.CheckIfUserExistsAndReturnValue(Input.Email, Input.Password);
            if (dataFromDB == null)
            {
                return BadRequest();
            }
            if (dataFromDB.Count == 0)
            {
                return BadRequest();
            }
            return Ok(new AuthenticationResult(dataFromDB, _config));
        }
        [HttpGet("info")]
        //[Authorize]
        public async Task<ActionResult> Info()
        {

            string accessToken = HttpContext.Request.Headers["authentication"];
            if (accessToken is null)
            {
                return Unauthorized();
            }
            if (!accessToken.StartsWith("Bearer "))
            {
                return Unauthorized();
            }
            accessToken = accessToken.Substring("Bearer ".Length);
            var result = _dbHandler.GetUserProjectsByToken(accessToken);
            if (result == null)
                return Unauthorized();
            if (result.Count == 0)
                return Unauthorized();
            return Ok(result);
        }
    }
}