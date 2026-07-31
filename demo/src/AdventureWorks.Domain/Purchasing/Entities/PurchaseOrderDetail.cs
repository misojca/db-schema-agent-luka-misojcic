namespace AdventureWorks.Domain.Purchasing.Entities;

/// <summary>
/// Individual products associated with a specific purchase order.
/// Maps to purchasing.purchaseorderdetail.
/// </summary>
public class PurchaseOrderDetail
{
    /// <summary>Primary key. FK to PurchaseOrderHeader.</summary>
    public int PurchaseOrderId { get; set; }

    /// <summary>Primary key. One line number per purchased product.</summary>
    public int PurchaseOrderDetailId { get; set; }

    /// <summary>Date the product is expected to arrive.</summary>
    public DateTime DueDate { get; set; }

    /// <summary>Quantity ordered.</summary>
    public short OrderQty { get; set; }

    /// <summary>FK to production.product.</summary>
    public int ProductId { get; set; }

    /// <summary>Vendor's selling price of a single product.</summary>
    public decimal UnitPrice { get; set; }

    /// <summary>Quantity actually received from the vendor.</summary>
    public decimal ReceivedQty { get; set; }

    /// <summary>Quantity rejected during inspection.</summary>
    public decimal RejectedQty { get; set; }

    /// <summary>Date and time the record was last updated.</summary>
    public DateTime ModifiedDate { get; set; }
}
