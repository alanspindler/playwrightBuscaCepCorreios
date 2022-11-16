using Dapper;
using Microsoft.Playwright;
using NUnit.Framework;
using Portal.Entities;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace PlaywrightAutomacao
{

    public class ProdutosAVendaMasterTeste : FuncoesProdutosAVenda
    {
        private FuncoesLogin _funcoesLogin = new FuncoesLogin();
        private FuncoesBancoIt4360Stg _funcoesBancoIt4360Stg = new FuncoesBancoIt4360Stg();
        private ConexaoBancoRedis _conexaoBancoRedis = new ConexaoBancoRedis();
        private SqlConnection _connection;

        public ProdutosAVendaMasterTeste(FuncoesLogin funcoesLogin, FuncoesBancoIt4360Stg funcoesBancoIt4360Stg, ConexaoBancoRedis conexaoBancoRedis)
        {
            _funcoesLogin = funcoesLogin;
            _funcoesBancoIt4360Stg = funcoesBancoIt4360Stg;
            _conexaoBancoRedis = conexaoBancoRedis;
        }

        public ProdutosAVendaMasterTeste() { }

        [OneTimeSetUp]
        protected void OneTimeSetUp()
        {
            _funcoesBancoIt4360Stg.ExcluirProdutoAVenda();
            _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel();
            _conexaoBancoRedis.DeleteData("IdProdutoAVenda");
            _conexaoBancoRedis.DeleteData("UrlSkuSemAtributo");
            _conexaoBancoRedis.DeleteData("UrlProdutoParaEdicao");
            _conexaoBancoRedis.DeleteData("UrlSkuParaEdicao");
            _conexaoBancoRedis.DeleteData("UrlParaCurarProduto");
        }

        [OneTimeTearDown]
        protected void OneTimeTearDown()
        {

        }

        [Parallelizable]
        [Test]
        public async Task B01_M_AVenda_PesquisarProdutoPorCodigo()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string codigoProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCodigo(driver, "40523381-f184-42c8-8a9e-8d2102182a1d");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                    Assert.AreEqual("40523381-f184-42c8-8a9e-8d2102182a1d", codigoProduto, "O produto encontrado n�o corresponde ao c�digo desejado. C�digo encontrado: " + codigoProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B02_M_AVenda_PesquisarProdutosPorMultiplosCodigos()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string codigoProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCodigo(driver, "40523381-f184-42c8-8a9e-8d2102182a1d", "61dd6abc-5619-4bf6-8d06-00e358e35926");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                    codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                    Assert.IsTrue("40523381-f184-42c8-8a9e-8d2102182a1d" == codigoProduto || "61dd6abc-5619-4bf6-8d06-00e358e35926" == codigoProduto,
                        "O c�digo encontrado n�o corresponde a nenhuma dos itens cotidos na lista");
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B03_M_AVenda_PesquisarProdutoPorReferencia()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string codigoProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorReferencia(driver, "Produto Alan 3");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                _connection = new SqlConnection("Data Source=stg360.database.windows.net;Initial Catalog=stg360;Integrated Security=False;User ID=administrador;Password=Kemed@#100;MultipleActiveResultSets=true");
                using (var connection = _connection)
                {
                    foreach (int linha in linhasTabelaGridPrincipal)
                    {
                        codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                        var query = $"select Id, IdProduct, RefChannelProduct from ChannelProduct where Id = '{codigoProduto}'";
                        var produto = connection.QueryFirstOrDefault<ChannelProduct>(query);
                        // A refer�ncia do canal � em formato de id
                        Assert.AreEqual("eb565d1c-7ac4-460a-9899-74519922f6aa", produto.RefChannelProduct, "O produto encontrado n�o contem a refer�ncia desejada. Refer�ncia encontrada: " + produto.RefChannelProduct);
                    }
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B04_M_AVenda_PesquisarProdutosPorMultiplasReferencias()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string codigoProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorReferencia(driver, "Produto Alan 3", "22415");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                _connection = new SqlConnection("Data Source=stg360.database.windows.net;Initial Catalog=stg360;Integrated Security=False;User ID=administrador;Password=Kemed@#100;MultipleActiveResultSets=true");
                using (var connection = _connection)
                {
                    foreach (int linha in linhasTabelaGridPrincipal)
                    {
                        codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                        var query = $"select Id, IdProduct, RefChannelProduct from ChannelProduct where Id = '{codigoProduto}'";
                        var produto = connection.QueryFirstOrDefault<ChannelProduct>(query);
                        // A refer�ncia do canal � em formato de id
                        Assert.IsTrue(produto.RefChannelProduct == "eb565d1c-7ac4-460a-9899-74519922f6aa" || produto.RefChannelProduct == "9f4c029b-ead8-4599-a363-4f7541b7e1b8", produto.RefChannelProduct, "O produto encontrado n�o contem a refer�ncia desejada. Refer�ncia encontrada: " + produto.RefChannelProduct);
                    }
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B05_M_AVenda_PesquisarProdutoPorNome()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string nomeProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorNome(driver, "Cal�a Moletom Cinza");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Nome));
                    Assert.IsTrue(nomeProduto.ToLower().Contains("cal�a moletom cinza"), "O produto encontrado n�o possui o nome desejado. O nome encontrado: " + nomeProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B06_M_AVenda_PesquisarProdutoPorNomeParcial()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string nomeProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorNome(driver, "Cal�a");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Nome));
                    Assert.IsTrue(nomeProduto.ToLower().Contains("cal�a"), "O produto encontrado n�o possui o nome desejado. O nome encontrado: " + nomeProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                await driver.CloseAsync();
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B07_M_AVenda_PesquisarProdutoPorMarca()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string marcaProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado n�o possui a marca desejada. Marca encontrada: " + marcaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B08_M_AVenda_PesquisarProdutosPorMultiplasMarcas()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string marcaProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto, "MarcaTesteAnyMarket", itemAnyMarketMarcaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.IsTrue(marcaProduto.ToLower() == "it4 solution" || marcaProduto.ToLower() == "marcatesteanymarket", "O produto encontrado n�o possui a marca desejada. Marca encontrada: " + marcaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B09_M_AVenda_PesquisarProdutoPorCategoria()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string categoriaProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCategoria(driver, "Roupas", itemRoupasCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("roupas", categoriaProduto.ToLower(), "O produto encontrado n�o possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B10_M_AVenda_PesquisarProdutoPorMultiplasCategoria()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string categoriaProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCategoria(driver, "Roupas", itemRoupasCategoriaProduto, "Teste", itemTesteCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.IsTrue(categoriaProduto.ToLower() == "roupas" || categoriaProduto.ToLower() == "teste", "O produto encontrado n�o possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B11_M_AVenda_PesquisarProdutoPorCanal()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string canalProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCanal(driver, "IT4Solution VTEX", itemIt4SolutionVtexCanalProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canal));
                    Assert.AreEqual("it4solution vtex", canalProduto.ToLower(), "O produto encontrado n�o possui o canal desejado. Canal encontrado: " + canalProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B12_M_AVenda_PesquisarProdutoPorMultiplosCanais()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string canalProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCanal(driver, "IT4Solution VTEX", itemIt4SolutionVtexCanalProduto, "IT4Solution Magento", itemIt4MagentoCanalProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canal));
                    Assert.IsTrue(canalProduto.ToLower().Contains("it4solution vtex") || canalProduto.ToLower().Contains("it4solution magento"),
                        "O produto encontrado n�o possui o canal desejado. Canal encontrado: " + canalProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B13_M_AVenda_PesquisarProdutoPorNomeMarca()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string marcaProduto;
                string nomeProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await PreencherPesquisaProdutoPorNome(driver, "Cal�a Moletom Cinza");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Nome));
                    Assert.IsTrue(nomeProduto.ToLower().Contains("cal�a moletom cinza"), "O produto encontrado n�o possui o nome desejado. O nome encontrado: " + nomeProduto);
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado n�o possui a marca desejada. Marca encontrada: " + marcaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B14_M_AVenda_PesquisarProdutoPorNomeCategoria()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string categoriaProduto;
                string nomeProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCategoria(driver, "Higiene Pessoal", itemHigienePessoalCategoriaProduto);
                await PreencherPesquisaProdutoPorNome(driver, "Cal�a Moletom Cinza");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Nome));
                    Assert.IsTrue(nomeProduto.ToLower().Contains("cal�a moletom cinza"), "O produto encontrado n�o possui o nome desejado. O nome encontrado: " + nomeProduto);
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("higiene pessoal", categoriaProduto.ToLower(), "O produto encontrado n�o possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B15_M_AVenda_PesquisarProdutoPorMarcaCategoria()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string categoriaProduto;
                string marcaProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await PreencherPesquisaProdutoPorCategoria(driver, "Higiene Pessoal", itemHigienePessoalCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado n�o possui a marca desejada. Marca encontrada: " + marcaProduto);
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("higiene pessoal", categoriaProduto.ToLower(), "O produto encontrado n�o possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B16_M_AVenda_PesquisarProdutoPorMarcaCategoriaCanal()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string categoriaProduto;
                string marcaProduto;
                string canalProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await PreencherPesquisaProdutoPorCategoria(driver, "Higiene Pessoal", itemHigienePessoalCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado n�o possui a marca desejada. Marca encontrada: " + marcaProduto);
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("higiene pessoal", categoriaProduto.ToLower(), "O produto encontrado n�o possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canal));
                    Assert.AreEqual("it4solution vtex", canalProduto.ToLower(), "O produto encontrado n�o possui o canal desejado. Canal encontrado: " + canalProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B17_M_AVenda_PesquisarProdutoPorStatusCurado()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoCurado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("curado", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B18_M_AVenda_PesquisarProdutoPorStatusNaoCurado()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoNaoCurado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("n�o curado", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B19_M_AVenda_PesquisarProdutoPorStatusErroNaIntegracao()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoErroNaIntegracao);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("erro de integra��o", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B20_M_AVenda_PesquisarProdutoPorStatusSincronizando()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoSincronizando);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("sincronizando", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B21_M_AVenda_PesquisarProdutoPorStatusAtivo()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoAtivo);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("ativo", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B22_M_AVenda_PesquisarProdutoPorStatusDesabilitado()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoDesabilitado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("desabilitado", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B23_M_AVenda_PesquisarProdutoPorStatusAtualizacaoSincronizando()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusAtualizacaoProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoSincronizando);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusAtualizacaoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.StatusAtualizacao));
                    Assert.AreEqual("sincronizando", statusAtualizacaoProduto.ToLower(), "O produto encontrado n�o possui o status de atualiza��o desejado. Status de atualiza��o encontrado: " + statusAtualizacaoProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B24_M_AVenda_PesquisarProdutoPorStatusAtualizacaoErroNaIntegracao()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusAtualizacaoProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoErroNaIntegracao);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusAtualizacaoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.StatusAtualizacao));
                    Assert.AreEqual("erro na integra��o", statusAtualizacaoProduto.ToLower(), "O produto encontrado n�o possui o status de atualiza��o desejado. Status de atualiza��o encontrado: " + statusAtualizacaoProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B25_M_AVenda_PesquisarProdutoPorStatusAtualizacaoNaoSincronizado()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusAtualizacaoProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoNaoSincronizado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusAtualizacaoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.StatusAtualizacao));
                    Assert.AreEqual("n�o sincronizado", statusAtualizacaoProduto.ToLower(), "O produto encontrado n�o possui o status de atualiza��o desejado. Status de atualiza��o encontrado: " + statusAtualizacaoProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B26_M_AVenda_PesquisarProdutoPorStatusAtualizacaoSincronizado()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusAtualizacaoProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoSincronizado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusAtualizacaoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.StatusAtualizacao));
                    Assert.AreEqual("sincronizado", statusAtualizacaoProduto.ToLower(), "O produto encontrado n�o possui o status de atualiza��o desejado. Status de atualiza��o encontrado: " + statusAtualizacaoProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B27_M_AVenda_PesquisarProdutoPorMarcaCategoriaCanalStatus()
        {
            // Cria��o e configura��o do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string marcaProduto;
                string categoriaProduto;
                string statusProduto;
                string canalProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoNaoCurado);
                await PreencherPesquisaProdutoPorCanal(driver, "IT4Solution VTEX", itemIt4SolutionVtexCanalProduto);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await PreencherPesquisaProdutoPorCategoria(driver, "Higiene Pessoal", itemHigienePessoalCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "N�o h� registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("n�o curado", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canal));
                    Assert.AreEqual("it4solution vtex", canalProduto.ToLower(), "O produto encontrado n�o possui o canal desejado. Canal encontrado: " + canalProduto);
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("higiene pessoal", categoriaProduto.ToLower(), "O produto encontrado n�o possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado n�o possui a marca desejada. Marca encontrada: " + marcaProduto);
                }
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B28_M_AVenda_ValidarExistenciaDeAspectosDaPaginaAVendaMaster()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                await ClicarElemento(driver, botaoMaximizarMenu);
                await ClicarElemento(driver, botaoMenuProdutos);
                await AguardarElemento(driver, botaoMenuAVenda);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoMenuAVenda), "O Submenu A Venda n�o p�de ser encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoAtualizarProduto), "O bot�o Atualizar Produto n�o foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoFiltros), "O bot�o Filtros n�o foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoVisualizarProduto), "O bot�o Visualizar Produto n�o foi encontrado");
                await ClicarElemento(driver, botaoAtualizarProduto);
                await AguardarElemento(driver, botaoUpload);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoExportarPlanilha));
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoUpload));
                await ClicarElemento(driver, botaoFecharJanela);
                //Valida se o primeiro elemento do grid � um checkbox
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoCheckBox), "O Checkbox (selecionar todos) do cabe�alho n�o foi encontrado");
                // Verifica��o do grid
                Assert.AreEqual("Imagem", await RetornaTextoElemento(driver, textoCabe�alhoImagem), "O texto do cabe�alho da coluna 'Imagem' n�o corresponde ao nome 'Imagem'");
                Assert.AreEqual("C�digo", await RetornaTextoElemento(driver, textoCabe�alhoCodigo), "O texto do cabe�alho da coluna 'C�digo' n�o corresponde ao nome 'C�digo'");
                Assert.AreEqual("Nome", await RetornaTextoElemento(driver, textoCabe�alhoNome), "O texto do cabe�alho da coluna 'Nome' n�o corresponde ao nome 'Nome'");
                Assert.AreEqual("Marca", await RetornaTextoElemento(driver, textoCabe�alhoMarca), "O texto do cabe�alho da coluna 'Marca' n�o corresponde ao nome 'Marca'");
                Assert.AreEqual("Categoria", await RetornaTextoElemento(driver, textoCabe�alhoCategoria), "O texto do cabe�alho da coluna 'Categoria' n�o corresponde ao nome 'Categoria'");
                Assert.AreEqual("Canal", await RetornaTextoElemento(driver, textoCabe�alhoCanal), "O texto do cabe�alho da coluna 'Canal' n�o corresponde ao nome 'Qtd. SKUS'");
                Assert.AreEqual("Empresa", await RetornaTextoElemento(driver, textoCabe�alhoEmpresa), "O texto do cabe�alho da coluna 'Empresa' n�o corresponde ao nome 'Canais'");
                Assert.AreEqual("Status", await RetornaTextoElemento(driver, textoCabe�alhoStatus), "O texto do cabe�alho da coluna 'Status' n�o corresponde ao nome 'Status'");
                Assert.AreEqual("Status Atualiza��o", await RetornaTextoElemento(driver, textoCabe�alhoStatusAtualizacao), "O texto do cabe�alho da coluna 'Status atualiza��o' n�o corresponde ao nome 'Status'");
                // Verificar os quadros de status
                Assert.AreEqual("Resumo de status do produto", await RetornaTextoElemento(driver, labelQuadroStatusProduto), "O t�tulo do quadro de resumos dos 'Status' n�o p�de ser visualizado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusNaoCurado), "No quadro de resumos do status, n�o foi poss�vel visualizar o campo 'N�o curado'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusCurado), "No quadro de resumos do status, n�o foi poss�vel visualizar o campo 'Curado'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusSincronizando), "No quadro de resumos do status, n�o foi poss�vel visualizar o campo 'Sincronizando'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusAtivo), "No quadro de resumos do status, n�o foi poss�vel visualizar o campo 'Ativo'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusErroNaIntegra��o), "No quadro de resumos do status, n�o foi poss�vel visualizar o campo 'Erro de Integra��o'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusDesabilitado), "No quadro de resumos do status, n�o foi poss�vel visualizar o campo 'Desabilitado'");
                // Verificar os quadros de status atualiza��o
                Assert.AreEqual("Resumo de status de atualiza��o", await RetornaTextoElemento(driver, labelQuadroStatusAtualizacaoProduto), "O t�tulo do quadro de resumos dos 'Status de atualiza��o' n�o p�de ser visualizado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusAtualizacaoNaoSincronizado), "No quadro de resumos do status de atualiza��o, n�o foi poss�vel visualizar o campo 'N�o sincronizando'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusAtualizacaoSincronizando), "No quadro de resumos do status de atualiza��o, n�o foi poss�vel visualizar o campo 'Sincronizando'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusAtualizacaoSincronizado), "No quadro de resumos do status de atualiza��o, n�o foi poss�vel visualizar o campo 'Sincronizado'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, labelQuadroStatusAtualizacaoErroNaIntegracao), "No quadro de resumos do status de atualiza��o, n�o foi poss�vel visualizar o campo 'Erro na integra��o'");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B29_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusSincronizando()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoSincronizando);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                Assert.IsFalse(await ElementoEstaVisivel(driver, botaoAcoes));
                await ClicarElemento(driver, botaoVisualizarProduto);
                // Verifica que o bot�o de edi��o interno do produto n�o existe
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.IsFalse(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto foi localizado, contudo, neste cen�rio ele n�o deveria estar dispon�vel");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B30_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusDesabilitado()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoDesabilitado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                await ForcarClicarElemento(driver, botaoAcoes);
                await AguardarElemento(driver, menuDoBotaoAcoes);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEditarProduto) == true && await ElementoEstaAtivo(driver, botaoEditarProduto) == false, "O bot�o 'Editar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoRemoverProduto) == true && await ElementoEstaAtivo(driver, botaoRemoverProduto) == false, "O bot�o 'Remover' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoDesabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoDesabilitarProduto) == false, "O bot�o 'Desabilitar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoHabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoHabilitarProduto) == true, "O bot�o 'Habilitar' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, menuDoBotaoAcoes) == true && await ElementoEstaAtivo(driver, botaoSincronizarAlteracoesProduto) == false, "O bot�o 'Sincronizar' do produto est� ativo");
                // Verifica que o bot�o de edi��o interno do produto existe e funciona
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.IsFalse(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B31_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusCurado()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoCurado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                await ForcarClicarElemento(driver, botaoAcoes);
                await AguardarElemento(driver, menuDoBotaoAcoes);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEditarProduto) == true && await ElementoEstaAtivo(driver, botaoEditarProduto) == true, "O bot�o 'Editar' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoRemoverProduto) == true && await ElementoEstaAtivo(driver, botaoRemoverProduto) == true, "O bot�o 'Remover' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoDesabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoDesabilitarProduto) == false, "O bot�o 'Desabilitar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoHabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoHabilitarProduto) == false, "O bot�o 'Habilitar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, menuDoBotaoAcoes) == true && await ElementoEstaAtivo(driver, botaoSincronizarAlteracoesProduto) == false, "O bot�o 'Sincronizar' do produto est� ativo");
                // Verifica que o bot�o de edi��o interno do produto existe e funciona
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.True(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto n�o foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B32_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusNaoCurado()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoNaoCurado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                await ForcarClicarElemento(driver, botaoAcoes);
                await AguardarElemento(driver, menuDoBotaoAcoes);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEditarProduto) == true && await ElementoEstaAtivo(driver, botaoEditarProduto) == true, "O bot�o 'Editar' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoRemoverProduto) == true && await ElementoEstaAtivo(driver, botaoRemoverProduto) == true, "O bot�o 'Remover' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoDesabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoDesabilitarProduto) == false, "O bot�o 'Desabilitar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoHabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoHabilitarProduto) == false, "O bot�o 'Habilitar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, menuDoBotaoAcoes) == true && await ElementoEstaAtivo(driver, botaoSincronizarAlteracoesProduto) == false, "O bot�o 'Sincronizar' do produto est� ativo");
                // Verifica que o bot�o de edi��o interno do produto existe e funciona
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.True(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto n�o foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B33_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusErroNaIntegracao()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoErroNaIntegracao);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                await ForcarClicarElemento(driver, botaoAcoes);
                await AguardarElemento(driver, menuDoBotaoAcoes);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEditarProduto) == true && await ElementoEstaAtivo(driver, botaoEditarProduto) == true, "O bot�o 'Editar' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoRemoverProduto) == true && await ElementoEstaAtivo(driver, botaoRemoverProduto) == false, "O bot�o 'Remover' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoDesabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoDesabilitarProduto) == true, "O bot�o 'Desabilitar' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoHabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoHabilitarProduto) == false, "O bot�o 'Habilitar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, menuDoBotaoAcoes) == true && await ElementoEstaAtivo(driver, botaoSincronizarAlteracoesProduto) == false, "O bot�o 'Sincronizar' do produto est� ativo");
                // Verifica que o bot�o de edi��o interno do produto existe e funciona
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.True(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto n�o foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B34_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusAtivo()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatus(driver, itemStatusProdutoAtivo);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                await ForcarClicarElemento(driver, botaoAcoes);
                await AguardarElemento(driver, menuDoBotaoAcoes);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEditarProduto) == true && await ElementoEstaAtivo(driver, botaoEditarProduto) == true, "O bot�o 'Editar' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoRemoverProduto) == true && await ElementoEstaAtivo(driver, botaoRemoverProduto) == false, "O bot�o 'Remover' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoDesabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoDesabilitarProduto) == true, "O bot�o 'Desabilitar' do produto n�o est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoHabilitarProduto) == true && await ElementoEstaAtivo(driver, botaoHabilitarProduto) == false, "O bot�o 'Habilitar' do produto est� ativo");
                Assert.IsTrue(await ElementoEstaVisivel(driver, menuDoBotaoAcoes) == true && await ElementoEstaAtivo(driver, botaoSincronizarAlteracoesProduto) == false, "O bot�o 'Sincronizar' do produto est� ativo");
                // Verifica que o bot�o de edi��o interno do produto existe e funciona
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.True(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto n�o foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B35_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusAtializacaoSincronizando()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoSincronizando);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                Assert.IsFalse(await ElementoEstaVisivel(driver, botaoAcoes), "O bot�o de a��es est� sendo exibido");
                await ClicarElemento(driver, botaoVisualizarProduto);
                // Verifica que o bot�o de edi��o interno do produto n�o existe
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.IsFalse(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto foi localizado, contudo, neste cen�rio ele n�o deveria estar dispon�vel");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B36_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusAtializacaoErroNaIntegracao()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoErroNaIntegracao);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoAcoes), "O bot�o de a��es n�o est� sendo exibido");
                // Verifica que o bot�o de edi��o interno do produto existe e funciona
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.True(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto n�o foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B37_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusAtializacaoSincronizado()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosAVenda(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoSincronizado);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                //Verifica que o bot�o de Edi��o do produto existe
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoAcoes), "O bot�o de a��es n�o est� sendo exibido");
                // Verifica que o bot�o de edi��o interno do produto existe e funciona
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, labelNomeDoProduto);
                Assert.True(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto n�o foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        // O status predominante est� sendo o sincronizando, ent�o acarreta em um bug. Perguntar pro Alan
        //[Parallelizable]
        //[Test]
        //public async Task B38_M_AVenda_ValidarBotoesDeAcaoEEdicaoInternaNoStatusAtializacaoNaoSincronizado()
        //{
        //    var playwright = await Playwright.CreateAsync();
        //    var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
        //    var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
        //    var driver = await context.NewPageAsync();
        //    await driver.SetViewportSizeAsync(1920, 1080);
        //    driver.SetDefaultNavigationTimeout(200000);
        //    try
        //    {
        //        await _funcoesLogin.LoginIT4MasterAsync(driver);
        //        await AbrirPaginaProdutosAVenda(driver);
        //        await AbrirFiltros(driver);
        //        await PreencherPesquisaProdutoPorStatusAtualizacao(driver, itemStatusProdutoNaoSincronizado);
        //        await BuscarNoFiltro(driver);
        //        await AguardarElemento(driver, botaoVisualizarProduto);
        //        //Verifica que o bot�o de Edi��o do produto existe
        //        Assert.IsTrue(await ElementoEstaVisivel(driver, botaoAcoes), "O bot�o de a��es n�o est� sendo exibido");
        //        // Verifica que o bot�o de edi��o interno do produto existe e funciona
        //        await ClicarElemento(driver, botaoVisualizarProduto);
        //        await AguardarElemento(driver, labelNomeDoProduto);
        //        Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEditarInternoProduto), "O bot�o Edi��o Interna do produto n�o foi localizado, contudo, neste cen�rio ele n�o deveria estar dispon�vel");
        //    }
        //    catch (Exception e)
        //    {
        //        await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
        //        GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
        //        Assert.IsTrue(false);
        //    }
        //    finally
        //    {
        //        await driver.CloseAsync();
        //        playwright.Dispose();
        //    }
        //}

        [Parallelizable]
        [Test]
        public async Task B38_M_AVenda_AgregarUmAtributoAoProduto()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                // Cria produto e sku Dispon�vel, A Venda, al�m de atribuir uma imagem para o sku
                _funcoesBancoIt4360Stg.CriarProduto("TesteAutomatizadoAVenda", "TesteAutoAVenda", "TesteAutoAVendaSku", "TesteAutoAVendaEan");
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                // Aguarda o produto A Venda terminar o cadastro para que a chave do redis seja resgatada
                string idProdutoAVenda = _conexaoBancoRedis.GetData<string>("IdProdutoAVenda");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (idProdutoAVenda == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    idProdutoAVenda = _conexaoBancoRedis.GetData<string>("IdProdutoAVenda");
                    duracao = timer.ElapsedMilliseconds;
                }
                // Recebe a chave do redis e navega at� a url desejada
                await AbrirPaginaProdutosAVenda(driver);
                await driver.GotoAsync("https://stg2.it4360.com.br/products/sales/" + idProdutoAVenda + "/edit");
                // Confer�ncia dos campos obrigat�rios de um atributo
                await ClicarElemento(driver, botaoMenuAtributos);
                await AguardarElemento(driver, campoAtributoSelecionavelProdutoCategSaude);
                await ClicarElemento(driver, botaoAtualizar);
                await AguardarElemento(driver, mensagemCampoObrigatorioSelecionavelProduto);
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioSelecionavelProduto), "A label de obrigatoriedade n�o foi evidenciada para o campo 'Selecionavel'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioCampoAbertoProduto), "A label de obrigatoriedade n�o foi evidenciada para o campo 'CampoAberto'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioMultivaloradoProduto), "A label de obrigatoriedade n�o foi evidenciada para o campo 'Multivalorado'");
                // Preenchimento dos atributos
                await PreencherCampoESelecionarItem(driver, campoAtributoSelecionavelProdutoCategSaude, "ValorAtributoSelecionavel1_3", itemValorAtributoSelecionavel1_3ProdutoCategSaude);
                await PreencherCampoESelecionarItem(driver, campoAtributoMultivaloradoProdutoCategSaude, "ValorAtributoMultivalorado2", itemValorAtributoMultivalorado2ProdutoCategSaude);
                await PreencherCampo(driver, campoAtributoCampoAbertoProdutoCategSaude, "ValorAtributoCampoAberto");
                await ClicarElemento(driver, botaoAtualizar);
                await AguardarElemento(driver, notificacaoSucessoEnvioProdutoCanal);
                await driver.ReloadAsync();
                // Confere os campos rec�m preenchidos
                await ClicarElemento(driver, botaoMenuAtributos);
                Assert.AreEqual("ValorAtributoSelecionavel1_3", await RetornaTextoElemento(driver, campoVerificacaoAtributoSelecionavelCategSaude), "O valor atributo encontrado n�o corresponde � 'ValorAtributoSelecionavel1_3'");
                Assert.AreEqual("ValorAtributoMultivalorado2", await RetornaTextoElemento(driver, campoVerificacaoAtributoMultivaloradoCategSaude), "O valor atributo encontrado n�o corresponde � 'ValorAtributoMultivalorado2'");
                Assert.AreEqual("ValorAtributoCampoAberto", await RetornaValorElemento(driver, campoAtributoCampoAbertoProdutoCategSaude), "O valor atributo encontrado n�o corresponde � 'A�o'");
                string urlSkuSemAtributo = driver.Url;
                _conexaoBancoRedis.SetData("UrlSkuSemAtributo", urlSkuSemAtributo, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("TesteAutoAVenda", "TesteAutoAVendaSku");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("TesteAutoAVenda", "TesteAutoAVendaSku", "DescricaoImagemSku");
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B39_M_AVenda_AgregarUmAtributoAoSkuDoProduto()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                // Aguarda a thread que agrega os atributos do produto terminar (AgregarUmAtributoAoProduto) para que a chave do redis seja resgatada
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                string urlSkuSemAtributo = _conexaoBancoRedis.GetData<string>("UrlSkuSemAtributo");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (urlSkuSemAtributo == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    urlSkuSemAtributo = _conexaoBancoRedis.GetData<string>("UrlSkuSemAtributo");
                    duracao = timer.ElapsedMilliseconds;
                }
                // Recebe a chave do redis e navega at� a url desejada
                await AbrirPaginaProdutosAVenda(driver);
                await driver.GotoAsync(urlSkuSemAtributo);
                await AguardarElemento(driver, botaoEditarSkuTesteAutoAVendaDescricao);
                // Confer�ncia dos campos obrigat�rios de um atributo
                await ForcarClicarElemento(driver, botaoEditarSkuTesteAutoAVendaDescricao);
                await ClicarElemento(driver, botaoAtualizarSku);
                await AguardarElemento(driver, mensagemCampoObrigatorioSelecionavelSku);
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioSelecionavelSku), "A label de obrigatoriedade n�o foi evidenciada para o campo 'Selecionavel'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioCampoAbertoSku), "A label de obrigatoriedade n�o foi evidenciada para o campo 'CampoAberto'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioMultivaloradoSku), "A label de obrigatoriedade n�o foi evidenciada para o campo 'Multivalorado'");
                // Preenchimento dos atributos
                await PreencherCampoESelecionarItem(driver, campoAtributoSelecionavelSku, "ValorAtributoSelecionavel1_3", itemValorAtributoSelecionavel1_3Sku);
                await PreencherCampoESelecionarItem(driver, campoAtributoMultivaloradoSku, "ValorAtributoMultivalorado2", itemValorAtributoMultivalorado2Sku);
                await PreencherCampo(driver, campoAtributoCampoAbertoSku, "ValorAtributoCampoAberto");
                await ClicarElemento(driver, botaoAtualizarSku);
                await AguardarElemento(driver, notificacaoSucessoAtualiza��oSku);
                // Atualiza a p�gina e verifica se todos os campos est�o de acordo com o editado
                await driver.ReloadAsync();
                await AguardarElemento(driver, botaoEditarSkuTesteAutoAVendaDescricao);
                await ClicarElemento(driver, botaoEditarSkuTesteAutoAVendaDescricao);
                Assert.AreEqual("ValorAtributoCampoAberto", await RetornaValorElemento(driver, campoAtributoCampoAbertoSku), "O valor atributo encontrado n�o corresponde � 'ValorAtributoCampoAberto'");
                Assert.AreEqual("ValorAtributoMultivalorado2", await RetornaTextoElemento(driver, campoAtributoMultivaloradoSkuVerificacao), "O valor atributo encontrado n�o corresponde � 'ValorAtributoMultivalorado2'");
                Assert.AreEqual("ValorAtributoSelecionavel1_3", await RetornaTextoElemento(driver, campoAtributoSelecionavelSkuVerificacao), "O valor atributo encontrado n�o corresponde � 'ValorAtributoSelecionavel1_3'");
                string urlProdutoParaEdicao = driver.Url;
                _conexaoBancoRedis.SetData("UrlProdutoParaEdicao", urlProdutoParaEdicao, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("TesteAutoAVenda", "TesteAutoAVendaSku");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("TesteAutoAVenda", "TesteAutoAVendaSku", "DescricaoImagemSku");
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B40_M_AVenda_EditarUmProdutoNaoCurado()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                // Aguarda a thread que agrega os atributos do SKU terminar (AgregarUmAtributoAoSkuDoProduto) para que a chave do redis seja resgatada
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                string urlProdutoParaEdicao = _conexaoBancoRedis.GetData<string>("UrlProdutoParaEdicao");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (urlProdutoParaEdicao == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    urlProdutoParaEdicao = _conexaoBancoRedis.GetData<string>("UrlProdutoParaEdicao");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirPaginaProdutosAVenda(driver);
                await driver.GotoAsync(urlProdutoParaEdicao);
                await LimparTextoEmEdicaoDoProduto(driver);
                // Atribui os novos valores aos campos
                await PreencherCampo(driver, campoNomeCadastroProduto, "TesteAutoNomeEditado");
                await PreencherCampo(driver, campoDescricaoLongaCadastroProduto, "TesteAutoDescricaoLongaEdicao");
                await PreencherCampo(driver, campoDescricaoCurtaCadastroProduto, "TesteAutoDescricaoCurtaEdicao");
                await PreencherCampo(driver, campoMetaTagsCadastroProduto, "MetaTagEditada");
                await PreencherCampo(driver, campoPalavraChaveCadastroProduto, "PalavraChaveEditada");
                await PreencherCampoESelecionarItem(driver, campoMarcaCadastroProduto, "leonardo", itemLeonardoMarcaProduto);
                await PreencherCampoESelecionarItem(driver, campoCategoriaCadastroProduto, "Eletronicos", itemEletronicosCategoriaProduto);
                await ClicarElemento(driver, botaoMenuAtributos);
                await PreencherCampoESelecionarItem(driver, campoAtributoSelecionavelProdutoCategEletronicos, "ValorAtributoSelecionavel1_3", itemValorAtributoSelecionavel1_3ProdutoCategEletronicos);
                await PreencherCampoESelecionarItem(driver, campoAtributoMultivaloradoProdutoCategEletronicos, "ValorAtributoMultivalorado1_2", itemValorAtributoMultivalorado1_2ProdutoCategEletronicos);
                await PreencherCampo(driver, campoAtributoCampoAbertoProdutoCategEletronicos, "ValorAtributoCampoAbertoEditado");
                await ClicarElemento(driver, botaoAtualizar);
                await AguardarElemento(driver, notificacaoSucessoAtualiza��oProduto);
                // Verifica os campos rec�m editados
                await driver.ReloadAsync();
                await AguardarElemento(driver, botaoEditarSkuTesteAutoAVendaDescricao);
                Assert.AreEqual("TesteAutoNomeEditado", await RetornaValorElemento(driver, campoNomeCadastroProduto), "O valor do nome encontrado n�o corresponde � 'TesteAutoNomeEditado'");
                Assert.AreEqual("TesteAutoDescricaoLongaEdicao", await RetornaValorElemento(driver, campoDescricaoLongaCadastroProduto), "O valor da Descri��o Longa encontrada n�o corresponde � 'TesteAutoDescricaoLongaEdicao'");
                Assert.AreEqual("TesteAutoDescricaoCurtaEdicao", await RetornaValorElemento(driver, campoDescricaoCurtaCadastroProduto), "O valor da Descri��o Curta encontrada n�o corresponde � 'TesteAutoDescricaoCurtaEdicao'");
                Assert.AreEqual("MetaTagEditada", await RetornaTextoElemento(driver, campoVerificacaoMetaTagsEdicaoProduto), "O valor da Meta Tag encontrada n�o corresponde � 'MetaTagEditada'");
                Assert.AreEqual("PalavraChaveEditada", await RetornaTextoElemento(driver, campoVerificacaoPalavraChaveEdicaoProduto), "O valor da Palavra Chave encontrada n�o corresponde � 'PalavraChaveEditada'");
                await ClicarElemento(driver, botaoMenuAtributos);
                await AguardarElemento(driver, campoAtributoCampoAbertoProdutoCategEletronicos);
                Assert.AreEqual("ValorAtributoCampoAbertoEditado", await RetornaValorElemento(driver, campoAtributoCampoAbertoProdutoCategEletronicos), "O valor atributo encontrado n�o corresponde � 'ValorAtributoCampoAberto'");
                Assert.AreEqual("ValorAtributoMultivalorado1_2", await RetornaTextoElemento(driver, campoVerificacaoAtributoMultivaloradoCategEletronicos), "O valor atributo encontrado n�o corresponde � 'ValorAtributoMultivalorado2'");
                Assert.AreEqual("ValorAtributoSelecionavel1_3", await RetornaTextoElemento(driver, campoVerificacaoAtributoSelecionavelCategEletronicos), "O valor atributo encontrado n�o corresponde � 'ValorAtributoSelecionavel1_3'");
                string urlSkuParaEdicao = driver.Url;
                _conexaoBancoRedis.SetData("UrlSkuParaEdicao", urlSkuParaEdicao, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("TesteAutoAVenda", "TesteAutoAVendaSku");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("TesteAutoAVenda", "TesteAutoAVendaSku", "DescricaoImagemSku");
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B41_M_AVenda_EditarUmSkuDoProdutoNaoCurado()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                // Aguarda a thread que edita os campos do produto terminar (EditarUmProdutoNaoCurado) para que a chave do redis seja resgatada
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                string urlSkuParaEdicao = _conexaoBancoRedis.GetData<string>("UrlSkuParaEdicao");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (urlSkuParaEdicao == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    urlSkuParaEdicao = _conexaoBancoRedis.GetData<string>("UrlSkuParaEdicao");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirPaginaProdutosAVenda(driver);
                await driver.GotoAsync(urlSkuParaEdicao);
                await AguardarElemento(driver, botaoEditarSkuTesteAutoAVendaDescricao);
                await ForcarClicarElemento(driver, botaoEditarSkuTesteAutoAVendaDescricao);
                await LimparTextoEmEdicaoDoSku(driver);
                // Atribui os novos valores aos campos
                await PreencherCampo(driver, campoDescricaoCadastroSku, "TesteAutoDescEditado");
                await PreencherCampo(driver, campoPrecoDeCadastroSku, "40000");
                await PreencherCampo(driver, campoPrecoPorCadastroSku, "40000");
                await PreencherCampo(driver, campoAlturaCadastroSku, "2");
                await PreencherCampo(driver, campoAlturaRealCadastroSku, "2");
                await PreencherCampo(driver, campoComprimentoCadastroSku, "2");
                await PreencherCampo(driver, campoComprimentoRealCadastroSku, "2");
                await PreencherCampo(driver, campoPesoCadastroSku, "2");
                await PreencherCampo(driver, campoPesoRealCadastroSku, "2");
                await PreencherCampo(driver, campoPesoCubicoCadastroSku, "2");
                await PreencherCampo(driver, campoLarguraCadastroSku, "2");
                await PreencherCampo(driver, campoLarguraRealCadastroSku, "2");
                // Atribuindo valores aos Atributos do Sku
                await PreencherCampoESelecionarItem(driver, campoAtributoSelecionavelSkuCategEletronicos, "ValorAtributoSelecionavel1_3", itemValorAtributoSelecionavel1_3Sku);
                await PreencherCampoESelecionarItem(driver, campoAtributoMultivaloradoSkuCategEletronicos, "ValorAtributoMultivalorado1_2", itemValorAtributoMultivalorado1_2Sku);
                await PreencherCampo(driver, campoAtributoCampoAbertoSkuCategEletronicos, "ValorAtributoCampoAbertoSkuEditado");
                await ClicarElemento(driver, botaoAtualizarSku);
                await AguardarElemento(driver, notificacaoSucessoAtualiza��oSku);
                //// Verifica os campos rec�m editados
                await driver.ReloadAsync();
                await AguardarElemento(driver, botaoEditarSkuTesteAutoDescEditado);
                await ClicarElemento(driver, botaoEditarSkuTesteAutoDescEditado);
                Assert.AreEqual("TesteAutoDescEditado", await RetornaValorElemento(driver, campoDescricaoCadastroSku), "O valor da descri��o encontrada n�o corresponde � 'TesteAutoDescEditado'");
                Assert.AreEqual("R$400,00", await RetornaValorElemento(driver, campoPrecoDeCadastroSku), "O valor do Pre�o De encontrado n�o corresponde � 'R$ 400,00'");
                Assert.AreEqual("R$400,00", await RetornaValorElemento(driver, campoPrecoPorCadastroSku), "O valor do Pre�o Por encontrado n�o corresponde � 'R$ 400,00'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoAlturaCadastroSku), "O valor da Altura encontrada n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoAlturaRealCadastroSku), "O valor da Altura Real encontrada n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoComprimentoCadastroSku), "O valor do Comprimento encontrado n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoComprimentoRealCadastroSku), "O valor do Comprimento Real encontrado n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoPesoCadastroSku), "O valor do Peso encontrado n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoPesoRealCadastroSku), "O valor do Peso Real encontrado n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoPesoCubicoCadastroSku), "O valor do Peso C�bico encontrado n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoLarguraCadastroSku), "O valor da Largura encontrada n�o corresponde � '2'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoLarguraRealCadastroSku), "O valor da Largura Real encontrada n�o corresponde � '2'");
                Assert.AreEqual("ValorAtributoCampoAbertoSkuEditado", await RetornaValorElemento(driver, campoAtributoCampoAbertoSkuCategEletronicos), "O valor atributo encontrado n�o corresponde � 'ValorAtributoCampoAberto'");
                Assert.AreEqual("ValorAtributoMultivalorado1_2", await RetornaTextoElemento(driver, campoAtributoMultivaloradoSkuCategEletronicosVerificacao), "O valor atributo encontrado n�o corresponde � 'ValorAtributoMultivalorado2'");
                Assert.AreEqual("ValorAtributoSelecionavel1_3", await RetornaTextoElemento(driver, campoAtributoSelecionavelSkuCategEletronicosVerificacao), "O valor atributo encontrado n�o corresponde � 'ValorAtributoSelecionavel1_3'");
                string urlParaCurarProduto = driver.Url;
                _conexaoBancoRedis.SetData("UrlParaCurarProduto", urlParaCurarProduto, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("TesteAutoAVenda", "TesteAutoAVendaSku");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("TesteAutoAVenda", "TesteAutoAVendaSku", "DescricaoImagemSku");
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task B42_M_Avenda_MarcarProdutoComoCurado()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                // Aguarda a thread que edita os campos do sku terminar (EditarUmSkuDoProdutoNaoCurado) para que a chave do redis seja resgatada
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                string urlParaCurarProduto = _conexaoBancoRedis.GetData<string>("UrlParaCurarProduto");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (urlParaCurarProduto == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    urlParaCurarProduto = _conexaoBancoRedis.GetData<string>("UrlParaCurarProduto");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirPaginaProdutosAVenda(driver);
                // Marca o produto como curado
                await driver.GotoAsync(urlParaCurarProduto);
                await AguardarElemento(driver, checkBoxCurado);
                await ClicarElemento(driver, checkBoxCurado);
                await ClicarElemento(driver, botaoAtualizar);
                await AguardarElemento(driver, notificacaoSucessoAtualiza��oProduto);
                string idProdutoCurado = urlParaCurarProduto.Substring(42, 36);
                await driver.GotoAsync("https://stg2.it4360.com.br/products/sales?idsProducts[]=" + idProdutoCurado);
                string statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(1, (int)PosicaoCampoGridMaster.Status));
                Assert.AreEqual("curado", statusProduto.ToLower(), "O produto encontrado n�o possui o status desejado. Status encontrado: " + statusProduto);
                //_conexaoBancoRedis.SetData("UrlProdutoCurado", urlProdutoCurado, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }
    }
}