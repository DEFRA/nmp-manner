using System.Text.Json.Serialization;

namespace Manner.Application.DTOs;

public class FieldDetail
{
    public FieldDetail()
    {
        FieldName = string.Empty;

    }
    public int? FieldID { get; set; }
    public string? FieldName { get; set; }
    /// <summary>
    /// Manner Crop Type ID
    /// </summary>
    [JsonPropertyName("MannerCropTypeID")]
    public int CropTypeID { get; set; }
    public int TopsoilID { get; set; }
    public int SubsoilId { get; set; }
    public bool IsInNVZ { get; set; }
}

