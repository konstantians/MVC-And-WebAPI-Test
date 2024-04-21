using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MVCAndWebAPIAuthAndAuthTest.DataLibraryRestAPI.Tests.UnitTests.HelperMethods;

internal class JsonHelperMethods
{
    public static string GetStringValueFromJsonObject(object result)
    {
        var objectResult = result as BadRequestObjectResult;
        var json = JsonConvert.SerializeObject(objectResult!.Value);
        var anonymousObject = JsonConvert.DeserializeAnonymousType(json, new { ErrorMessage = "" });
        return anonymousObject!.ErrorMessage;
    }
}
