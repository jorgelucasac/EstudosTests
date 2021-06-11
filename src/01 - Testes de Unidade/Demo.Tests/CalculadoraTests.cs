using Xunit;

namespace Demo.Tests
{
    public class CalculadoraTests
    {
        [Fact]
        public void Calculadora_Somar_RetornarValorSoma()
        {
            // Arrange
            var calculadora = new Calculadora();

            // Act
            var resultado = calculadora.Somar(5, 5);

            // Assert
            Assert.Equal(10, resultado);
        }

        [Theory]
        [InlineData(2, 2, 4)]
        [InlineData(4, 4, 8)]
        [InlineData(5, 5, 10)]
        [InlineData(15, 5, 20)]
        [InlineData(9, 9, 18)]
        public void Calculadora_Somar_RetornarValoresCorretos(double valo1, double valor2, double total)
        {
            // Arrange
            var calculadora = new Calculadora();

            // Act
            var resultado = calculadora.Somar(valo1, valor2);

            // Assert
            Assert.Equal(total, resultado);
        }

    }
}