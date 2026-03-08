using System.ComponentModel.DataAnnotations;

namespace Models
{
    public class StructureTheCurrentRepositoryWithAddingSolutDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public string Labels { get; set; }

        [Required]
        public string AcceptanceCriteria { get; set; }
    }

    public class CreateStructureTheCurrentRepositoryWithAddingSolutRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Title { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; }

        [Required]
        public string Priority { get; set; }

        [Required]
        public string Labels { get; set; }

        [Required]
        public string AcceptanceCriteria { get; set; }
    }

    public class UpdateStructureTheCurrentRepositoryWithAddingSolutRequest
    {
        [Required]
        public int Id { get; set; }

        [StringLength(100, MinimumLength = 5)]
        public string Title { get; set; }

        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; }

        public string Priority { get; set; }

        public string Labels { get; set; }

        public string AcceptanceCriteria { get; set; }
    }
}