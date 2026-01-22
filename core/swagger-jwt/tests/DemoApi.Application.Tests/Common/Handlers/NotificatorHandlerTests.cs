using DemoApi.Domain.Handlers;
using FluentAssertions;

namespace DemoApi.Application.Tests.Common.Handlers
{
    public class NotificatorHandlerTests
    {
        [Fact]
        public void Constructor_ShouldInitializeEmptyErrorList()
        {
            // Arrange & Act
            NotificatorHandler notificator = new();

            // Assert
            notificator.HasErrors().Should().BeFalse();
            notificator.GetErrors().Should().BeEmpty();
        }

        [Fact]
        public void AddError_ShouldAddNotificationToList()
        {
            // Arrange
            NotificatorHandler notificator = new();
            string errorMessage = "Test error message";

            // Act
            notificator.AddError(errorMessage);

            // Assert
            notificator.HasErrors().Should().BeTrue();
            notificator.GetErrors().Should().HaveCount(1);
            notificator.GetErrors().First().Message.Should().Be(errorMessage);
        }

        [Fact]
        public void AddError_ShouldAddMultipleNotifications()
        {
            // Arrange
            NotificatorHandler notificator = new();

            // Act
            notificator.AddError("Error 1");
            notificator.AddError("Error 2");
            notificator.AddError("Error 3");

            // Assert
            notificator.HasErrors().Should().BeTrue();
            notificator.GetErrors().Should().HaveCount(3);
        }

        [Fact]
        public void HasErrors_ShouldReturnTrue_WhenErrorsExist()
        {
            // Arrange
            NotificatorHandler notificator = new();
            notificator.AddError("Some error");

            // Act
            bool result = notificator.HasErrors();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void HasErrors_ShouldReturnFalse_WhenNoErrors()
        {
            // Arrange
            NotificatorHandler notificator = new();

            // Act
            bool result = notificator.HasErrors();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void GetErrors_ShouldReturnAllNotifications()
        {
            // Arrange
            NotificatorHandler notificator = new();
            notificator.AddError("Error 1");
            notificator.AddError("Error 2");

            // Act
            IEnumerable<Notification> errors = notificator.GetErrors();

            // Assert
            errors.Should().HaveCount(2);
            errors.Select(e => e.Message).Should().Contain(new[] { "Error 1", "Error 2" });
        }

        [Fact]
        public void GetErrors_ShouldReturnEmptyList_WhenNoErrorsAdded()
        {
            // Arrange
            NotificatorHandler notificator = new();

            // Act
            IEnumerable<Notification> errors = notificator.GetErrors();

            // Assert
            errors.Should().BeEmpty();
        }
    }
}