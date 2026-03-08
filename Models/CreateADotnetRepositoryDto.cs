csharp
using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class CreateADotnetRepositoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string RepositoryName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Url]
        public string RepositoryUrl { get; set; }

        [Required]
        [EmailAddress]
        public string OwnerEmail { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxContributors { get; set; }
    }

    public class UpdateDotnetRepositoryDto
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string RepositoryName { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Url]
        public string? RepositoryUrl { get; set; }

        [EmailAddress]
        public string? OwnerEmail { get; set; }

        [Range(1, int.MaxValue)]
        public int? MaxContributors { get; set; }
    }
}