using NerdStore.BDD.Tests.Config;

namespace NerdStore.BDD.Tests.Pedidos
{
    public class PedidoTela : PageObjectModel
    {
        public PedidoTela(SeleniumHelper helper) : base(helper)
        {

        }

        public void AcessarVitrineDeProdutos()
        {
            Helper.IrParaUrl(Helper.Configuration.VitrineUrl);
        }

        public void ObterDetalhesDoProduto(int posicao = 1)
        {
            Helper.ClicarPorXPath($"html/body/div/main/div/div/div[{posicao}]/span/a");
        }


        public bool ValidarProdutoDisponivel()
        {
            return Helper.ValidarConteudoUrl(Helper.Configuration.ProdutoUrl);
        }

        public int ObterQuantidadeNoEstoque()
        {
            var elemento = Helper.ObterElementoPorXPath("/html/body/div/main/div/div/div[2]/p[1]");
            var quantidade = elemento.Text.ApenasNumeros();

            if (char.IsNumber(quantidade.ToString(), 0)) return quantidade;

            return 0;
        }
    }
}