using System.ComponentModel.DataAnnotations;

namespace walletv2.Data.Entities;
public interface IBaseEntity
{
    [Key]
    Guid id { get; set; }
    DateTime createdAt { get; set; }
    Guid? createdById { get; set; }
    void Create();

    Guid? deletedById { get; set; }
    DateTime? deletedAt { get; set; }
    void MarkAsDeleted();
    void Delete();
    void Restore();

    Guid? updatedById { get; set; }
    DateTime updatedAt { get; set; }
    void Update();

    bool isDeleted { get; set; }
}

public class BaseEntityImplementation : IBaseEntity
{
    public Guid id { get; set; } = Guid.NewGuid();
    public DateTime createdAt { get; set; } = DateTime.UtcNow;
    public Guid? createdById { get; set; }
    public Guid? deletedById { get; set; }
    public DateTime? deletedAt { get; set; }
    public bool isDeleted { get; set; } = false;
    public Guid? updatedById { get; set; }
    public DateTime updatedAt { get; set; } = DateTime.UtcNow;
    public void Create() => createdAt = DateTime.UtcNow;
    public void MarkAsDeleted()
    {
        isDeleted = true;
        deletedAt = DateTime.UtcNow;
    }
    public void Delete() => MarkAsDeleted();
    public void Restore() => isDeleted = false;
    public void Update() => updatedAt = DateTime.UtcNow;
}
