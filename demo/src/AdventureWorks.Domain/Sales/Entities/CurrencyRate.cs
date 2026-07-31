namespace AdventureWorks.Domain.Sales.Entities;

/// <summary>
/// Currency exchange rates.
/// Maps to sales.currencyrate.
/// </summary>
public class CurrencyRate
{
    public int CurrencyRateId { get; set; }
    public DateTime CurrencyRateDate { get; set; }
    public string FromCurrencyCode { get; set; } = string.Empty;
    public string ToCurrencyCode { get; set; } = string.Empty;

    /// <summary>
    /// Average exchange rate for the day.
    /// Issue #4: type being changed from real to numeric(19,4) via expand-contract.
    /// During Phase 1 this maps to averagerate (old); after Phase 2 maps to averagerate_new.
    /// </summary>
    public decimal AverageRate { get; set; }

    public decimal EndOfDayRate { get; set; }
    public DateTime ModifiedDate { get; set; }
}
