namespace eShop.Basket.API.Model;

public class BasketItem : IValidatableObject
{
    public string Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        if (string.IsNullOrWhiteSpace(Id))
        {
            results.Add(new ValidationResult("Id is required", new[] { "Id" }));
        }

        if (ProductId <= 0)
        {
            results.Add(new ValidationResult("ProductId must be greater than 0", new[] { "ProductId" }));
        }

        if (string.IsNullOrWhiteSpace(ProductName))
        {
            results.Add(new ValidationResult("ProductName is required", new[] { "ProductName" }));
        }

        if (UnitPrice < 0)
        {
            results.Add(new ValidationResult("UnitPrice cannot be negative", new[] { "UnitPrice" }));
        }

        if (OldUnitPrice < 0)
        {
            results.Add(new ValidationResult("OldUnitPrice cannot be negative", new[] { "OldUnitPrice" }));
        }

        if (Quantity < 1)
        {
            results.Add(new ValidationResult("Invalid number of units", new[] { "Quantity" }));
        }

        if (string.IsNullOrWhiteSpace(PictureUrl))
        {
            results.Add(new ValidationResult("PictureUrl is required", new[] { "PictureUrl" }));
        }
        else if (!Uri.TryCreate(PictureUrl, UriKind.Absolute, out _))
        {
            results.Add(new ValidationResult("PictureUrl must be a valid URL", new[] { "PictureUrl" }));
        }

        return results;
    }
}
