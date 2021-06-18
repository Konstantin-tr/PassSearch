using Microsoft.AspNetCore.Mvc;
using PasswordQueryTool.Backend.Database;
using PasswordQueryTool.Model;
using System;
using System.Threading.Tasks;

namespace PasswordQueryTool.Backend.Services.DatabaseAccess.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class QueriesController : ControllerBase
    {
        private readonly IDatabaseHelper _databaseHelper;

        public QueriesController(IDatabaseHelper databaseHelper)
        {
            _databaseHelper = databaseHelper ?? throw new ArgumentNullException(nameof(databaseHelper));
        }

        /// <summary>
        /// Gets the most common passwords for a domain.
        /// </summary>
        /// <param name="domain">The domain to query for.</param>
        /// <returns>An HTTP Result.</returns>
        [HttpGet("common/domain/{domain}")]
        public async Task<IActionResult> QueryCommonByDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return BadRequest();
            }

            var result = await _databaseHelper.GetMostCommonByDomainAsync(domain);

            return Ok(result);
        }

        /// <summary>
        /// Gets the most common passwords for a top level domain.
        /// </summary>
        /// <param name="tld">The top level domain to query for.</param>
        /// <returns>An HTTP Result.</returns>
        [HttpGet("common/tld/{tld}")]
        public async Task<IActionResult> QueryCommonByTopLevelDomain(string tld)
        {
            if (string.IsNullOrWhiteSpace(tld))
            {
                return BadRequest();
            }

            var result = await _databaseHelper.GetMostCommonByTopLevelDomainAsync(tld);

            return Ok(result);
        }

        /// <summary>
        /// Gets the amount of entries in the db.
        /// </summary>
        /// <returns>An HTTP Result.</returns>
        [HttpGet("debug/amount")]
        public async Task<IActionResult> GetAmountOfEntries()
        {
            var result = await _databaseHelper.GetAmountOfEntries();

            return Ok(result);
        }

        /// <summary>
        /// Gets login data for a specific domain. Used for debugging.
        /// </summary>
        /// <param name="domain">The domain to query for.</param>
        /// <returns>An HTTP Result.</returns>
        [HttpGet("debug/domain/{domain}")]
        public async Task<IActionResult> QueryDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain))
            {
                return BadRequest();
            }

            var result = await _databaseHelper.GetDataByDomainAsync(new QueryRequest() { Filter = domain.Trim() });

            return Ok(result);
        }

        /// <summary>
        /// Gets all login data for a specific email address.
        /// </summary>
        /// <param name="email">The email to query for.</param>
        /// <returns>An HTTP Result.</returns>
        [HttpGet("email/{email}")]
        public async Task<IActionResult> QueryEmail(string email)
        {
            var emailStrings = email.Trim().Split("@");

            if (emailStrings.Length != 2)
            {
                return BadRequest();
            }

            var result = await _databaseHelper.GetDataByFullEmailAsync(emailStrings[0], emailStrings[1]);

            return Ok(new QueryResponse() { Data = result.ToArray() });
        }

        /// <summary>
        /// Gets all login data for a specific username.
        /// </summary>
        /// <param name="username">The username to query for.</param>
        /// <returns>An HTTP Result.</returns>
        [HttpGet("search/username/{username}")]
        public async Task<IActionResult> SearchByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest();
            }

            var result = await _databaseHelper.GetDataByUsernameAsync(username);

            return Ok(new QueryResponse() { Data = result.ToArray() });
        }

        /// <summary>
        /// Gets login data for a specific top level domain. Used for debugging.
        /// </summary>
        /// <param name="tld">The top level domain to query for.</param>
        /// <returns>An HTTP Result.</returns>
        [HttpGet("debug/topleveldomain/{tld}")]
        public async Task<IActionResult> QueryTopLevelDomain(string tld)
        {
            var result = await _databaseHelper.GetDataByTopLevelDomainAsync(tld.Trim());

            return Ok(result);
        }
    }
}