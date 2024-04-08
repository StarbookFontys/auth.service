namespace authentication_service.Models
{
	public class HashInfo
	{
		public string email { get; set; }
		public string hashed { get; set; }
		public byte[] salt { get; set; }
	}
}
