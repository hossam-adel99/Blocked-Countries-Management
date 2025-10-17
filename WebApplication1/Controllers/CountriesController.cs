using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/countries")]
    public class CountriesController : ControllerBase
    {
        private readonly IBlockedCountryService _blocked;
        public CountriesController(IBlockedCountryService blocked) => _blocked = blocked;

        [HttpPost("block")]
        public IActionResult Block([FromBody] BlockedCountry model)
        {
            if (string.IsNullOrWhiteSpace(model.CountryCode)) return BadRequest("CountryCode required");
            var code = model.CountryCode.ToUpperInvariant();
            var name = model.CountryName ?? code;

            if (!_blocked.TryBlockCountry(code, name))
                return Conflict("Country already blocked");

            return CreatedAtAction(nameof(GetBlocked), new { }, model);
        }

        [HttpDelete("block/{countryCode}")]
        public IActionResult Unblock(string countryCode)
        {
            if (!_blocked.TryRemoveBlockedCountry(countryCode))
                return NotFound();
            return NoContent();
        }

        [HttpGet("blocked")]
        public IActionResult GetBlocked([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string? q = null)
        {
            var items = _blocked.GetBlockedCountries();

            if (!string.IsNullOrWhiteSpace(q))
            {
                var uq = q.ToUpperInvariant();
                items = items.Where(i => i.CountryCode.ToUpperInvariant().Contains(uq) || i.CountryName.ToUpperInvariant().Contains(uq));
            }

            var total = items.Count();
            var paged = items.Skip((page - 1) * pageSize).Take(pageSize);

            return Ok(new { Total = total, Page = page, PageSize = pageSize, Items = paged });
        }

        [HttpPost("temporal-block")]
        public IActionResult TemporalBlock([FromBody] TemporalBlockRequest req)
        {
            if (req == null) return BadRequest();
            string? error;
            if (!_blocked.TryTemporalBlock(req.CountryCode, req.CountryName ?? req.CountryCode, req.DurationMinutes, out error))
                return Conflict(error);

            return Ok();
        }
    }

    public record TemporalBlockRequest(string CountryCode, string? CountryName, int DurationMinutes);
}
