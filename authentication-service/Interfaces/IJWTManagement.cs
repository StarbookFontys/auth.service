namespace authentication_service.Interfaces
{
	public interface IJWTManagement
	{
		public string GenerateJWTToken(string email, string userlevel);
		public (string email, Boolean isAdmin) JWTDecoder(string jwtToken);
	}
}
