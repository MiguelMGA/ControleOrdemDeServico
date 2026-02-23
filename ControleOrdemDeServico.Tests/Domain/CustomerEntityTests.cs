using FluentAssertions;
using OsService.Domain.Entities;
using OsService.Domain.Exceptions;
using Xunit;

namespace OsService.Tests.Domain
{
    public class CustomerEntityTests
    {
        [Fact]
        public void CreateCustomer_WithInvalidName_ShouldThrowDomainException()
        {
            Action act = () =>
                CustomerEntity.Create(
                    "",
                    "27999999999",
                    "miguel@email.com",
                    "12345678900"
                );

            act.Should().Throw<DomainException>()
                .WithMessage("Nome do cliente é obrigatório.");
        }
    }
}
