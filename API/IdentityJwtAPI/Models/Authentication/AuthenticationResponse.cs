namespace JwtIdentityAPI.Models.Account
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string JwtToken { get; set; }
        public string JwtExpiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
