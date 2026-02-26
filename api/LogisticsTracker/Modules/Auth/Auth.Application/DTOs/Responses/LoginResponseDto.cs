namespace Auth.Application.DTOs.Responses;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public UserProfileDto User { get; set; } = new();
}