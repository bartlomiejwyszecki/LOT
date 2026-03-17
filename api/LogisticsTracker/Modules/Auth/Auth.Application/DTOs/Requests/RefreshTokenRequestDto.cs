namespace Auth.Application.DTOs.Requests;

public class RefreshTokenRequestDto
{
    public Guid UserId { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
}
