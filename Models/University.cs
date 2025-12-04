using System.Text.Json.Serialization;

namespace COMP584StudyAbroadServer.Models;

public class University
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Country { get; set; } = null!;

    public string City { get; set; } = null!;

    // e.g. Public, Private
    public string Type { get; set; } = "Public";

    public string? Website { get; set; }

    public string? Description { get; set; }

    [JsonIgnore]
    public ICollection<StudyProgram> Programs { get; set; } = new List<StudyProgram>();
}
