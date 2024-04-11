﻿namespace MVCAndWebAPIAuthAndAuthTest.EmailLibrary;

public class SmtpSettings
{
    public string? Host { get; set; }
    public int Port { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
    public bool EnableSsl { get; set; }

    public SmtpSettings() { }
    public SmtpSettings(string host, int port, string username, string password, bool enableSsl)
    {
        Host = host;
        Port = port;
        Username = username;
        Password = password;
        EnableSsl = enableSsl;
    }
}

