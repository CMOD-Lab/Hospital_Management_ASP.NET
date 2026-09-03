using System;
using Xunit;
using CareTrack.Domain.Exceptions;

namespace CareTrack.Domain.Exceptions.Tests
{
    public class DomainExceptionsTests
    {
        // NotFoundException Tests
        [Fact]
        public void NotFoundException_WithMessage_SetsMessage()
        {
            // Arrange & Act
            var ex = new NotFoundException("Entity not found");

            // Assert
            Assert.Equal("Entity not found", ex.Message);
        }

        [Fact]
        public void NotFoundException_WithEntityNameAndKey_FormatsMessage()
        {
            // Arrange & Act
            var ex = new NotFoundException("Patient", 42);

            // Assert
            Assert.Contains("Patient", ex.Message);
            Assert.Contains("42", ex.Message);
        }

        [Fact]
        public void NotFoundException_IsExceptionType()
        {
            // Arrange & Act
            var ex = new NotFoundException("test");

            // Assert
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void NotFoundException_WithEntityNameAndStringKey_FormatsMessage()
        {
            // Arrange & Act
            var ex = new NotFoundException("Doctor", "doc-001");

            // Assert
            Assert.Contains("Doctor", ex.Message);
            Assert.Contains("doc-001", ex.Message);
        }

        [Fact]
        public void NotFoundException_CanBeThrown()
        {
            // Arrange & Act & Assert
            Assert.Throws<NotFoundException>(() => ThrowNotFoundException());
        }

        private static void ThrowNotFoundException() => throw new NotFoundException("Not found");
        private static void ThrowValidationException() => throw new ValidationException("Invalid input");
        private static void ThrowDuplicateEntryException() => throw new DuplicateEntryException("Email already exists");

        // ValidationException Tests
        [Fact]
        public void ValidationException_WithMessage_SetsMessage()
        {
            // Arrange & Act
            var ex = new ValidationException("Validation failed");

            // Assert
            Assert.Equal("Validation failed", ex.Message);
        }

        [Fact]
        public void ValidationException_IsExceptionType()
        {
            // Arrange & Act
            var ex = new ValidationException("test");

            // Assert
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void ValidationException_CanBeThrown()
        {
            // Arrange & Act & Assert
            Assert.Throws<ValidationException>(() => ThrowValidationException());
        }

        [Fact]
        public void ValidationException_MessageIsPreserved()
        {
            // Arrange
            var message = "Name cannot be empty";

            // Act
            var ex = new ValidationException(message);

            // Assert
            Assert.Equal(message, ex.Message);
        }

        // DuplicateEntryException Tests
        [Fact]
        public void DuplicateEntryException_WithMessage_SetsMessage()
        {
            // Arrange & Act
            var ex = new DuplicateEntryException("Duplicate entry detected");

            // Assert
            Assert.Equal("Duplicate entry detected", ex.Message);
        }

        [Fact]
        public void DuplicateEntryException_IsExceptionType()
        {
            // Arrange & Act
            var ex = new DuplicateEntryException("test");

            // Assert
            Assert.IsAssignableFrom<Exception>(ex);
        }

        [Fact]
        public void DuplicateEntryException_CanBeThrown()
        {
            // Arrange & Act & Assert
            Assert.Throws<DuplicateEntryException>(() => ThrowDuplicateEntryException());
        }

        [Fact]
        public void DuplicateEntryException_MessageIsPreserved()
        {
            // Arrange
            var message = "Email already registered";

            // Act
            var ex = new DuplicateEntryException(message);

            // Assert
            Assert.Equal(message, ex.Message);
        }

        [Fact]
        public void AllExceptions_HaveCorrectInheritance()
        {
            // Arrange & Act
            var notFound = new NotFoundException("test");
            var validation = new ValidationException("test");
            var duplicate = new DuplicateEntryException("test");

            // Assert
            Assert.IsAssignableFrom<Exception>(notFound);
            Assert.IsAssignableFrom<Exception>(validation);
            Assert.IsAssignableFrom<Exception>(duplicate);
        }
    }
}
