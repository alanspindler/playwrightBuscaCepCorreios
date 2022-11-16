using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlaywrightAutomacao
{
    public class FuncoesProdutosAVenda : PageObjectsProdutosAVenda
    {
        public enum PosicaoCampoGridMaster : int
        {
            CheckBox = 1,
            Imagem = 2,
            Codigo = 3,
            Nome = 4,
            Marca = 5,
            Categoria = 6,
            Canal = 7,
            Empresas = 8,
            Status = 9,
            StatusAtualizacao = 10
        }

        public async Task AbrirPaginaProdutosAVenda(IPage driver)
        {
            await driver.GotoAsync("https://stg2.it4360.com.br/products/sales?name=Teste%20de");
            //Thread.Sleep(4000);
            await AguardarCarregarPagina(driver);
            //Thread.Sleep(4000);
            await AguardarElemento(driver, botaoVisualizarProduto);
        }

        public async Task PreencherPesquisaProdutoPorNome(IPage driver, string nomeProduto)
        {
            await PreencherCampo(driver, campoNomeProduto, nomeProduto);
            await driver.Keyboard.PressAsync("Enter");
        }

        public async Task AbrirFiltros(IPage driver)
        {
            await ClicarElemento(driver, botaoFiltros);
            await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await ClicarElemento(driver, botaoLimpar);
        }

        public async Task AguardarDivProcessandoAVenda(IPage driver)
        {
            Thread.Sleep(1500);
            await AguardarElemento(driver, botaoVisualizarProduto);
            Thread.Sleep(1000);
            await AguardarElementoNaoVisivel(driver, divCarregando);
            Thread.Sleep(3000);
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
            await AguardarDivProcessandoAVenda(driver);
            await AguardarElemento(driver, botaoVisualizarProduto);
            Thread.Sleep(3000);
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

        public async Task PreencherPesquisaProdutoPorCanal(IPage driver, string canalProduto1, string itemCanalProduto, string? canalProduto2 = null, string? itemCanalProduto2 = null)
        {
            await PreencherCampoESelecionarItem(driver, campoCanalProduto, canalProduto1, itemCanalProduto);
            if (canalProduto2 != null)
            {
                await PreencherCampoESelecionarItem(driver, campoCanalProduto, canalProduto2, itemCanalProduto2);
            }
        }

        public async Task PreencherPesquisaProdutoPorStatus(IPage driver, string itemStatusProduto)
        {
            await ForcarClicarElemento(driver, campoStatusProduto);
            await AguardarElemento(driver, itemStatusProduto);
            await ClicarElemento(driver, itemStatusProduto);
        }

        public async Task PreencherPesquisaProdutoPorStatusAtualizacao(IPage driver, string itemStatusAtualizacaoProduto)
        {
            await ForcarClicarElemento(driver, campoStatusAtualizacaoProduto);
            await AguardarElemento(driver, itemStatusAtualizacaoProduto);
            await ClicarElemento(driver, itemStatusAtualizacaoProduto);
        }

        public async Task LimparTextoEmEdicaoDoProduto(IPage driver)
        {
            await LimparCampo(driver, campoNomeCadastroProduto);
            await LimparCampo(driver, campoDescricaoLongaCadastroProduto);
            await LimparCampo(driver, campoDescricaoCurtaCadastroProduto);
            await driver.Locator(campoPalavraChaveCadastroProduto).PressAsync("Backspace");
            await driver.Locator(campoMetaTagsCadastroProduto).PressAsync("Backspace");
        }

        public async Task LimparTextoEmEdicaoDoSku(IPage driver)
        {
            await LimparCampo(driver, campoDescricaoCadastroSku);
            await LimparCampo(driver, campoPrecoDeCadastroSku);
            await LimparCampo(driver, campoPrecoPorCadastroSku);
            await LimparCampo(driver, campoPesoCadastroSku);
            await LimparCampo(driver, campoPesoRealCadastroSku);
            await LimparCampo(driver, campoAlturaCadastroSku);
            await LimparCampo(driver, campoAlturaRealCadastroSku);
            await LimparCampo(driver, campoLarguraCadastroSku);
            await LimparCampo(driver, campoLarguraRealCadastroSku);
            await LimparCampo(driver, campoComprimentoCadastroSku);
            await LimparCampo(driver, campoComprimentoRealCadastroSku);
            await LimparCampo(driver, campoPesoCubicoCadastroSku);
        }

        public async Task LimparTextoEmEdicaoDoAtributoProduto(IPage driver)
        {
            await LimparCampo(driver, campoAtributoCampoAbertoProdutoCategSaude);
            await driver.Locator(campoAtributoMultivaloradoProdutoCategSaude).PressAsync("Backspace");
        }
    }
}
