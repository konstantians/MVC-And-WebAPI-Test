﻿namespace MVCAndWebAPIAuthAndAuthTest.API.RequestModels;

public class ApiSignInModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    public string? RememberMe { get; set; }
}