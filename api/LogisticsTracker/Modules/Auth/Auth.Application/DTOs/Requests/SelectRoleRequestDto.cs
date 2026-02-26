namespace Auth.Application.DTOs.Requests;

public class SelectRoleRequestDto
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = string.Empty;
}