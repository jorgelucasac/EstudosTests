using NerdStore.BDD.Tests.Config;

namespace NerdStore.BDD.Tests.Usuarios
{
    public class BaseUsuarioTela : PageObjectModel
    {
        public BaseUsuarioTela(SeleniumHelper helper) : base(helper) { }

        public void AcessarSiteLoja()
        {
            Helper.IrParaUrl(Helper.Configuration.DomainUrl);
        }

        public bool ValidarSaudacaoUsuarioLogado(Usuario usuario)
        {
            return Helper.ObterTextoElementoPorId("saudacaoUsuario").Contains(usuario.Email);
        }
    }
}