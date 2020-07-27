using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector _protector;

        public SecurityController(IDataProtectionProvider dataProtectionProvider)
        {
            _protector = dataProtectionProvider.CreateProtector("value_secret_and_unique");
        }

        [HttpGet]
        public IActionResult Get()
        {
            string plainText = "Dimitris Kosmas";
            string encryptedText = _protector.Protect(plainText);
            string decreptedText = _protector.Unprotect(encryptedText);

            return Ok(new {plainText, encryptedText, decreptedText });
        }

        /// <summary>
        /// In this method after the exparation of lifetime,then we cannot decrypt 
        /// </summary>
        /// <returns></returns>
        [HttpGet("timeBound")]
        public async Task<IActionResult> GetTimeBound()
        {
            var protectorTimeBound = _protector.ToTimeLimitedDataProtector();

            string plainText = "Dimitris Kosmas";
            string encryptedText = protectorTimeBound.Protect(plainText, lifetime: TimeSpan.FromSeconds(5));

            //For test purposes only
            //await Task.Delay(6000);

            string decreptedText = protectorTimeBound.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decreptedText });
        }
    }
}
