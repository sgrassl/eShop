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
    /// Provides enhanced validation for basket items with improved performance, detailed error messages, and additional business rules.
    /// </summary>
    /// <param name="validationContext">
    /// The validation context that provides additional information about the validation operation.
    /// Can include custom validation parameters in the Items dictionary (e.g., "MaxItemValue" for configurable limits).
    /// </param>
    /// <returns>
    /// A lazy-evaluated collection of <see cref="ValidationResult"/> objects describing any validation errors found.
    /// Uses yield return for better memory efficiency when processing large collections.
    /// Returns an empty collection if the basket item is valid.
    /// </returns>
    /// <remarks>
    /// This enhanced validation method provides the following improvements over the standard <see cref="Validate"/> method:
    /// <list type="bullet">
    /// <item><description><strong>Performance:</strong> Uses yield return for lazy evaluation and better memory usage</description></item>
    /// <item><description><strong>Enhanced validation rules:</strong> Includes additional constraints like maximum values and length limits</description></item>
    /// <item><description><strong>Cross-field validation:</strong> Validates relationships between fields (e.g., price comparisons)</description></item>
    /// <item><description><strong>Business rules:</strong> Enforces total value limits and configurable constraints</description></item>
    /// <item><description><strong>Better error messages:</strong> Provides more descriptive and user-friendly validation messages</description></item>
    /// </list>
    ///
    /// <para><strong>Validation Rules Applied:</strong></para>
    /// <list type="number">
    /// <item><description>Id: Required, non-empty string</description></item>
    /// <item><description>ProductId: Must be positive integer</description></item>
    /// <item><description>ProductName: Required, non-empty, max 250 characters</description></item>
    /// <item><description>UnitPrice: Non-negative, max $999,999.99</description></item>
    /// <item><description>OldUnitPrice: Non-negative, max $999,999.99</description></item>
    /// <item><description>Quantity: Min 1, max 9,999 items</description></item>
    /// <item><description>PictureUrl: Required, valid absolute HTTP/HTTPS URL</description></item>
    /// <item><description>Price consistency: Warns if current price is significantly higher than old price</description></item>
    /// <item><description>Total value: Price × Quantity cannot exceed $999,999.99</description></item>
    /// <item><description>Configurable limits: Respects MaxItemValue from validation context if provided</description></item>
    /// </list>
    ///
    /// <para><strong>Usage with custom validation context:</strong></para>
    /// <code>
    /// var context = new ValidationContext(basketItem);
    /// context.Items["MaxItemValue"] = 50000m; // Set custom maximum item value
    /// var results = basketItem.ValidateEnhanced(context);
    /// </code>
    /// </remarks>
    public IEnumerable<ValidationResult> ValidateEnhanced(ValidationContext validationContext)
    {
        // Validate Id - required and should be a non-empty string
        if (string.IsNullOrWhiteSpace(Id))
        {
            yield return new ValidationResult(
                "Basket item identifier is required and cannot be empty.",
                new[] { nameof(Id) });
        }

        // Validate ProductId - must be a positive integer
        if (ProductId <= 0)
        {
            yield return new ValidationResult(
                "Product identifier must be a positive number greater than zero.",
                new[] { nameof(ProductId) });
        }

        // Validate ProductName - required with reasonable length constraints
        if (string.IsNullOrWhiteSpace(ProductName))
        {
            yield return new ValidationResult(
                "Product name is required and cannot be empty.",
                new[] { nameof(ProductName) });
        }
        else if (ProductName.Length > 250)
        {
            yield return new ValidationResult(
                "Product name cannot exceed 250 characters.",
                new[] { nameof(ProductName) });
        }

        // Validate UnitPrice - must be non-negative (free items allowed)
        if (UnitPrice < 0)
        {
            yield return new ValidationResult(
                "Unit price cannot be negative. Use zero for free items.",
                new[] { nameof(UnitPrice) });
        }
        else if (UnitPrice > 999999.99m)
        {
            yield return new ValidationResult(
                "Unit price cannot exceed $999,999.99.",
                new[] { nameof(UnitPrice) });
        }

        // Validate OldUnitPrice - must be non-negative
        if (OldUnitPrice < 0)
        {
            yield return new ValidationResult(
                "Previous unit price cannot be negative.",
                new[] { nameof(OldUnitPrice) });
        }
        else if (OldUnitPrice > 999999.99m)
        {
            yield return new ValidationResult(
                "Previous unit price cannot exceed $999,999.99.",
                new[] { nameof(OldUnitPrice) });
        }

        // Validate Quantity - must be positive with reasonable upper limit
        if (Quantity < 1)
        {
            yield return new ValidationResult(
                "Quantity must be at least 1. Remove the item if no longer needed.",
                new[] { nameof(Quantity) });
        }
        else if (Quantity > 9999)
        {
            yield return new ValidationResult(
                "Quantity cannot exceed 9,999 items per basket item.",
                new[] { nameof(Quantity) });
        }

        // Validate PictureUrl - required and must be a valid absolute URL
        if (string.IsNullOrWhiteSpace(PictureUrl))
        {
            yield return new ValidationResult(
                "Product picture URL is required.",
                new[] { nameof(PictureUrl) });
        }
        else if (!Uri.TryCreate(PictureUrl, UriKind.Absolute, out var uri))
        {
            yield return new ValidationResult(
                "Product picture URL must be a valid absolute URL.",
                new[] { nameof(PictureUrl) });
        }
        else if (uri.Scheme != "http" && uri.Scheme != "https")
        {
            yield return new ValidationResult(
                "Product picture URL must use HTTP or HTTPS protocol.",
                new[] { nameof(PictureUrl) });
        }

        // Cross-field validation: Price comparison logic
        if (UnitPrice > 0 && OldUnitPrice > 0 && UnitPrice > OldUnitPrice * 10)
        {
            yield return new ValidationResult(
                "Current price appears to be significantly higher than the previous price. Please verify pricing.",
                new[] { nameof(UnitPrice), nameof(OldUnitPrice) });
        }

        // Business rule: Total value validation
        var totalValue = UnitPrice * Quantity;
        if (totalValue > 999999.99m)
        {
            yield return new ValidationResult(
                "Total item value (price × quantity) cannot exceed $999,999.99.",
                new[] { nameof(UnitPrice), nameof(Quantity) });
        }

        // Additional business validation could be added here based on context
        // For example, checking against inventory, user limits, etc.
        if (validationContext?.Items?.ContainsKey("MaxItemValue") == true)
        {
            if (validationContext.Items["MaxItemValue"] is decimal maxValue && totalValue > maxValue)
            {
                yield return new ValidationResult(
                    $"Total item value cannot exceed the configured maximum of {maxValue:C}.",
                    new[] { nameof(UnitPrice), nameof(Quantity) });
            }
        }
    }
}
