namespace SwimSchool.Domain.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }

    // UTC, set automatically on save
    public DateTime CreatedAt { get; set; }

    // UTC, set automatically on save
    public DateTime UpdatedAt { get; set; }
}
