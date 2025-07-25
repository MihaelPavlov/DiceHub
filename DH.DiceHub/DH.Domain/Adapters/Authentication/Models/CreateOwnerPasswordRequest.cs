﻿namespace DH.Domain.Adapters.Authentication.Models;

public class CreateOwnerPasswordRequest
{
    public string Email { get; set; } = string.Empty;
    public string ClubPhoneNumber { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
    public string ConfirmPassword { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}
