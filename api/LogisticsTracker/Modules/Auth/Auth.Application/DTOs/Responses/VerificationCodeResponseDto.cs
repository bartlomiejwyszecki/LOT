namespace Auth.Application.DTOs.Responses;

public class VerificationCodeResponseDto
{
    public Guid UserId { get; set; }
    public string Message { get; set; } = string.Empty;
}