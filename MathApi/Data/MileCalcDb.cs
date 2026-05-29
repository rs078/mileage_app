using Microsoft.EntityFrameworkCore;

namespace MathApi.Data;

// This is the "notebook cover" — open it to read/write the database.
// EF Core uses it to know which tables exist and how to talk to SQLite.
public sealed class MileCalcDb(DbContextOptions<MileCalcDb> options) : DbContext(options)
{
    public DbSet<HistoryEntry> History => Set<HistoryEntry>();
}

// One row in the History table — one saved calculation.
public sealed class HistoryEntry
{
    public int      Id        { get; set; }           // auto-incremented primary key
    public string   Mode      { get; set; } = "";     // "mpg", "tripcost", etc.
    public string   Summary   { get; set; } = "";     // human-readable result line
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
