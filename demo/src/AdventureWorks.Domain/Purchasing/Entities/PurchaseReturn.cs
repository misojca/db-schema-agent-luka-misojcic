namespace AdventureWorks.Domain.Purchasing.Entities;

/// <summary>
/// Tracks purchase returns sent back to vendors.
/// Maps to purchasing.purchasereturn.
/// Reference: issue #24 / Scenario 7.
/// </summary>
public class PurchaseReturn
{
    /// <summary>Primary key. Auto-generated serial.</summary>
    public int PurchaseReturnId { get; set; }

    /// <summary>FK to purchasing.purchaseorderdetail (purchaseorderid, purchaseorderdetailid).</summary>
    public int PurchaseOrderDetailId { get; set; }

    /// <summary>Date and time the return was recorded. Defaults to now().</summary>
    public DateTime ReturnDate { get; set; }

    /// <summary>Number of units returned. Must be positive.</summary>
    public short Quantity { get; set; }

    /// <summary>Optional reason for the return (max 200 characters).</summary>
    public string? Reason { get; set; }

    /// <summary>Date and time the record was last updated. Defaults to now().</summary>
    public DateTime ModifiedDate { get; set; }

    /// <summary>Navigation property to the originating purchase order detail line.</summary>
    public PurchaseOrderDetail? PurchaseOrderDetail { get; set; }
}
