﻿using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.RequestModels;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.InternalModels.SqlModels;

internal class SqlEmailModel
{
    public string? Id { get; set; }
    public DateTime SentAt { get; set; }
    public string? Receiver { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }

    public SqlEmailModel()
    {
        
    }

    public SqlEmailModel(EmailRequestModel emailRequestModel)
    {
        Receiver = emailRequestModel.Receiver;
        Title = emailRequestModel.Title;
        Message = emailRequestModel.Message;
    }
}
