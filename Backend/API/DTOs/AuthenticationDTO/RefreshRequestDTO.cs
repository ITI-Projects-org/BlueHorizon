namespace API.DTOs.AuthenticationDTO
{
    public class RefreshRequestDTO
    {
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
    }
}
