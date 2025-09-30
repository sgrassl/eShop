using System.ComponentModel.DataAnnotations;

namespace eShop.Basket.API.Model;

public class BasketItem : IValidatableObject
{
    public BasketItem()
    {
    }

    public string Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal OldUnitPrice { get; set; }
    public int Quantity { get; set; }
    public string PictureUrl { get; set; }

    /// <summary>
    /// Validates the basket item properties to ensure they meet business rules and constraints.
    /// </summary>
    /// <param name="validationContext">The validation context that provides additional information about the validation operation.</param>
    /// <returns>
    /// A collection of <see cref="ValidationResult"/> objects describing any validation errors found.
    /// Returns an empty collection if the basket item is valid.
    /// </returns>
    /// <remarks>
    /// This method validates the following rules:
    /// <list type="bullet">
    /// <item><description>Id must not be null or whitespace</description></item>
    /// <item><description>ProductId must be greater than 0</description></item>
    /// <item><description>ProductName must not be null or whitespace</description></item>
    /// <item><description>UnitPrice must not be negative</description></item>
    /// <item><description>OldUnitPrice must not be negative</description></item>
    /// <item><description>Quantity must be at least 1</description></item>
    /// <item><description>PictureUrl must not be null/whitespace and must be a valid absolute URL</description></item>
    /// </list>
    /// </remarks>
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

    /// <summary>
    /// Performs advanced validation of the basket item properties to ensure they meet extended business rules and constraints.
    /// This method focuses on advanced business logic validation beyond basic field validation.
    /// </summary>
    /// <param name="validationContext">The validation context that provides additional information about the validation operation.</param>
    /// <returns>
    /// A collection of <see cref="ValidationResult"/> objects describing any validation errors found.
    /// Returns an empty collection if the basket item is valid according to advanced rules.
    /// </returns>
    /// <remarks>
    /// This method validates the following advanced rules:
    /// <list type="bullet">
    /// <item><description>Quantity cannot exceed 100 items per product</description></item>
    /// <item><description>UnitPrice cannot exceed 10,000</description></item>
    /// <item><description>ProductName must be between 3 and 100 characters</description></item>
    /// <item><description>OldUnitPrice should be greater than or equal to UnitPrice when representing a discount</description></item>
    /// <item><description>Discount percentage cannot exceed 90%</description></item>
    /// <item><description>PictureUrl should use HTTPS for security</description></item>
    /// </list>
    /// </remarks>
    public IEnumerable<ValidationResult> ValidateAdvanced(ValidationContext validationContext)
    {
        var results = new List<ValidationResult>();

        // Advanced business rules validation
        if (Quantity > 100)
        {
            results.Add(new ValidationResult("Quantity cannot exceed 100 items per product", new[] { "Quantity" }));
        }

        if (UnitPrice > 10000m)
        {
            results.Add(new ValidationResult("UnitPrice cannot exceed 10000", new[] { "UnitPrice" }));
        }

        if (!string.IsNullOrWhiteSpace(ProductName))
        {
            if (ProductName.Length < 3)
            {
                results.Add(new ValidationResult("ProductName must be at least 3 characters long", new[] { "ProductName" }));
            }

            if (ProductName.Length > 100)
            {
                results.Add(new ValidationResult("ProductName cannot exceed 100 characters", new[] { "ProductName" }));
            }
        }

        // Discount logic validation
        if (OldUnitPrice > 0 && UnitPrice > 0 && OldUnitPrice < UnitPrice)
        {
            results.Add(new ValidationResult("OldUnitPrice should be greater than or equal to current UnitPrice when representing a discount", new[] { "OldUnitPrice" }));
        }

        // Check for excessive discount percentage (> 90%)
        if (OldUnitPrice > 0 && UnitPrice > 0)
        {
            var discountPercentage = (OldUnitPrice - UnitPrice) / OldUnitPrice * 100;
            if (discountPercentage > 90m)
            {
                results.Add(new ValidationResult("Discount percentage cannot exceed 90%", new[] { "OldUnitPrice" }));
            }
        }

        // Security validation for PictureUrl
        if (!string.IsNullOrWhiteSpace(PictureUrl) && Uri.TryCreate(PictureUrl, UriKind.Absolute, out var uri))
        {
            if (uri.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase))
            {
                results.Add(new ValidationResult("PictureUrl should use HTTPS for security", new[] { "PictureUrl" }));
            }
        }

        return results;
    }
}
