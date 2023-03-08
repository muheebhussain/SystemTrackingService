public class AuditLog
{
    public int Id { get; set; }
    public string EntityType { get; set; }
    public string Action { get; set; }
    public string PreviousValues { get; set; }
    public string CurrentValues { get; set; }
    public DateTime Timestamp { get; set; }
}

public DbSet<AuditLog> AuditLogs { get; set; }

public override int SaveChanges()
{
    var auditLogs = new List<AuditLog>();

    foreach (var entry in ChangeTracker.Entries())
    {
        if (entry.State == EntityState.Added)
        {
            auditLogs.Add(new AuditLog
            {
                EntityType = entry.Entity.GetType().Name,
                Action = "Added",
                CurrentValues = JsonSerializer.Serialize(entry.Entity),
                Timestamp = DateTime.Now
            });
        }
        else if (entry.State == EntityState.Modified)
        {
            auditLogs.Add(new AuditLog
            {
                EntityType = entry.Entity.GetType().Name,
                Action = "Modified",
                PreviousValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject()),
                CurrentValues = JsonSerializer.Serialize(entry.CurrentValues.ToObject()),
                Timestamp = DateTime.Now
            });
        }
        else if (entry.State == EntityState.Deleted)
        {
            auditLogs.Add(new AuditLog
            {
                EntityType = entry.Entity.GetType().Name,
                Action = "Deleted",
                PreviousValues = JsonSerializer.Serialize(entry.OriginalValues.ToObject()),
                Timestamp = DateTime.Now
            });
        }
    }

    foreach (var auditLog in auditLogs)
    {
        AuditLogs.Add(auditLog);
    }

    return base.SaveChanges();
}
