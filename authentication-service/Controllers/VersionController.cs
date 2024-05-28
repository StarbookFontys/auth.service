using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace authentication_service.Controllers
{
	[Route("[controller]")]
	[ApiController]
	public class VersionController : ControllerBase
	{
		// GET: api/<VersionController>
		[HttpGet]
		public string Get()
		{
			string version = "v1.0.0";
			return version;
		}

		// GET api/<VersionController>/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/<VersionController>
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/<VersionController>/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/<VersionController>/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
