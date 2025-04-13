// Common/Response.cs
using System.Collections.Generic;

namespace StudyApp.Common;

public class Response
{
    public List<Error> Errors { get; set; } = new List<Error>();
    public object Data { get; set; }
    public bool HasErrors => Errors.Count > 0;

    public void AddError(string field, string message)
    {
        Errors.Add(new Error { Field = field, Message = message });
    }
}

public class Error
{
    public string Field { get; set; }
    public string Message { get; set; }
}