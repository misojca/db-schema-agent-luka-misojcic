namespace AdventureWorks.Domain.Purchasing.Entities;

/// <summary>
/// An employee's request to purchase a product.
/// Maps to purchasing.purchaserequest.
/// Added in migration AddPurchaseRequest.
/// </summary>
public class PurchaseRequest
{
    /// <summary>Primary key, serial (identity).</summary>
    public int PurchaseRequestId { get; private set; }

    /// <summary>FK → humanresources.employee.businessentityid.</summary>
    public int EmployeeId { get; private set; }

    /// <summary>FK → production.product.productid.</summary>
    public int ProductId { get; private set; }

    /// <summary>Number of units requested. NOT NULL.</summary>
    public short Quantity { get; private set; }

    /// <summary>Date and time the request was submitted. Defaults to now().</summary>
    public DateTime RequestDate { get; private set; }

    /// <summary>Current workflow status. Defaults to 'pending'. Max 20 characters.</summary>
    public string Status { get; private set; } = "pending";

    private PurchaseRequest() { }

    public static PurchaseRequest Create(
        int employeeId,
        int productId,
        short quantity,
        string status = "pending")
    {
        return new PurchaseRequest
        {
            EmployeeId = employeeId,
            ProductId = productId,
            Quantity = quantity,
            RequestDate = DateTime.UtcNow,
            Status = status
        };
    }
}
