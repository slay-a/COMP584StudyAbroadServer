using System.Text.Json.Serialization;

namespace COMP584StudyAbroadServer.Models;

public class StudyProgram
{
    public int Id { get; set; }

    public string Name { get; set; } = null!; // e.g. "M.S. Computer Science"

    // Bachelors / Masters / PhD etc.
    public string DegreeLevel { get; set; } = null!;

    public string Language { get; set; } = "English";

    public int DurationMonths { get; set; }

    public decimal TuitionPerYear { get; set; }

    public bool IsExchangeFriendly { get; set; }

    // Foreign key to University
    public int UniversityId { get; set; }

    // ✅ make this nullable so it's NOT required from the request body
    [JsonIgnore]
    public University? University { get; set; }
}
