namespace Manner.Application.DTOs;

public class StandardResponse
{
    public StandardResponse() 
    {
        Success = false;
        Message = string.Empty;
        Errors = new List<string>();
    }
    public bool Success { get; set; }
    public string Message { get; set; } 
    public dynamic? Data { get; set; }
    public List<string> Errors { get; set; }
}
