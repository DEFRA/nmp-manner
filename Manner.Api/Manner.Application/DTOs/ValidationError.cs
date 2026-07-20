namespace Manner.Application.DTOs;

public class ValidationError
{
    public ValidationError()
    {
        Type= string.Empty;
        Title= string.Empty;
        Errors = new Dictionary<string, string[]>();
        TraceId= string.Empty;
    }
    public string Type { get; set; }
    public string Title { get; set; }
    public int Status { get; set; }    
    public Dictionary<string, string[]> Errors { get; set; }    
    public string TraceId { get; set; }
}
