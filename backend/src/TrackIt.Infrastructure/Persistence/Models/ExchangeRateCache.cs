using System.ComponentModel.DataAnnotations.Schema;

namespace TrackIt.Infrastructure.Persistence.Models;

public class ExchangeRateCache
{
    public int Id { get; set; }
    public string BaseCurrency { get; set; } = default!;
    public string TargetCurrency { get; set; } = default!;

    [Column(TypeName = "decimal(18,6)")]
    public decimal Rate { get; set; }

    public DateTime FetchedAt { get; set; } = DateTime.UtcNow;
}
