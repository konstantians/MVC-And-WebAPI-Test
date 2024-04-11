﻿using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.DtoModels;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.InternalModels.NoSqlModels;

public class NoSqlEmailModel
{
    public string? Id { get; set; }
    public DateTime SentAt { get; set; }
    public string? Receiver { get; set; }
    public string? Title { get; set; }
    public string? Message { get; set; }

    public NoSqlEmailModel()
    {

    }

    public NoSqlEmailModel(EmailRequestModel emailRequestModel)
    {
        Receiver = emailRequestModel.Receiver;
        Title = emailRequestModel.Title;
        Message = emailRequestModel.Message;
    }

}
