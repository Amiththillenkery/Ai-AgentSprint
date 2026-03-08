namespace CreateADotnetRepositoryWithCleanArchitecture.Application.DTOs
{
    public record BaseDto
    {
        public Guid Id { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}