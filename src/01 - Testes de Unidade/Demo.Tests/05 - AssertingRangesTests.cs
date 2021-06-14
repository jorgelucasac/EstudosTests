using Xunit;
using Xunit.Abstractions;

namespace Demo.Tests
{
    public class AssertingRangesTests
    {
        [Theory]
        [InlineData(700)]
        [InlineData(1500)]
        [InlineData(2000)]
        [InlineData(7500)]
        [InlineData(8000)]
        [InlineData(15000)]
        public void Funcionario_Salario_FaixasSalariaisDevemRespeitarNivelProfissional(double salario)
        {
            // Arrange & Act
            var funcionario = new Funcionario("Jorge", salario);

            switch (funcionario.NivelProfissional)
            {
                // Assert
                case NivelProfissional.Junior:
                    Assert.InRange(funcionario.Salario, 500, 1999);
                    break;
                case NivelProfissional.Pleno:
                    Assert.InRange(funcionario.Salario, 2000, 7999);
                    break;
                case NivelProfissional.Senior:
                    Assert.InRange(funcionario.Salario, 8000, double.MaxValue);
                    break;
            }

            Assert.NotInRange(funcionario.Salario, 0, 499);
        }
    }
}