using FluentAssertions;
using OsService.Domain.Entities;
using OsService.Domain.Enums;
using OsService.Domain.Exceptions;
using Xunit;

namespace OsService.Tests.Domain
{
    public class ServiceOrderEntityTests
    {
        [Fact]
        public void CreateServiceOrder_WithValidData_ShouldCreateSuccessfully()
        {
            var customerId = Guid.NewGuid();

            var os = ServiceOrderEntity.Create(
                customerId,
                "Troca de tela"
            );

            os.CustomerId.Should().Be(customerId);
            os.Description.Should().Be("Troca de tela");
            os.Status.Should().Be(ServiceOrderStatus.Open);
            os.Price.Should().BeNull();
            os.Currency.Code.Should().Be("BRL");
            os.OpenedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void CreateServiceOrder_WithEmptyCustomerId_ShouldThrow()
        {
            Action act = () =>
                ServiceOrderEntity.Create(
                    Guid.Empty,
                    "Troca de tela"
                );

            act.Should().Throw<DomainException>()
                .WithMessage("CustomerId não pode estar vazio.");
        }

        [Fact]
        public void CreateServiceOrder_WithEmptyDescription_ShouldThrow()
        {
            Action act = () =>
                ServiceOrderEntity.Create(
                    Guid.NewGuid(),
                    ""
                );

            act.Should().Throw<DomainException>()
                .WithMessage("A descrição é obrigatória.");
        }

        [Fact]
        public void StartServiceOrder_ShouldChangeStatusToInProgress()
        {
            var os = ServiceOrderEntity.Create(
                Guid.NewGuid(),
                "Teste"
            );

            os.Start();

            os.Status.Should().Be(ServiceOrderStatus.InProgress);
            os.StartedAt.Should().NotBeNull();
        }

        [Fact]
        public void FinishServiceOrder_WithPrice_ShouldFinalize()
        {
            var os = ServiceOrderEntity.Create(
                Guid.NewGuid(),
                "Teste"
            );

            os.Start();
            os.UpdatePrice(150);

            os.Finish();

            os.Status.Should().Be(ServiceOrderStatus.Finished);
            os.FinishedAt.Should().NotBeNull();
        }

        [Fact]
        public void FinishServiceOrder_WithoutStarting_ShouldThrow()
        {
            var os = ServiceOrderEntity.Create(
                Guid.NewGuid(),
                "Teste"
            );

            os.UpdatePrice(150);

            Action act = () => os.Finish();

            act.Should().Throw<DomainException>()
                .WithMessage("Transição inválida de Open para Finished.");
        }

        [Fact]
        public void StartServiceOrder_AfterFinished_ShouldThrow()
        {
            var os = ServiceOrderEntity.Create(
                Guid.NewGuid(),
                "Teste"
            );

            os.Start();
            os.UpdatePrice(150);
            os.Finish();

            Action act = () => os.Start();

            act.Should().Throw<DomainException>();
        }

        [Fact]
        public void UpdatePrice_WithNegativeValue_ShouldThrow()
        {
            var os = ServiceOrderEntity.Create(
                Guid.NewGuid(),
                "Teste"
            );

            Action act = () => os.UpdatePrice(-10);

            act.Should().Throw<DomainException>()
                .WithMessage("O valor não pode ser negativo.");
        }

        [Fact]
        public void FinishServiceOrder_WithoutPrice_ShouldThrow()
        {
            var os = ServiceOrderEntity.Create(
                Guid.NewGuid(),
                "Teste"
            );

            os.Start();

            Action act = () => os.Finish();

            act.Should().Throw<DomainException>()
                .WithMessage("Não é possível finalizar a ordem de serviço sem o valor.");
        }

        [Fact]
        public void UpdatePrice_AfterFinished_ShouldThrow()
        {
            var os = ServiceOrderEntity.Create(
                Guid.NewGuid(),
                "Teste"
            );

            os.Start();
            os.UpdatePrice(100);
            os.Finish();

            Action act = () => os.UpdatePrice(200);

            act.Should().Throw<DomainException>()
                .WithMessage("Não é possível alterar o valor após a finalização.");
        }
    }
}