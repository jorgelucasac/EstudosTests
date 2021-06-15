using Features.Clientes;
using System;
using Xunit;

namespace Features.Tests
{
    [CollectionDefinition(nameof(ClienteCollection))]
    public class ClienteCollection : ICollectionFixture<ClienteTestsFixture>
    { }
    public class ClienteTestsFixture : IDisposable
    {


        public Cliente GerarClienteValido()
        {
            var cliente = new Cliente(
                Guid.NewGuid(),
                "Jorge",
                "Lucas",
                DateTime.Now.AddYears(-30),
                "jorge@email.com",
                true,
                DateTime.Now);

            return cliente;
        }

        public Cliente GerarClienteInValido()
        {
            var cliente = new Cliente(
                Guid.NewGuid(),
                "",
                "",
                DateTime.Now,
                "jorge1email.com",
                true,
                DateTime.Now
            );

            return cliente;
        }


        public void Dispose()
        {
        }
    }
}