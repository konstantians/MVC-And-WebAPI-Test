﻿using Microsoft.AspNetCore.Mvc;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.DataAccessLogic;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.DtoModels;
using MVCAndWebAPIAuthAndAuthTest.EmailLibrary.Models.ResponseModels;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmailsController : ControllerBase
{
    private readonly IEmailDataAccess _emailDataAccess;
    private readonly IEmailService _emailService;

    public EmailsController(IEmailDataAccess emailDataAccess, IEmailService emailService)
    {
        _emailDataAccess = emailDataAccess;
        _emailService = emailService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetEmailEntry(string id)
    {
        try
        {
            EmailResponseModel? emailResponseModel = await _emailDataAccess.GetEmailEntryAsync(id);
            if(emailResponseModel is null)
                return NotFound();

            return Ok(emailResponseModel);
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetEmailEntries()
    {
        try
        {
            IEnumerable<EmailResponseModel> result = await _emailDataAccess.GetEmailEntriesAsync();

            return Ok(result.ToList());
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpPost]
    public async Task<IActionResult> SendEmailAndSaveEmailEntry([FromBody] EmailRequestModel emailRequestModel)
    {
        try
        {
            var emailSentResult = await _emailService.SendEmailAsync(emailRequestModel.Receiver!, emailRequestModel.Title!, emailRequestModel.Message!);
            if (!emailSentResult)
                return BadRequest();

            string? result = await _emailDataAccess.SaveEmailEntryAsync(emailRequestModel);
            if (result is null)
            {

                return Ok(new {WarningMessage = "DatabaseEntryCreationFailure"});
            }

            return Ok(new {WarningMessage = "None"});
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmailEntry(string id)
    {
        try
        {
            bool result = await _emailDataAccess.DeleteEmailEntryAsync(id);
            if (!result)
                return NotFound(new { ErrorMessage = "FailedEmailEntryDeletion" });

            return NoContent();
        }
        catch
        {
            return StatusCode(500, "Internal Server Error");
        }
    }
}
