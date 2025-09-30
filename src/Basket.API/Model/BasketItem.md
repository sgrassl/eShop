# BasketItem Class Documentation

## Overview

The `BasketItem` class represents an individual item within a shopping basket in the eShop microservices application. It implements the `IValidatableObject` interface to provide comprehensive validation logic for ensuring data integrity and business rule compliance.

## Namespace

```csharp
namespace eShop.Basket.API.Model
```

## Class Declaration

```csharp
public class BasketItem : IValidatableObject
```

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | Unique identifier for the basket item |
| `ProductId` | `int` | Identifier of the product in the catalog |
| `ProductName` | `string` | Display name of the product |
| `UnitPrice` | `decimal` | Current price per unit of the product |
| `OldUnitPrice` | `decimal` | Previous price per unit (used for price comparison) |
| `Quantity` | `int` | Number of units of this product in the basket |
| `PictureUrl` | `string` | URL to the product's image |

## Validation Methods

### Standard Validation: `Validate(ValidationContext validationContext)`

The standard validation method implements basic business rules and data integrity checks.

#### Validation Rules

1. **Id Validation**
   - Must not be null or whitespace
   - Error: "Id is required"

2. **ProductId Validation**
   - Must be greater than 0
   - Error: "ProductId must be greater than 0"

3. **ProductName Validation**
   - Must not be null or whitespace
   - Error: "ProductName is required"

4. **UnitPrice Validation**
   - Must not be negative
   - Error: "UnitPrice cannot be negative"

5. **OldUnitPrice Validation**
   - Must not be negative
   - Error: "OldUnitPrice cannot be negative"

6. **Quantity Validation**
   - Must be at least 1
   - Error: "Invalid number of units"

7. **PictureUrl Validation**
   - Must not be null or whitespace
   - Must be a valid absolute URL
   - Errors: "PictureUrl is required" or "PictureUrl must be a valid URL"

#### Usage Example

```csharp
var basketItem = new BasketItem
{
    Id = "item-123",
    ProductId = 1,
    ProductName = "Sample Product",
    UnitPrice = 29.99m,
    OldUnitPrice = 34.99m,
    Quantity = 2,
    PictureUrl = "https://example.com/product.jpg"
};

var context = new ValidationContext(basketItem);
var results = basketItem.Validate(context);

if (results.Any())
{
    foreach (var error in results)
    {
        Console.WriteLine($"{string.Join(", ", error.MemberNames)}: {error.ErrorMessage}");
    }
}
```

### Enhanced Validation: `ValidateEnhanced(ValidationContext validationContext)`

The enhanced validation method provides improved performance, detailed error messages, and additional business rules.

#### Key Improvements

- **Performance**: Uses `yield return` for lazy evaluation and better memory efficiency
- **Enhanced Rules**: Additional constraints like maximum values and length limits
- **Cross-field Validation**: Validates relationships between fields
- **Business Rules**: Enforces total value limits and configurable constraints
- **Better Error Messages**: More descriptive and user-friendly validation messages

#### Enhanced Validation Rules

1. **Id Validation**
   - Required, non-empty string
   - Error: "Basket item identifier is required and cannot be empty."

2. **ProductId Validation**
   - Must be positive integer
   - Error: "Product identifier must be a positive number greater than zero."

3. **ProductName Validation**
   - Required, non-empty, maximum 250 characters
   - Errors: "Product name is required and cannot be empty." or "Product name cannot exceed 250 characters."

4. **UnitPrice Validation**
   - Non-negative, maximum $999,999.99
   - Errors: "Unit price cannot be negative. Use zero for free items." or "Unit price cannot exceed $999,999.99."

5. **OldUnitPrice Validation**
   - Non-negative, maximum $999,999.99
   - Errors: "Previous unit price cannot be negative." or "Previous unit price cannot exceed $999,999.99."

6. **Quantity Validation**
   - Minimum 1, maximum 9,999 items
   - Errors: "Quantity must be at least 1. Remove the item if no longer needed." or "Quantity cannot exceed 9,999 items per basket item."

7. **PictureUrl Validation**
   - Required, valid absolute HTTP/HTTPS URL
   - Errors: "Product picture URL is required.", "Product picture URL must be a valid absolute URL.", or "Product picture URL must use HTTP or HTTPS protocol."

8. **Cross-field Validation**
   - Price consistency check: Warns if current price is significantly higher than old price
   - Error: "Current price appears to be significantly higher than the previous price. Please verify pricing."

9. **Business Rules**
   - Total value (Price × Quantity) cannot exceed $999,999.99
   - Error: "Total item value (price × quantity) cannot exceed $999,999.99."

10. **Configurable Limits**
    - Respects `MaxItemValue` from validation context if provided
    - Error: "Total item value cannot exceed the configured maximum of {maxValue:C}."

#### Usage Example with Custom Validation Context

```csharp
var basketItem = new BasketItem
{
    Id = "item-123",
    ProductId = 1,
    ProductName = "Premium Product",
    UnitPrice = 149.99m,
    OldUnitPrice = 129.99m,
    Quantity = 3,
    PictureUrl = "https://example.com/premium-product.jpg"
};

var context = new ValidationContext(basketItem);
context.Items["MaxItemValue"] = 500m; // Set custom maximum item value

var results = basketItem.ValidateEnhanced(context);

foreach (var error in results)
{
    Console.WriteLine($"Field(s): {string.Join(", ", error.MemberNames)}");
    Console.WriteLine($"Error: {error.ErrorMessage}");
    Console.WriteLine();
}
```

## Architecture Context

### eShop Integration

The `BasketItem` class is part of the **Basket.API** microservice in the eShop application architecture:

- **Service**: Basket.API (Shopping basket using Redis)
- **Storage**: Redis for basket persistence
- **Communication**: Exposes gRPC services for inter-service communication
- **Events**: Participates in event-driven architecture through integration events

### Design Patterns

1. **Validation Pattern**: Implements `IValidatableObject` for self-validation
2. **Domain Model**: Represents a core business entity with encapsulated validation logic
3. **Microservices Pattern**: Part of the distributed basket service

### Dependencies

The class uses standard .NET validation components:
- `System.ComponentModel.DataAnnotations.ValidationContext`
- `System.ComponentModel.DataAnnotations.ValidationResult`
- `System.ComponentModel.DataAnnotations.IValidatableObject`

## Best Practices

### When to Use Each Validation Method

- **Use `Validate()`** for:
  - Standard validation scenarios
  - Simple business rules
  - Legacy system integration

- **Use `ValidateEnhanced()`** for:
  - Performance-critical scenarios with large collections
  - Complex business rule validation
  - Applications requiring detailed error messages
  - Systems with configurable validation constraints

### Performance Considerations

- The enhanced validation method uses `yield return` for lazy evaluation
- Validation is performed on-demand, improving memory usage
- Consider caching validation contexts for repeated validations

### Error Handling

Both validation methods return collections of `ValidationResult` objects:
- Check `results.Any()` to determine if validation passed
- Access `error.MemberNames` for affected property names
- Use `error.ErrorMessage` for user-friendly error descriptions

## Related Classes

- **CustomerBasket**: Contains collections of BasketItem objects
- **BasketController**: Handles HTTP requests for basket operations
- **BasketRepository**: Manages persistence of basket data in Redis

## Testing Considerations

When writing unit tests for BasketItem validation:

1. Test each validation rule independently
2. Test edge cases (null values, boundary conditions)
3. Test cross-field validation scenarios
4. Test custom validation context parameters
5. Verify performance characteristics of enhanced validation

```csharp
[Test]
public void Validate_WhenQuantityIsZero_ReturnsValidationError()
{
    // Arrange
    var basketItem = new BasketItem { Quantity = 0 };
    var context = new ValidationContext(basketItem);

    // Act
    var results = basketItem.Validate(context);

    // Assert
    Assert.That(results, Has.Some.Matches<ValidationResult>(
        r => r.MemberNames.Contains("Quantity") &&
             r.ErrorMessage.Contains("Invalid number of units")));
}
```

## Version History

- **Initial Version**: Basic validation implementation
- **Enhanced Version**: Added `ValidateEnhanced` method with improved performance and additional business rules

---

*This documentation is part of the eShop microservices application. For more information about the overall architecture, see the main eShop documentation.*