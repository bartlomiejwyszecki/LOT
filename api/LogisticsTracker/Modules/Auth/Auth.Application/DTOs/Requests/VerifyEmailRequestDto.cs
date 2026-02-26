namespace Auth.Application.DTOs.Requests;

public class VerifyEmailRequestDto
{
    public Guid UserId { get; set; }
    public string Code { get; set; } = string.Empty;
}