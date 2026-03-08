csharp
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CreateADotnetRepositoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string RepositoryName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Url]
        public string RepositoryUrl { get; set; } = string.Empty;

        [Required]
        public bool IsPrivate { get; set; }

        public CreateADotnetRepositoryDto() { }
    }

    public class UpdateDotnetRepositoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string RepositoryName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [Url]
        public string? RepositoryUrl { get; set; }

        public bool? IsPrivate { get; set; }

        public UpdateDotnetRepositoryDto() { }
    }
}