using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace COMP584StudyAbroadServer.Models;

public class University
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = null!;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = null!;

    // e.g. Public, Private
    [MaxLength(50)]
    public string Type { get; set; } = "Public";

    [Url]
    [MaxLength(300)]
    public string? Website { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [JsonIgnore]
    public ICollection<StudyProgram> Programs { get; set; } = new List<StudyProgram>();
}
