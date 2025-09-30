using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace eShop.Basket.API.Model
{
    [TestClass]
    public class BasketItemTest
    {
        private BasketItem CreateValidBasketItem()
        {
            return new BasketItem
            {
                Id = "1",
                ProductId = 10,
                ProductName = "Test Product",
                UnitPrice = 5.99m,
                OldUnitPrice = 8.99m,
                Quantity = 2,
                PictureUrl = "https://example.com/test.jpg"
            };
        }

        [TestMethod]
        public void Validate_WithValidItem_ReturnsNoValidationErrors()
        {
            // Arrange
            var item = CreateValidBasketItem();
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_WithNullId_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.Id = null;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Id is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("Id"));
        }

        [TestMethod]
        public void Validate_WithEmptyId_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.Id = "";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Id is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("Id"));
        }

        [TestMethod]
        public void Validate_WithWhitespaceId_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.Id = "   ";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Id is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("Id"));
        }

        [TestMethod]
        public void Validate_WithZeroProductId_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.ProductId = 0;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("ProductId must be greater than 0", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("ProductId"));
        }

        [TestMethod]
        public void Validate_WithNegativeProductId_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.ProductId = -1;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("ProductId must be greater than 0", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("ProductId"));
        }

        [TestMethod]
        public void Validate_WithNullProductName_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.ProductName = null;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("ProductName is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("ProductName"));
        }

        [TestMethod]
        public void Validate_WithEmptyProductName_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.ProductName = "";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("ProductName is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("ProductName"));
        }

        [TestMethod]
        public void Validate_WithWhitespaceProductName_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.ProductName = "   ";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("ProductName is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("ProductName"));
        }

        [TestMethod]
        public void Validate_WithNegativeUnitPrice_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.UnitPrice = -1.00m;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("UnitPrice cannot be negative", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("UnitPrice"));
        }

        [TestMethod]
        public void Validate_WithZeroUnitPrice_ReturnsNoValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.UnitPrice = 0.00m;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_WithNegativeOldUnitPrice_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.OldUnitPrice = -1.00m;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("OldUnitPrice cannot be negative", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("OldUnitPrice"));
        }

        [TestMethod]
        public void Validate_WithZeroOldUnitPrice_ReturnsNoValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.OldUnitPrice = 0.00m;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_WithZeroQuantity_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.Quantity = 0;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Invalid number of units", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("Quantity"));
        }

        [TestMethod]
        public void Validate_WithNegativeQuantity_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.Quantity = -1;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("Invalid number of units", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("Quantity"));
        }

        [TestMethod]
        public void Validate_WithNullPictureUrl_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.PictureUrl = null;
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("PictureUrl is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("PictureUrl"));
        }

        [TestMethod]
        public void Validate_WithEmptyPictureUrl_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.PictureUrl = "";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("PictureUrl is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("PictureUrl"));
        }

        [TestMethod]
        public void Validate_WithWhitespacePictureUrl_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.PictureUrl = "   ";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("PictureUrl is required", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("PictureUrl"));
        }

        [TestMethod]
        public void Validate_WithInvalidPictureUrl_ReturnsValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.PictureUrl = "not-a-valid-url";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(1, results.Count);
            Assert.AreEqual("PictureUrl must be a valid URL", results[0].ErrorMessage);
            Assert.IsTrue(results[0].MemberNames.Contains("PictureUrl"));
        }

        [TestMethod]
        public void Validate_WithRelativePictureUrl_ReturnsNoValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.PictureUrl = "/images/test.jpg";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert - Relative URLs are actually accepted by Uri.TryCreate with UriKind.Absolute
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_WithValidHttpPictureUrl_ReturnsNoValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.PictureUrl = "http://example.com/test.jpg";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_WithValidHttpsPictureUrl_ReturnsNoValidationError()
        {
            // Arrange
            var item = CreateValidBasketItem();
            item.PictureUrl = "https://example.com/test.jpg";
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void Validate_WithMultipleValidationErrors_ReturnsAllErrors()
        {
            // Arrange
            var item = new BasketItem
            {
                Id = "",
                ProductId = 0,
                ProductName = "",
                UnitPrice = -1.00m,
                OldUnitPrice = -2.00m,
                Quantity = 0,
                PictureUrl = ""
            };
            var context = new ValidationContext(item);

            // Act
            var results = new List<ValidationResult>(item.Validate(context));

            // Assert - 7 errors: 6 field errors + PictureUrl required
            Assert.AreEqual(7, results.Count);

            var errorMessages = results.Select(r => r.ErrorMessage).ToList();
            Assert.IsTrue(errorMessages.Contains("Id is required"));
            Assert.IsTrue(errorMessages.Contains("ProductId must be greater than 0"));
            Assert.IsTrue(errorMessages.Contains("ProductName is required"));
            Assert.IsTrue(errorMessages.Contains("UnitPrice cannot be negative"));
            Assert.IsTrue(errorMessages.Contains("OldUnitPrice cannot be negative"));
            Assert.IsTrue(errorMessages.Contains("Invalid number of units"));
            Assert.IsTrue(errorMessages.Contains("PictureUrl is required"));
        }
    }
}
