using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using MoviesApi.Services;
using System;
using System.Threading.Tasks;

namespace MoviesApi.Controllers
{
    [Route("api/security")]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IDataProtector _protector;
        private readonly HashService _hashService;

        public SecurityController(IDataProtectionProvider dataProtectionProvider, HashService hashService)
        {
            _protector = dataProtectionProvider.CreateProtector("value_secret_and_unique");
            _hashService = hashService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string plainText = "Dimitris Kosmas";
            string encryptedText = _protector.Protect(plainText);
            string decreptedText = _protector.Unprotect(encryptedText);

            return Ok(new { plainText, encryptedText, decreptedText });
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

        [HttpGet("hash")]
        public IActionResult Hash()
        {
            string plainText = "Dimitris Kosmas";
            var hashResult1 = _hashService.Hash(plainText);
            var hashResult2 = _hashService.Hash(plainText);

            return Ok(new { plainText, hashResult1, hashResult2 });
        }
    }
}
