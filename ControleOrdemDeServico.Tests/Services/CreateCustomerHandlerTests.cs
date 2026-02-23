using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OsService.Domain.Entities;
using OsService.Domain.Exceptions;
using OsService.Infrastructure.Logging;
using OsService.Infrastructure.Repository;
using OsService.Services.V1.CreateCustomer;
using Xunit;

namespace OsService.Tests.Services;

public class CreateCustomerHandlerTests
{
    [Fact]
    public async Task CreateCustomer_WithValidData_ShouldReturnId()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        var loggerMock = new Mock<ILogger<CreateCustomerHandler>>();

        customerRepositoryMock
            .Setup(r => r.ExistsByDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        customerRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        customerRepositoryMock
            .Setup(r => r.ExistsByPhoneAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        customerRepositoryMock
            .Setup(r => r.InsertAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        auditLogRepositoryMock
            .Setup(a => a.AddAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var handler = new CreateCustomerHandler(
            customerRepositoryMock.Object,
            auditLogRepositoryMock.Object,
            loggerMock.Object
        );

        var command = new CreateCustomerCommand(
            "Miguel",
            "27999999999",
            "miguel@email.com",
            "12345678900"
        );

        var result = await handler.Handle(command, CancellationToken.None);

        result.Should().NotBe(Guid.Empty);

        customerRepositoryMock.Verify(
            r => r.InsertAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()),
            Times.Once);

        auditLogRepositoryMock.Verify(
            a => a.AddAsync(
                It.IsAny<string>(),
                It.IsAny<Guid?>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateCustomer_WithDuplicateDocument_ShouldThrowDomainException()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        var loggerMock = new Mock<ILogger<CreateCustomerHandler>>();

        var document = "12345678900";

        customerRepositoryMock
            .Setup(r => r.ExistsByDocumentAsync(document, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateCustomerHandler(
            customerRepositoryMock.Object,
            auditLogRepositoryMock.Object,
            loggerMock.Object
        );

        var command = new CreateCustomerCommand(
            "Miguel",
            "27999999999",
            "miguel@email.com",
            document
        );

        Func<Task> act = async () =>
            await handler.Handle(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<DomainException>();

        exception.Which.Message.Should().Be("Já existe cliente com o mesmo documento.");

        customerRepositoryMock.Verify(
            r => r.InsertAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);

        auditLogRepositoryMock.Verify(
            a => a.AddAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateCustomer_WithDuplicateEmail_ShouldThrowDomainException()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        var loggerMock = new Mock<ILogger<CreateCustomerHandler>>();

        var email = "miguel@email.com";

        customerRepositoryMock
            .Setup(r => r.ExistsByDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        customerRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateCustomerHandler(
            customerRepositoryMock.Object,
            auditLogRepositoryMock.Object,
            loggerMock.Object
        );

        var command = new CreateCustomerCommand(
            "Miguel",
            "27999999999",
            email,
            "12345678900"
        );

        Func<Task> act = async () =>
            await handler.Handle(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<DomainException>();

        exception.Which.Message.Should().Be("Já existe cliente com o mesmo email.");

        customerRepositoryMock.Verify(
            r => r.InsertAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);

        auditLogRepositoryMock.Verify(
            a => a.AddAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateCustomer_WithDuplicatePhone_ShouldThrowDomainException()
    {
        var customerRepositoryMock = new Mock<ICustomerRepository>();
        var auditLogRepositoryMock = new Mock<IAuditLogRepository>();
        var loggerMock = new Mock<ILogger<CreateCustomerHandler>>();

        var phone = "27999999999";

        customerRepositoryMock
            .Setup(r => r.ExistsByDocumentAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        customerRepositoryMock
            .Setup(r => r.ExistsByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        customerRepositoryMock
            .Setup(r => r.ExistsByPhoneAsync(phone, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var handler = new CreateCustomerHandler(
            customerRepositoryMock.Object,
            auditLogRepositoryMock.Object,
            loggerMock.Object
        );

        var command = new CreateCustomerCommand(
            "Miguel",
            phone,
            "miguel@email.com",
            "12345678900"
        );

        Func<Task> act = async () =>
            await handler.Handle(command, CancellationToken.None);

        var exception = await act.Should().ThrowAsync<DomainException>();

        exception.Which.Message.Should().Be("Já existe cliente com o mesmo telefone.");

        customerRepositoryMock.Verify(
            r => r.InsertAsync(It.IsAny<CustomerEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);

        auditLogRepositoryMock.Verify(
            a => a.AddAsync(
                It.IsAny<string>(),
                null,
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}