﻿namespace MVCAndWebAPIAuthAndAuthTest.API.Models.RequestModels;

public class ApiChangeEmailRequestModel
{
    public string? OldEmail { get; set; }
    public string? NewEmail { get; set; }
}
