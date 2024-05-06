using System.ComponentModel.DataAnnotations;

namespace authentication_service.Models
{
	public class JWTReturn
	{
		[Required]
		public Boolean VerifiyInformation { get; set; }
		public string JWTToken { get; set; }
	}
}
