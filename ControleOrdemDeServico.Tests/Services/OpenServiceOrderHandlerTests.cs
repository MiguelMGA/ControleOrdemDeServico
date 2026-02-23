using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OsService.Domain.Entities;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.OpenServiceOrder;
using Xunit;

namespace OsService.Tests.Services
{
    public class OpenServiceOrderHandlerTests
    {
        [Fact]
        public async Task OpenServiceOrder_WithExistingCustomer_ShouldCreateSuccessfully()
        {
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var serviceOrderRepositoryMock = new Mock<IServiceOrderRepository>();
            var auditLogRepositoryMock = new Mock<IAuditLogRepository>();
            var loggerMock = new Mock<ILogger<OpenServiceOrderHandler>>();

            var customerId = Guid.NewGuid();

            customerRepositoryMock
                .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            serviceOrderRepositoryMock
                .Setup(r => r.InsertAsync(It.IsAny<ServiceOrderEntity>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new OpenServiceOrderHandler(
                customerRepositoryMock.Object,
                serviceOrderRepositoryMock.Object,
                auditLogRepositoryMock.Object,
                loggerMock.Object);

            var command = new OpenServiceOrderCommand(
                customerId,
                "Troca de tela");

            var (Id, Number) = await handler.Handle(command, CancellationToken.None);

            Id.Should().NotBe(Guid.Empty);

            serviceOrderRepositoryMock.Verify(
                r => r.InsertAsync(It.IsAny<ServiceOrderEntity>(), It.IsAny<CancellationToken>()),
                Times.Once);

            auditLogRepositoryMock.Verify(
                a => a.AddAsync(
                    AuditEntities.ServiceOrder,
                    Id,
                    AuditActions.Opened,
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task OpenServiceOrder_WithNonExistingCustomer_ShouldThrow()
        {
            var customerRepositoryMock = new Mock<ICustomerRepository>();
            var serviceOrderRepositoryMock = new Mock<IServiceOrderRepository>();
            var auditLogRepositoryMock = new Mock<IAuditLogRepository>();
            var loggerMock = new Mock<ILogger<OpenServiceOrderHandler>>();

            var customerId = Guid.NewGuid();

            customerRepositoryMock
                .Setup(r => r.ExistsAsync(customerId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(false);

            var handler = new OpenServiceOrderHandler(
                customerRepositoryMock.Object,
                serviceOrderRepositoryMock.Object,
                auditLogRepositoryMock.Object,
                loggerMock.Object);

            var command = new OpenServiceOrderCommand(
                customerId,
                "Troca de tela");

            Func<Task> act = async () =>
                await handler.Handle(command, CancellationToken.None);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Customer não encontrado.");

            serviceOrderRepositoryMock.Verify(
                r => r.InsertAsync(It.IsAny<ServiceOrderEntity>(), It.IsAny<CancellationToken>()),
                Times.Never);

            auditLogRepositoryMock.Verify(
                a => a.AddAsync(
                    AuditEntities.ServiceOrder,
                    null,
                    AuditActions.OpenFailedCustomerNotFound,
                    It.IsAny<string?>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}