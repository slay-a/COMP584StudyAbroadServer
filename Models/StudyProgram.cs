using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace COMP584StudyAbroadServer.Models;

public class StudyProgram
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = null!;

    [Required]
    [MaxLength(50)]
    public string DegreeLevel { get; set; } = null!; // e.g. Bachelor, Master

    [Required]
    [MaxLength(50)]
    public string Language { get; set; } = null!;    // e.g. English, German

    [Range(1, 120, ErrorMessage = "Duration must be between 1 and 120 months.")]
    public int DurationMonths { get; set; }

    [Range(0, 1000000)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TuitionPerYear { get; set; }

    public bool IsExchangeFriendly { get; set; }

    [Required]
    public int UniversityId { get; set; }

    public University? University { get; set; }
}
