using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MVCAndWebAPIAuthAndAuthTest.EmailLibraryRestAPI.Tests.UnitTests.HelperMethods;

internal class JsonHelperMethods
{
    public static string GetStringValueFromJsonObject(object result)
    {
        var derivedOkObjectResult = result as OkObjectResult;
        var json = JsonConvert.SerializeObject(derivedOkObjectResult!.Value);
        var anonymousObject = JsonConvert.DeserializeAnonymousType(json, new { WarningMessage = "" });
        return anonymousObject!.WarningMessage;
    }
}
