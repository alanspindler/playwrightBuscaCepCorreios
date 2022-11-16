using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace PlaywrightAutomacao
{
    public class FuncoesProdutosDisponiveis : PageObjectsProdutosDisponiveis
    {
        public enum PosicaoCampoGridMaster : int
        {
            CheckBox = 1,
            Imagem = 2,
            Codigo = 3,
            Nome = 4,
            Marca = 5,
            Categoria = 6,
            QtdSku = 7,
            Canais = 8,
            Status = 9,
            BotaoVisualizarProduto = 10
        }

        public async Task AguardarDivProcessandoDisponiveis(IPage driver)
        {
            Thread.Sleep(1500);
            await AguardarElemento(driver, botaoVisualizarProduto);
            await AguardarElementoNaoVisivel(driver, divCarregando);
            Thread.Sleep(3000);
        }

        public async Task AbrirPaginaProdutosDisponiveis(IPage driver)
        {
            await driver.GotoAsync("https://stg2.it4360.com.br/products/available?name=box");
            await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public async Task AbrirFiltros(IPage driver)
        {
            await ClicarElemento(driver, botaoFiltros);
            await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await ClicarElemento(driver, botaoLimpar);
        }

        public async Task PreencherPesquisaProdutoPorCodigo(IPage driver, string codigoProduto1, string? codigoProduto2 = null)
        {
            await PreencherCampo(driver, campoCodigoProduto, codigoProduto1);
            if (codigoProduto2 != null)
            {
                await driver.Keyboard.PressAsync("Enter");
                await PreencherCampo(driver, campoCodigoProduto, codigoProduto2);
            }
        }

        public async Task PreencherPesquisaProdutoPorReferencia(IPage driver, string referenciaProduto1, string? referenciaProduto2 = null)
        {
            await PreencherCampo(driver, campoReferenciaProduto, referenciaProduto1);
            if (referenciaProduto2 != null)
            {
                await driver.Keyboard.PressAsync("Enter");
                await PreencherCampo(driver, campoReferenciaProduto, referenciaProduto2);
            }
        }

        public async Task PreencherPesquisaProdutoPorNome(IPage driver, string nomeProduto)
        {
            await PreencherCampo(driver, campoNomeProduto, nomeProduto);
            await driver.Keyboard.PressAsync("Enter");
        }

        public async Task PreencherPesquisaProdutoPorMarca(IPage driver, string marcaProduto1, string itemMarcaProduto, string? marcaProduto2 = null, string? itemMarcaProduto2 = null)
        {
            await PreencherCampoESelecionarItem(driver, campoMarcaProduto, marcaProduto1, itemMarcaProduto);
            if (marcaProduto2 != null)
            {
                await PreencherCampoESelecionarItem(driver, campoMarcaProduto, marcaProduto2, itemMarcaProduto2);
            }
        }

        public async Task PreencherPesquisaProdutoPorCategoria(IPage driver, string categoriaProduto1, string itemCategoriaProduto, string? categoriaProduto2 = null, string? itemCategoriaProduto2 = null)
        {
            await PreencherCampoESelecionarItem(driver, campoCategoriaProduto, categoriaProduto1, itemCategoriaProduto);
            if (categoriaProduto2 != null)
            {
                await PreencherCampoESelecionarItem(driver, campoCategoriaProduto, categoriaProduto2, itemCategoriaProduto2);
            }
        }

        public async Task PreencherPesquisaProdutoSemCanal(IPage driver)
        {
            await ClicarElemento(driver, campoApenasProdutoSemCanal);
            await ClicarElemento(driver, itemSimProduto);
        }

        public async Task PreencherPesquisaProdutoPorCanal(IPage driver, string canalProduto1, string itemCanalProduto, string? canalProduto2 = null, string? itemCanalProduto2 = null)
        {
            await PreencherCampoESelecionarItem(driver, campoCanalProduto, canalProduto1, itemCanalProduto);
            if (canalProduto2 != null)
            {
                await PreencherCampoESelecionarItem(driver, campoCanalProduto, canalProduto2, itemCanalProduto2);
            }
        }

        public async Task PreencherPesquisaStatusProdutoHabilitado(IPage driver)
        {
            await ClicarElemento(driver, campoStatusProduto);
            await ClicarElemento(driver, itemStatusProdutoHabilitado);
        }

        public async Task PreencherPesquisaStatusProdutoDesabilitado(IPage driver)
        {
            await ClicarElemento(driver, campoStatusProduto);
            await ClicarElemento(driver, itemStatusProdutoDesabilitado);
        }

        public async Task BuscarNoFiltro(IPage driver)
        {
            await ClicarElemento(driver, "[class='ant-drawer-title']:has-text(\"Filtros\")");
            await driver.WaitForLoadStateAsync(LoadState.Load);
            await driver.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await ClicarElemento(driver, botaoBuscarFiltros);
            await ClicarElemento(driver, botaoFecharFiltros);
            Thread.Sleep(1000);
            await AguardarDivProcessandoDisponiveis(driver);
        }

        public async Task PreencherCamposCadastroProduto(IPage driver, string nomeProduto, string referenciaProduto, string marcaProduto, string itemMarcaProduto,
            string categoriaProduto, string itemCategoriaProduto, string descricaoLongaProduto, string descricaoCurtaProduto, string metaTagProduto, string palavraChaveProduto)
        {
            await PreencherCampo(driver, campoPalavraChaveCadastroProduto, palavraChaveProduto);
            await driver.Keyboard.PressAsync("Enter");
            await PreencherCampo(driver, campoMetaTagsCadastroProduto, metaTagProduto);
            await driver.Keyboard.PressAsync("Enter");
            await PreencherCampo(driver, campoDescricaoCurtaCadastroProduto, descricaoCurtaProduto);
            await PreencherCampo(driver, campoDescricaoLongaCadastroProduto, descricaoLongaProduto);
            await PreencherCampoESelecionarItem(driver, campoCategoriaCadastroProduto, categoriaProduto, itemCategoriaProduto);
            await PreencherCampoESelecionarItem(driver, campoMarcaCadastroProduto, marcaProduto, itemMarcaProduto);
            await PreencherCampo(driver, campoReferenciaCadastroProduto, referenciaProduto);
            await PreencherCampo(driver, campoNomeCadastroProduto, nomeProduto);
        }

        public async Task AbrirUrlRedis(IPage driver, string chave)
        {
            ConexaoBancoRedis conexaoBancoRedis = new ConexaoBancoRedis();
            string chaveRedis = conexaoBancoRedis.GetData<string>(chave);
            await driver.GotoAsync(chaveRedis);
            await driver.WaitForLoadStateAsync(LoadState.Load);
            await driver.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
            Thread.Sleep(2000);
        }

        public async Task PreencherCamposCadastroSku(IPage driver, string referenciaSku, string eanSku, string precoDe, string precoPor, string descricaoSku,
            string pesoSku, string alturaSku, string larguraSku, string comprimentoSku,
            string? pesoRealSku = null, string? pesoCubicoSku = null, string? alturaRealSku = null, string? larguraRealSku = null, string? comprimentoRealSku = null)
        {
            await PreencherCampo(driver, campoReferenciaCadastroSku, referenciaSku);
            await PreencherCampo(driver, campoEanCadastroSku, eanSku);
            await PreencherCampo(driver, campoPrecoDeCadastroSku, precoDe);
            await PreencherCampo(driver, campoPrecoPorCadastroSku, precoPor);
            await PreencherCampo(driver, campoDescricaoCadastroSku, descricaoSku);
            await PreencherCampo(driver, campoPesoCadastroSku, pesoSku);
            await PreencherCampo(driver, campoAlturaCadastroSku, alturaSku);
            await PreencherCampo(driver, campoLarguraCadastroSku, larguraSku);
            await PreencherCampo(driver, campoComprimentoCadastroSku, comprimentoSku);
            if (pesoRealSku != null && pesoCubicoSku != null && alturaRealSku != null && larguraRealSku != null && comprimentoRealSku != null)
            {
                await PreencherCampo(driver, campoPesoRealCadastroSku, pesoRealSku);
                await PreencherCampo(driver, campoPesoCubicoCadastroSku, pesoCubicoSku);
                await PreencherCampo(driver, campoAlturaRealCadastroSku, alturaRealSku);
                await PreencherCampo(driver, campoLarguraRealCadastroSku, larguraRealSku);
                await PreencherCampo(driver, campoComprimentoRealCadastroSku, comprimentoRealSku);
            }
        }

        public async Task PreencherCamposDoCadastroDeImagemPorUrlDoSku(IPage driver)
        {
            await LimparCampo(driver, campoUrlImagem);
            await PreencherCampo(driver, campoUrlImagem, "https://360storagedev.blob.core.windows.net/360-sku-images/50d9dfe9-7fcb-4d59-ba5d-058815d2b380/da425f66-e19f-4017-9f65-416ddf40acb9/images/imagem_teste_automatizado.jpg");
            await LimparCampo(driver, campoNomeImagem);
            await PreencherCampo(driver, campoNomeImagem, "camiseta_teste_automatizado");
            await LimparCampo(driver, campoDescricaoImagem);
            await PreencherCampo(driver, campoDescricaoImagem, "DescricaoImagemSku");
        }

        public async Task PreencherCamposDoCadastroDeImagemPorComputadorDoSku(IPage driver)
        {            
            string caminhoImagem = DIRETORIO_APLICACAO + $"\\ImagensTesteAutomatizado\\bola_teste_automatizado.jpg";
            var fileChooser = await driver.RunAndWaitForFileChooserAsync(async () =>
            {
                await ClicarElemento(driver, localDeArrasteParaImagem);
            });
            await fileChooser.SetFilesAsync(caminhoImagem);
            await AguardarElemento(driver, campoDescricaoImagem);
            await PreencherCampo(driver, campoDescricaoImagem, "DescricaoImagemSku");
            Thread.Sleep(1000);
        }

        public async Task LimparTextoEmEdicaoDoProduto(IPage driver)
        {
            await LimparCampo(driver, campoNomeCadastroProduto);
            await LimparCampo(driver, campoReferenciaCadastroProduto);
            await LimparCampo(driver, campoDescricaoLongaCadastroProduto);
            await LimparCampo(driver, campoDescricaoCurtaCadastroProduto);
            await driver.Locator(campoPalavraChaveCadastroProduto).PressAsync("Backspace");
            await driver.Locator(campoMetaTagsCadastroProduto).PressAsync("Backspace");
        }

        public async Task LimparTextoEmEdicaoDoSku(IPage driver)
        {
            await LimparCampo(driver, campoReferenciaCadastroSku);
            await LimparCampo(driver, campoEanCadastroSku);
            await LimparCampo(driver, campoPrecoDeCadastroSku);
            await LimparCampo(driver, campoPrecoPorCadastroSku);
            await LimparCampo(driver, campoDescricaoCadastroSku);
            await LimparCampo(driver, campoPesoCadastroSku);
            await LimparCampo(driver, campoPesoRealCadastroSku);
            await LimparCampo(driver, campoPesoCubicoCadastroSku);
            await LimparCampo(driver, campoLarguraCadastroSku);
            await LimparCampo(driver, campoLarguraRealCadastroSku);
            await LimparCampo(driver, campoAlturaRealCadastroSku);
            await LimparCampo(driver, campoAlturaCadastroSku);
            await LimparCampo(driver, campoComprimentoCadastroSku);
            await LimparCampo(driver, campoComprimentoRealCadastroSku);
        }
    }
}
