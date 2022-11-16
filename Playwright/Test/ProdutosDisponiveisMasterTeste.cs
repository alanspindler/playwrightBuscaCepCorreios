using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Data.SqlClient;
using Dapper;
using System.Threading;
using System.Threading.Tasks;
using Portal.Entities;
using System.Diagnostics;

[assembly: LevelOfParallelism(5)]

namespace PlaywrightAutomacao
{
    public class ProdutosDisponiveisMasterTeste : FuncoesProdutosDisponiveis
    {
        private FuncoesLogin _funcoesLogin = new FuncoesLogin();
        private ConexaoBancoRedis _conexaoBancoRedis = new ConexaoBancoRedis();
        private FuncoesBancoIt4360Stg _funcoesBancoIt4360Stg = new FuncoesBancoIt4360Stg();
        private SqlConnection _connection;

        public ProdutosDisponiveisMasterTeste(FuncoesLogin funcoesLogin, ConexaoBancoRedis conexaoBancoRedis)
        {
            _funcoesLogin = funcoesLogin;
            _conexaoBancoRedis = conexaoBancoRedis;
        }

        public ProdutosDisponiveisMasterTeste() { }

        [OneTimeSetUp]
        protected void OneTimeSetUp()
        {
            _funcoesBancoIt4360Stg.ExcluirProdutoAVenda();
            _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel();
            _conexaoBancoRedis.DeleteData("ProdutoParaInserirSku");
            _conexaoBancoRedis.DeleteData("CodigoProduto");
            _conexaoBancoRedis.DeleteData("CodigoProdutoParaExclusao");
            _conexaoBancoRedis.DeleteData("ProdutoParaEdicao");
            _conexaoBancoRedis.DeleteData("ProdutoEditado");
            _conexaoBancoRedis.DeleteData("ProdutoESkuEditado");
        }

        [OneTimeTearDown]
        protected void OneTimeTearDown()
        {

        }

        [Parallelizable]
        [Test]
        public async Task A05_M_Disponiveis_PesquisarProdutoPorCodigo()
        {
            // Criação e configuração do navegador
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
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCodigo(driver, "e0fb0c84-caf0-4a0c-b01b-dbc5f8b83c03");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                    Assert.AreEqual("e0fb0c84-caf0-4a0c-b01b-dbc5f8b83c03", codigoProduto, "O produto encontrado não corresponde ao código desejado. Código encontrado: " + codigoProduto);
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
        public async Task A16_M_Disponiveis_PesquisarProdutosPorMultiplosCodigos()
        {
            // Criação e configuração do navegador
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
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCodigo(driver, "e0fb0c84-caf0-4a0c-b01b-dbc5f8b83c03", "8bc67c9a-c7fa-4e3d-89b4-2f64322e4ff7");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                    Assert.IsTrue(codigoProduto.ToLower() == "e0fb0c84-caf0-4a0c-b01b-dbc5f8b83c03" || codigoProduto.ToLower() == "8bc67c9a-c7fa-4e3d-89b4-2f64322e4ff7", "O produto encontrado não contem nenhum dos códigos listados. Código encontrado: " + codigoProduto);
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
        public async Task A09_M_Disponiveis_PesquisarProdutoPorReferencia()
        {
            // Criação e configuração do navegador
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
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorReferencia(driver, "Produto Alan 3");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                _connection = new SqlConnection("Data Source=stg360.database.windows.net;Initial Catalog=stg360;Integrated Security=False;User ID=administrador;Password=Kemed@#100;MultipleActiveResultSets=true");
                using (var connection = _connection)
                {
                    foreach (int linha in linhasTabelaGridPrincipal)
                    {
                        codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                        var query = $"select Id, [Name], RefProduct from Product where Id = '{codigoProduto}'";
                        var produto = connection.QueryFirstOrDefault<Product>(query);
                        Assert.AreEqual("Produto Alan 3", produto.RefProduct, "O produto encontrado não contem a referência desejada. Referência encontrada: " + produto.RefProduct);
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
        public async Task A23_M_Disponiveis_PesquisarProdutosPorMultiplasReferencias()
        {
            // Criação e configuração do navegador
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
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorReferencia(driver, "Produto Alan 3", "102030teste10");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                _connection = new SqlConnection("Data Source=stg360.database.windows.net;Initial Catalog=stg360;Integrated Security=False;User ID=administrador;Password=Kemed@#100;MultipleActiveResultSets=true");
                using (var connection = _connection)
                {
                    foreach (int linha in linhasTabelaGridPrincipal)
                    {
                        codigoProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Codigo));
                        var query = $"select Id, [Name], RefProduct from Product where Id = '{codigoProduto}'";
                        var produto = connection.QueryFirstOrDefault<Product>(query);
                        Assert.IsTrue(produto.RefProduct == "Produto Alan 3" || produto.RefProduct == "102030teste10", "O produto encontrado não contem a nenhuma das referências listadas. Referência encontrada: " + produto.RefProduct);
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
        public async Task A07_M_Disponiveis_PesquisarProdutoPorNome()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string nomeProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorNome(driver, "Calça Moletom Cinza");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Nome));
                    Assert.IsTrue(nomeProduto.ToLower().Contains("calça moletom cinza"), "O produto encontrado não possui o nome desejado. O nome encontrado: " + nomeProduto);
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
        public async Task A08_M_Disponiveis_PesquisarProdutoPorNomeParcial()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string nomeProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorNome(driver, "Calça");
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Nome));
                    Assert.IsTrue(nomeProduto.ToLower().Contains("calça"), "O produto encontrado não possui o nome desejado. O nome encontrado: " + nomeProduto);
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
        public async Task A06_M_Disponiveis_PesquisarProdutoPorMarca()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string marcaProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado não possui a marca desejada. Marca encontrada: " + marcaProduto);
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
        public async Task A13_M_Disponiveis_PesquisarProdutosPorMultiplasMarcas()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string marcaProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto, "leonardo", itemLeonardoMarcaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.IsTrue(marcaProduto.ToLower() == "it4 solution" || marcaProduto.ToLower() == "leonardo", "O produto encontrado não contem a nenhuma das marcas listadas. Marca encontrada: " + marcaProduto);
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
        public async Task A04_M_Disponiveis_PesquisarProdutoPorCategoria()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string categoriaProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCategoria(driver, "Jóias", itemJoiasCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.IsTrue(categoriaProduto.ToLower() == "jóias", "O produto encontrado não possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
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
        public async Task A12_M_Disponiveis_PesquisarProdutosPorMultiplasCategorias()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string categoriaProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCategoria(driver, "Jóias", itemJoiasCategoriaProduto, "Roupas", itemRoupasCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.IsTrue(categoriaProduto.ToLower() == "jóias" || categoriaProduto.ToLower() == "roupas",
                        "O produto encontrado não contem nenhuma das categorias listadas. Categoria encontrada: " + categoriaProduto);
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
        public async Task A21_M_Disponiveis_PesquisarProdutosSemCanal()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string produtoSemCanal;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoSemCanal(driver);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    produtoSemCanal = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canais));
                    Assert.IsTrue(produtoSemCanal == "", "O registro não deveria possuir um canal. Canal encontrado: " + produtoSemCanal);
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
        public async Task A22_M_Disponiveis_EnviarSomenteUmProdutoParaOCanal()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string nomeProduto;
                string canalProduto;
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                // Cadastro de produto
                await AbrirPaginaProdutosDisponiveis(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await ClicarElemento(driver, botaoCadastrarProduto);
                await PreencherCamposCadastroProduto(driver,
                    "TesteAutomatizadoParaEnvioDeUmProduto", // Nome
                    "RefTesteParaEnvioDeUmProduto", // Referência
                    "IT4 Solution", // Marca
                    itemIt4SolutionMarcaCriacaoProduto,// Item marca
                    "Camisetas", // Categoria
                    itemCamisetasCategoriaCriacaoProduto, // Item categoria
                    "Descrição longa para teste automatizado", // Descrição longa
                    "Descrição curta para teste automatizado", // Descrição curta
                    "TesteAutomatizado", // Metatags
                    "TesteAutomatizado"); // Palavras chave
                await ClicarElemento(driver, botaoCadastrar);
                await AguardarElemento(driver, notificacaoSucessoCriacaoProduto);
                await AguardarElemento(driver, botaoAdicionarSku);
                await ClicarElemento(driver, botaoAdicionarSku);
                // Cadastro SKU
                await PreencherCamposCadastroSku(driver,
                    "ReferenciaSkuTesteEnvioDeUmProduto", // Referencia sku
                    "EanSkuTesteEnvioDeUmProduto", // Ean sku
                    "2000", // Preco de
                    "2000", // Preço por
                    "Descricao sku para teste automatizado", // Descricao sku
                    "1", // Peso sku
                    "1", // Altura sku
                    "1", // Largura sku
                    "1", // Comprimento sku
                    "1", // Peso sku real
                    "1", // Peso sku cubico
                    "1", // Altura sku real
                    "1", // Largura sku real
                    "1"); // Comprimento sku real
                // Adicionando imagem ao SKU
                await ClicarElemento(driver, botaoAdicionarImagemPorUrl);
                await PreencherCamposDoCadastroDeImagemPorUrlDoSku(driver);
                await ClicarElemento(driver, botaoOk);
                await AguardarElementoNaoVisivel(driver, botaoOk);
                // Filtra um produto pelo nome, seleciona a checkbox do produto e envia somente ele ao canal
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?name=TesteAutomatizadoParaEnvioDeUmProduto&onlyProductsWithoutChannel=false");
                await AguardarElemento(driver, botaoVisualizarProduto);
                await ClicarElemento(driver, botaoCheckBox);
                await ClicarElemento(driver, botaoEnviarSelecionadosAoCanal);
                await ClicarElemento(driver, listaDeCanais);
                await ClicarElemento(driver, itemVtexListaDeCanais);
                await ClicarElemento(driver, botaoConfirmar);
                // Verifica se o produto existe no submenu A Venda
                await driver.GotoAsync("https://stg2.it4360.com.br/products/sales?name=TesteAutomatizadoParaEnvioDeUmProduto");
                await AguardarElemento(driver, botaoVisualizarProduto);

                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)FuncoesProdutosAVenda.PosicaoCampoGridMaster.Nome));
                    Assert.AreEqual("testeautomatizadoparaenviodeumproduto", nomeProduto.ToLower(), "O produto encontrado não possui o nome desejado. Nome encontrado: " + nomeProduto);
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)FuncoesProdutosAVenda.PosicaoCampoGridMaster.Canal));
                    Assert.AreEqual("it4solution vtex", canalProduto.ToLower(), "O produto encontrado não possui o canal desejado. Canal encontrado: " + canalProduto);
                }
                // Tenta excluir o produto do submenu Disponíveis, contudo, essa ação não deve estar disponível, pois existe um produto no canal
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?name=TesteAutomatizadoParaEnvioDeUmProduto&onlyProductsWithoutChannel=false");
                await AguardarElemento(driver, botaoVisualizarProduto);
                await ClicarElemento(driver, botaoAcoes);
                await AguardarElemento(driver, menuDoBotaoAcoes);
                Assert.IsFalse(await ElementoEstaVisivel(driver, botaoRemover), "O botão 'Excluir' foi localizado");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("RefTesteParaEnvioDeUmProduto", "ReferenciaSkuTesteEnvioDeUmProduto");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteParaEnvioDeUmProduto", "ReferenciaSkuTesteEnvioDeUmProduto", "DescricaoImagemSku");
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
        public async Task A03_M_Disponiveis_PesquisarProdutoPorCanal()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string canalProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCanal(driver, "IT4Solution VTEX", itemIt4SolutionVtexCanalProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canais));
                    Assert.AreEqual("it4solution vtex", canalProduto.ToLower(), "O produto encontrado não possui o canal desejado. Canal encontrado: " + canalProduto);
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
        public async Task A15_M_Disponiveis_PesquisarProdutosPorMultiplosCanais()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string canalProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorCanal(driver, "IT4Solution VTEX", itemIt4SolutionVtexCanalProduto, "IT4Solution Magento", itemIt4MagentoCanalProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canais));
                    // O front retorna a string grudada, por isso utilizamos o Contains
                    Assert.IsTrue(canalProduto.ToLower().Contains("it4solution vtex") || canalProduto.ToLower().Contains("it4solution magento"),
                       "O produto encontrado não contem nenhum dos canais listados. Canal encontrado: " + canalProduto);
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
        public async Task A20_M_Disponiveis_PesquisarProdutosPorStatusHabilitado()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaStatusProdutoHabilitado(driver);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("habilitado", statusProduto.ToLower(), "O produto encontrado não possui o status desejado. Status encontrado: " + statusProduto);
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
        public async Task A19_M_Disponiveis_PesquisarProdutosPorStatusDesabilitado()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string statusProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaStatusProdutoDesabilitado(driver);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("desabilitado", statusProduto.ToLower(), "O produto encontrado não possui o status desejado. Status encontrado: " + statusProduto);
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
        public async Task A24_M_Disponiveis_PesquisarProdutosPorNomeCategoria()
        {
            // Criação e configuração do navegador
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            try
            {
                string nomeProduto;
                string categoriaProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorNome(driver, "Calça");
                await PreencherPesquisaProdutoPorCategoria(driver, "Jóias", itemJoiasCategoriaProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Nome));
                    Assert.IsTrue(nomeProduto.ToLower().Contains("calça"), "O produto encontrado não possui o nome desejado. O nome encontrado: " + nomeProduto);
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("jóias", categoriaProduto.ToLower(), "O produto encontrado não possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
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
        public async Task A14_M_Disponiveis_PesquisarProdutosPorMarcaCategoriaCanal()
        {
            // Criação e configuração do navegador
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
                string canalProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await PreencherPesquisaProdutoPorCategoria(driver, "Roupas", itemRoupasCategoriaProduto);
                await PreencherPesquisaProdutoPorCanal(driver, "IT4solution VTex", itemIt4SolutionVtexCanalProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado não possui a marca desejada. Marca encontrada: " + marcaProduto);
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("roupas", categoriaProduto.ToLower(), "O produto encontrado não possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canais));
                    Assert.AreEqual("it4solution vtex", canalProduto.ToLower(), "O produto encontrado não possui o canal desejado. Canal encontrado: " + canalProduto);
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
        public async Task A26_M_Disponiveis_PesquisarProdutosPorMarcaCategoriaCanalStatus()
        {
            // Criação e configuração do navegador
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
                string canalProduto;
                string statusProduto;
                // Efetuando login e abrindo os filtros
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AbrirFiltros(driver);
                await PreencherPesquisaStatusProdutoHabilitado(driver);
                await PreencherPesquisaProdutoPorMarca(driver, "IT4 Solution", itemIt4SolutionMarcaProduto);
                await PreencherPesquisaProdutoPorCategoria(driver, "Roupas", itemRoupasCategoriaProduto);
                await PreencherPesquisaProdutoPorCanal(driver, "IT4solution VTex", itemIt4SolutionVtexCanalProduto);
                await BuscarNoFiltro(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    marcaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Marca));
                    Assert.AreEqual("it4 solution", marcaProduto.ToLower(), "O produto encontrado não possui a marca desejada. Marca encontrada: " + marcaProduto);
                    categoriaProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Categoria));
                    Assert.AreEqual("roupas", categoriaProduto.ToLower(), "O produto encontrado não possui a categoria desejada. Categoria encontrada: " + categoriaProduto);
                    canalProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Canais));
                    Assert.AreEqual("it4solution vtex", canalProduto.ToLower(), "O produto encontrado não possui o canal desejado. Canal encontrado: " + canalProduto);
                    statusProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)PosicaoCampoGridMaster.Status));
                    Assert.AreEqual("habilitado", statusProduto.ToLower(), "O produto encontrado não possui o status desejado. Status encontrado: " + statusProduto);
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
        public async Task A02_M_Disponiveis_ValidarExistenciaDeAspectosDaPagina()
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
                await AbrirPaginaProdutosDisponiveis(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoCadastrarProduto), "O botão 'Cadastrar Produto' não foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoAtualizarEstoque), "O botao 'Atualizar Estoque' não foi encontrado");
                Assert.IsFalse(await ElementoEstaVisivel(driver, botaoExportarProdutos), "O botão 'Exportar Produto' foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoAtualizarProdutos), "O botão 'Atualizar Produtos' não foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEnviarParaCanal), "O botão 'Enviar P/ Canal' não foi encontrado'");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoEnviarFotos), "O botão 'Enviar Fotos' não foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoFiltros), "O botão 'Filtros' não foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoAcoes), "O botão 'Ações' (três pontos) não foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoVisualizarProduto), "O botão 'Visualizar Produto' (responsável por adentrar o produto) não foi encontrado");
                await ClicarElemento(driver, botaoAtualizarEstoque);
                await driver.Locator(botaoExportarPlanilha).WaitForAsync();
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoExportarPlanilha), "O botão interno 'Exportar Panilha', logo após ter pressionado 'Atualizar Estoque', não foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoUpload), "O botão interno 'Upload', logo após ter pressionado 'Atualizar Estoque', não foi encontrado");
                await ClicarElemento(driver, botaoFecharModal);
                await ClicarElemento(driver, botaoAtualizarProdutos);
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoExportarPlanilha + " >> nth=1"), "O botão interno 'Exportar Panilha', logo após ter pressionado 'Atualizar Produto', não foi encontrado");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoUpload + " >> nth=1"), "O botão interno 'Upload', logo após ter pressionado 'Atualizar Produto', não foi encontrado");
                await ClicarElemento(driver, botaoFecharModal + " >> nth=1");
                Assert.IsTrue(await ElementoEstaVisivel(driver, botaoCheckBox), "O Checkbox (selecionar todos) do cabeçalho não foi encontrado");
                Assert.AreEqual("Imagem", await RetornaTextoElemento(driver, textoCabeçalhoImagem), "O texto do cabeçalho da coluna 'Imagem' não corresponde ao nome 'Imagem'");
                Assert.AreEqual("Código", await RetornaTextoElemento(driver, textoCabeçalhoCodigo), "O texto do cabeçalho da coluna 'Código' não corresponde ao nome 'Código'");
                Assert.AreEqual("Nome", await RetornaTextoElemento(driver, textoCabeçalhoNome), "O texto do cabeçalho da coluna 'Nome' não corresponde ao nome 'Nome'");
                Assert.AreEqual("Marca", await RetornaTextoElemento(driver, textoCabeçalhoMarca), "O texto do cabeçalho da coluna 'Marca' não corresponde ao nome 'Marca'");
                Assert.AreEqual("Categoria", await RetornaTextoElemento(driver, textoCabeçalhoCategoria), "O texto do cabeçalho da coluna 'Categoria' não corresponde ao nome 'Categoria'");
                Assert.AreEqual("Qtd. SKUS", await RetornaTextoElemento(driver, textoCabeçalhoQtdSkus), "O texto do cabeçalho da coluna 'Qtd. Sku' não corresponde ao nome 'Qtd. SKUS'");
                Assert.AreEqual("Canais", await RetornaTextoElemento(driver, textoCabeçalhoCanais), "O texto do cabeçalho da coluna 'Canais' não corresponde ao nome 'Canais'");
                Assert.AreEqual("Status", await RetornaTextoElemento(driver, textoCabeçalhoStatus), "O texto do cabeçalho da coluna 'Status' não corresponde ao nome 'Status'");
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
        public async Task A01_M_Disponiveis_CadastrarProduto()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var driver = await context.NewPageAsync();
            await driver.SetViewportSizeAsync(1920, 1080);
            driver.SetDefaultNavigationTimeout(200000);
            driver.SetDefaultTimeout(200000);
            try
            {
                await _funcoesLogin.LoginIT4MasterAsync(driver);
                await AbrirPaginaProdutosDisponiveis(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await ClicarElemento(driver, botaoCadastrarProduto);
                await ClicarElemento(driver, botaoCadastrar);
                await driver.Locator(mensagemCampoObrigatorioPalavrasChave).WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
                // Verificação dos campos obrigatórios
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioNome), "A mensagem de obrigatoriedade do campo 'Nome' não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioReferencia), "A mensagem de obrigatoriedade do campo 'Referencia' não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioMarca), "A mensagem de obrigatoriedade do campo 'Marca' não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioCategoria), "A mensagem de obrigatoriedade do campo 'Categoria' não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioDescricaoLonga), "A mensagem de obrigatoriedade do campo 'Descrição Longa' não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioDescricaoCurta), "A mensagem de obrigatoriedade do campo 'Descrição Curta' não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioMetaTags), "A mensagem de obrigatoriedade do campo 'Meta Tags' não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioPalavrasChave), "A mensagem de obrigatoriedade do campo 'Palavras Chave' não está visível");
                // Criação do produto
                await PreencherCamposCadastroProduto(driver,
                    "TesteAutomatizado", // Nome
                    "RefTesteAutomatizado", // Referência
                    "IT4 Solution", // Marca
                    itemIt4SolutionMarcaCriacaoProduto,// Item marca
                    "Camisetas", // Categoria
                    itemCamisetasCategoriaCriacaoProduto, // Item categoria
                    "Descrição longa para teste automatizado", // Descrição longa
                    "Descrição curta para teste automatizado", // Descrição curta
                    "TesteAutomatizado", // Metatags
                    "TesteAutomatizado"); // Palavras chave
                await ClicarElemento(driver, botaoCadastrar);
                await AguardarElemento(driver, notificacaoSucessoCriacaoProduto);
                // Valida os campos do produto cadastrado
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?name=TesteAutomatizado&onlyProductsWithoutChannel=false");
                await driver.WaitForLoadStateAsync(LoadState.Load);
                await driver.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, botaoEditarProduto);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await driver.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await driver.WaitForLoadStateAsync(LoadState.Load);
                Thread.Sleep(1000);
                await ClicarElemento(driver, botaoEditarProduto);
                await AguardarElemento(driver, campoNomeCadastroProduto);
                Assert.AreEqual("TesteAutomatizado", await RetornaValorElemento(driver, campoNomeCadastroProduto), "O valor do campo 'Nome' do Produto não corresponde à 'TesteAutomatizado'");
                Assert.AreEqual("RefTesteAutomatizado", await RetornaValorElemento(driver, campoReferenciaCadastroProduto), "O valor do campo 'Referencia' do Produto não corresponde à 'RefTesteAutomatizado'");
                Assert.AreEqual("IT4 Solution", await RetornaTextoElemento(driver, campoMarcaProdutoVerificacaoDisponiveis), "O valor do campo 'Marca' do Produto não corresponde à 'IT4 Solution'");
                Assert.AreEqual("Camisetas", await RetornaTextoElemento(driver, campoCategoriaProdutoVerificacaoDisponiveis), "O valor do campo 'Categoria' do Produto  não corresponde à 'Camisetas'");
                Assert.AreEqual("Descrição longa para teste automatizado", await RetornaTextoElemento(driver, campoDescricaoLongaCadastroProduto), "O valor do campo 'Descrição Longa' do Produto não corresponde à 'Descrição longa para teste automatizado'");
                Assert.AreEqual("Descrição curta para teste automatizado", await RetornaValorElemento(driver, campoDescricaoCurtaCadastroProduto), "O valor do campo 'Descrição Curta' do Produto não corresponde à 'Descrição curta para teste automatizado'");
                Assert.AreEqual("TesteAutomatizado", await RetornaTextoElemento(driver, campoMetaTagsProdutoVerificacaoDisponiveis), "O valor do campo 'Meta Tags' do Produto não corresponde à 'TesteAutomatizado'");
                Assert.AreEqual("TesteAutomatizado", await RetornaTextoElemento(driver, campoPalavrasChaveProdutoVerificacaoDisponiveis), "O valor do campo 'Palavras Chave' do Produto não corresponde à 'TesteAutomatizado'");
                // Grava a Url no Redis
                string url = driver.Url;
                string codigoProduto = url.Substring(46, 37);
                _conexaoBancoRedis.SetData("CodigoProduto", codigoProduto, 1800);
                _conexaoBancoRedis.SetData("ProdutoParaInserirSku", url, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizado");
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
        public async Task A11_M_Disponiveis_CadastrarSkuComTodosOsCampos()
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
                string chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (chaveRedis == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirUrlRedis(driver, "ProdutoParaInserirSku");
                await driver.Locator(botaoAdicionarSku).WaitForAsync();
                await ClicarElemento(driver, botaoAdicionarSku);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await ClicarElemento(driver, botaoOk);
                await driver.Locator(mensagemCampoObrigatorioReferenciaSku).WaitForAsync();
                Thread.Sleep(500);
                // Verificação dos campos obrigatórios
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioReferenciaSku), "A mensagem de obrigatoriedade do campo 'Referência' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioEanSku), "A mensagem de obrigatoriedade do campo 'Ean' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioPrecoDeSku), "A mensagem de obrigatoriedade do campo 'Preço De' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioPrecoPorSku), "A mensagem de obrigatoriedade do campo 'Preço Por' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioDescricaoSku), "A mensagem de obrigatoriedade do campo 'Descrição' do SKU não está visível");
                //Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioPesoSku), "A mensagem de obrigatoriedade do campo 'Peso' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioAlturaSku), "A mensagem de obrigatoriedade do campo 'Altura' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioLarguraSku), "A mensagem de obrigatoriedade do campo 'Largura' do SKU não está visível");
                //Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioComprimentoSku), "A mensagem de obrigatoriedade do campo 'Comprimento' do SKU não está visível");
                // Criação Sku
                await PreencherCamposCadastroSku(driver,
                    "ReferenciaSkuTesteAutomatizado", // Referencia do sku
                    "EanSkuTesteAutomatizado", // Ean do sku
                    "2000", // Preco de
                    "2000", // Preço por
                    "Primeiro sku para teste automatizado", // Descricao cadastro
                    "1", // Peso
                    "1", // Altura sku
                    "1", // Largura sku
                    "1", // Comprimento sku
                    "1", // Peso real sku
                    "1", // Peso cubico sku
                    "1", // Altura real sku
                    "1", // Largura real sku
                    "1"); // Comprimento real sku
                await ClicarElemento(driver, botaoOk);
                await AguardarElemento(driver, notificacaoSucessoCriacaoSku);
                // Verificacao dos campos cadastrados
                await ClicarElemento(driver, botaoEditar1);
                Assert.AreEqual("ReferenciaSkuTesteAutomatizado", await RetornaValorElemento(driver, campoReferenciaCadastroSku), "O valor do campo 'Referência' do SKU não corresponde à 'ReferenciaSkuTesteAutomatizado'");
                Assert.AreEqual("EanSkuTesteAutomatizado", await RetornaValorElemento(driver, campoEanCadastroSku), "O valor do campo 'Ean' do SKU não corresponde à 'EanSkuTesteAutomatizado'");
                Assert.AreEqual("R$20,00", await RetornaValorElemento(driver, campoPrecoDeCadastroSku), "O valor do campo 'Preço De' do SKU não corresponde à 'R$20,00'");
                Assert.AreEqual("R$20,00", await RetornaValorElemento(driver, campoPrecoPorCadastroSku), "O valor do campo 'Preço Por' do SKU não corresponde à 'R$20,00'");
                Assert.AreEqual("Primeiro sku para teste automatizado", await RetornaValorElemento(driver, campoDescricaoCadastroSku), "O valor do campo 'Descrição' do SKU não corresponde à 'Primeiro sku para teste automatizado'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoPesoCadastroSku), "O valor do campo 'Peso' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoPesoRealCadastroSku), "O valor do campo 'Peso Real' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoPesoCubicoCadastroSku), "O valor do campo 'Peso Cúbico' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoAlturaCadastroSku), "O valor do campo 'Altura' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoAlturaRealCadastroSku), "O valor do campo 'Altura Real' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoLarguraCadastroSku), "O valor do campo 'Largura' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoLarguraRealCadastroSku), "O valor do campo 'Largura Real' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoComprimentoCadastroSku), "O valor do campo 'Comprimento' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoComprimentoRealCadastroSku), "O valor do campo 'Comprimento Real' do SKU não corresponde à '1, 0000'");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizado", "ReferenciaSkuTesteAutomatizado");
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
        public async Task A10_M_Disponiveis_CadastrarSkuSomenteComCamposObrigatorios()
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
                string chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (chaveRedis == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirUrlRedis(driver, "ProdutoParaInserirSku");
                await driver.Locator(botaoAdicionarSku).WaitForAsync();
                await ClicarElemento(driver, botaoAdicionarSku);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await ClicarElemento(driver, botaoOk);
                await driver.Locator(mensagemCampoObrigatorioLarguraSku).WaitForAsync(new LocatorWaitForOptions() { State = WaitForSelectorState.Visible });
                // Verificação dos campos obrigatórios
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioReferenciaSku), "A mensagem de obrigatoriedade do campo 'Referência' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioEanSku), "A mensagem de obrigatoriedade do campo 'Ean' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioPrecoDeSku), "A mensagem de obrigatoriedade do campo 'Preço De' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioPrecoPorSku), "A mensagem de obrigatoriedade do campo 'Preço Por' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioDescricaoSku), "A mensagem de obrigatoriedade do campo 'Descrição' do SKU não está visível");
                //Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioPesoSku), "A mensagem de obrigatoriedade do campo 'Peso' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioAlturaSku), "A mensagem de obrigatoriedade do campo 'Altura' do SKU não está visível");
                Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioLarguraSku), "A mensagem de obrigatoriedade do campo 'Largura' do SKU não está visível");
                //Assert.IsTrue(await ElementoEstaVisivel(driver, mensagemCampoObrigatorioComprimentoSku), "A mensagem de obrigatoriedade do campo 'Comprimento' do SKU não está visível");
                // Criação Sku
                await PreencherCamposCadastroSku(driver,
                    "ReferenciaSkuTesteAutomatizado2", // Referencia Sku
                    "EanSkuTesteAutomatizado2", // Ean sku
                    "2000", // Preco de
                    "2000", // Preco por
                    "Segundo sku para teste automatizado", // Descricao sku
                    "1", // Peso sku
                    "1", // Altura sku
                    "1", // Largura sku
                    "1"); // Comprimento sku
                await ClicarElemento(driver, botaoOk);
                await AguardarElemento(driver, notificacaoSucessoCriacaoSku);
                // Verificacao dos campos cadastrados
                await ClicarElemento(driver, botaoEditar2);
                Assert.AreEqual("ReferenciaSkuTesteAutomatizado2", await RetornaValorElemento(driver, campoReferenciaCadastroSku), "O valor do campo 'Referência' do SKU não corresponde à 'ReferenciaSkuTesteAutomatizado2'");
                Assert.AreEqual("EanSkuTesteAutomatizado2", await RetornaValorElemento(driver, campoEanCadastroSku), "O valor do campo 'Ean' do SKU não corresponde à 'EanSkuTesteAutomatizado2'");
                Assert.AreEqual("R$20,00", await RetornaValorElemento(driver, campoPrecoDeCadastroSku), "O valor do campo 'Preço De' do SKU não corresponde à 'R$20,00'");
                Assert.AreEqual("R$20,00", await RetornaValorElemento(driver, campoPrecoPorCadastroSku), "O valor do campo 'Preço Por' do SKU não corresponde à 'R$20,00'");
                Assert.AreEqual("Segundo sku para teste automatizado", await RetornaValorElemento(driver, campoDescricaoCadastroSku), "O valor do campo 'Descrição' do SKU não corresponde à 'Segundo sku para teste automatizado'");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoPesoCadastroSku), "O valor do campo 'Peso' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("", await RetornaValorElemento(driver, campoPesoRealCadastroSku), "O valor do campo 'Peso Real' do SKU possui algum preenchimento");
                Assert.AreEqual("", await RetornaValorElemento(driver, campoPesoCubicoCadastroSku), "O valor do campo 'Peso Cúbico' do SKU possui algum preenchimento");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoAlturaCadastroSku), "O valor do campo 'Altura' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("", await RetornaValorElemento(driver, campoAlturaRealCadastroSku), "O valor do campo 'Altura Real' do SKU possui algum preenchimento");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoLarguraCadastroSku), "O valor do campo 'Largura' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("", await RetornaValorElemento(driver, campoLarguraRealCadastroSku), "O valor do campo 'Largura Real' do SKU possui algum preenchimento");
                Assert.AreEqual("1,0000", await RetornaValorElemento(driver, campoComprimentoCadastroSku), "O valor do campo 'Comprimento' do SKU não corresponde à '1, 0000'");
                Assert.AreEqual("", await RetornaValorElemento(driver, campoComprimentoRealCadastroSku), "O valor do campo 'Comprimento Real' do SKU possui algum preenchimento");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizado", "ReferenciaSkuTesteAutomatizado2");
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
        public async Task A18_M_Disponiveis_AtribuirImagemAoSkuPorUrl()
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
                string chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (chaveRedis == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirUrlRedis(driver, "ProdutoParaInserirSku");
                // Aguarda até que os dois SKUs existam
                bool existeBotaoEditar2 = await ElementoEstaVisivel(driver, botaoEditar2);
                while (!existeBotaoEditar2 && duracao <= 180000)
                {
                    await driver.ReloadAsync();
                    await AguardarElemento(driver, botaoAdicionarSku);
                    Thread.Sleep(1500);
                    existeBotaoEditar2 = await ElementoEstaVisivel(driver, botaoEditar2);
                    duracao = timer.ElapsedMilliseconds;
                }
                // Atribuicao da imagem ao SKU
                await AguardarElemento(driver, botaoEditar1);
                await ClicarElemento(driver, botaoEditar1);
                await AguardarElemento(driver, botaoAdicionarImagemPorUrl);
                await ClicarElemento(driver, botaoAdicionarImagemPorUrl);
                await PreencherCamposDoCadastroDeImagemPorUrlDoSku(driver);
                await ClicarElemento(driver, botaoOk);
                // Nova abertura do SKU para verificacao da imagem
                await AguardarElemento(driver, botaoEditar1);
                await ClicarElemento(driver, botaoEditar1);
                await AguardarElemento(driver, imagemSku);
                string sourceImagem = await driver.Locator(imagemSku).GetAttributeAsync("src");
                Assert.IsTrue(sourceImagem.Contains("imagem_teste_automatizado"), "A source da imagem não contem a partição 'imagem_teste_automatizado'");
                Assert.AreEqual("camiseta_teste_automatizado", await RetornaValorElemento(driver, campoNomeImagem), "O valor encontrado para o campo 'Nome' da imagem não corresponde à 'camiseta_teste_automatizado'");
                Assert.AreEqual("DescricaoImagemSku", await RetornaValorElemento(driver, campoDescricaoImagem), "O valor encontrado para o campo 'Descrição' da imagem não corresponde à 'DescricaoImagemSku'");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizado", "ReferenciaSkuTesteAutomatizado", "DescricaoImagemSku");
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
        public async Task A17_M_Disponiveis_AtribuirImagemAoSkuPorComputador()
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
                string chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (chaveRedis == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaInserirSku");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirUrlRedis(driver, "ProdutoParaInserirSku");
                // Aguarda até que os dois SKUs existam
                bool existeBotaoEditar2 = await ElementoEstaVisivel(driver, botaoEditar2);
                while (!existeBotaoEditar2 && duracao <= 180000)
                {
                    await driver.ReloadAsync();
                    await AguardarElemento(driver, botaoAdicionarSku);
                    Thread.Sleep(1500);
                    existeBotaoEditar2 = await ElementoEstaVisivel(driver, botaoEditar2);
                    duracao = timer.ElapsedMilliseconds;
                }
                // Atribuicao da imagem
                await AguardarElemento(driver, botaoEditar2);
                await ClicarElemento(driver, botaoEditar2);
                await AguardarElemento(driver, botaoAdicionarImagemPorComputador);
                await ClicarElemento(driver, botaoAdicionarImagemPorComputador);
                await PreencherCamposDoCadastroDeImagemPorComputadorDoSku(driver);
                await ClicarElemento(driver, botaoOk);
                // Nova abertura do SKU para verificacao da imagem
                await AguardarElementoNaoVisivel(driver, botaoOk);
                await ClicarElemento(driver, botaoEditar2);
                await AguardarElemento(driver, imagemSku);
                string imagemComputadorSku = await driver.Locator(imagemSku).GetAttributeAsync("src");
                Assert.IsTrue(imagemComputadorSku.Contains("bola_teste_automatizado"), "A source da imagem não contem a partição 'bola_teste_automatizado'");
                Assert.AreEqual("bola_teste_automatizado.jpg", await RetornaValorElemento(driver, campoNomeImagem), "O valor encontrado para o campo 'Nome' da imagem não corresponde à 'bola_teste_automatizado.jpg'");
                Assert.AreEqual("DescricaoImagemSku", await RetornaValorElemento(driver, campoDescricaoImagem), "O valor encontrado para o campo 'Descrição' da imagem não corresponde à 'DescricaoImagemSku'");
                string url = driver.Url;
                string codigoProduto = url.Substring(46, 36);
                _conexaoBancoRedis.SetData("CodigoProdutoParaExclusao", codigoProduto, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizado", "ReferenciaSkuTesteAutomatizado2", "DescricaoImagemSku");
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
        public async Task A31_M_Disponiveis_ExcluirProdutoDisponivel()
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
                string codigoProdutoExclusao = _conexaoBancoRedis.GetData<string>("CodigoProdutoParaExclusao");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (codigoProdutoExclusao == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    codigoProdutoExclusao = _conexaoBancoRedis.GetData<string>("CodigoProdutoParaExclusao");
                    duracao = timer.ElapsedMilliseconds;
                }
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?idsProducts[]=" + codigoProdutoExclusao + "&name=&onlyProductsWithoutChannel=false");
                await driver.WaitForLoadStateAsync(LoadState.Load);
                await driver.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                int[] quantidadelinhas = await RetornarLinhasGrid(driver);
                await ClicarElemento(driver, botaoAcoes);
                await ClicarElemento(driver, botaoRemover);
                await ClicarElemento(driver, botaoConfirmar);
                await AguardarElementoNaoVisivel(driver, botaoVisualizarProduto);
                // Verifica se o registro de fato não existe mais
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                _connection = new SqlConnection("Data Source=stg360.database.windows.net;Initial Catalog=stg360;Integrated Security=False;User ID=administrador;Password=Kemed@#100;MultipleActiveResultSets=true");
                using (var connection = _connection)
                {
                    var query = $"select Id, [Name], RefProduct from Product where Id = '{codigoProdutoExclusao}'";
                    var produto = connection.QueryFirstOrDefault<Product>(query);
                    Assert.IsNull(produto, "O produto encontrado contem um registro no banco.");
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
        public async Task A27_M_Disponiveis_EnviarTodosOsProdutosParaOCanal()
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
                await AbrirPaginaProdutosDisponiveis(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                // Cadastro de produto
                await ClicarElemento(driver, botaoCadastrarProduto);
                await PreencherCamposCadastroProduto(driver,
                    "TesteAutomatizadoParaEnvioDeProdutos", // Nome
                    "RefTesteAutoParaEnvioDeProdutos", // Referência
                    "IT4 Solution", // Marca
                    itemIt4SolutionMarcaCriacaoProduto,// Item marca
                    "Camisetas", // Categoria
                    itemCamisetasCategoriaCriacaoProduto, // Item categoria
                    "Descrição longa para teste automatizado", // Descrição longa
                    "Descrição curta para teste automatizado", // Descrição curta
                    "TesteAutomatizado", // Metatags
                    "TesteAutomatizado"); // Palavras chave
                await ClicarElemento(driver, botaoCadastrar);
                await AguardarElemento(driver, botaoAdicionarSku);
                // Cadastro de SKU
                await ClicarElemento(driver, botaoAdicionarSku);
                await PreencherCamposCadastroSku(driver,
                    "ReferenciaSkuTesteAutomatizadoParaEnvio", // Referencia
                    "EanSkuTesteAutomatizadoEnvio", // Ean
                    "2000", // Preco de
                    "2000", // Preço por
                    "Descricao sku para teste automatizado", // Descricao cadastro
                    "1", // Peso
                    "1", // Altura
                    "1", // Largura
                    "1", // Comprimento
                    "1", // Peso real
                    "1", // Peso cubico
                    "1", // Altura real
                    "1", // Largura real
                    "1"); // Comprimento real
                // Adicionando imagem ao SKU
                await ClicarElemento(driver, botaoAdicionarImagemPorUrl);
                await PreencherCamposDoCadastroDeImagemPorUrlDoSku(driver);
                await ClicarElemento(driver, botaoOk);
                await AguardarElemento(driver, notificacaoSucessoCriacaoSku);
                await AguardarElementoNaoVisivel(driver, botaoOk);
                // Salva o código do produto para filtrar no submenu A Venda
                string url = driver.Url;
                string codigoProduto = url.Substring(46, 36); // Começa na posição 45 e tem um comprimento de 36 caracteres
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?idsProducts[]=" + codigoProduto + "&onlyProductsWithoutChannel=false");
                // Envia todo grid visível para o canal
                await AguardarElemento(driver, botaoVisualizarProduto);
                await ClicarElemento(driver, botaoEnviarParaCanal);
                await ClicarElemento(driver, listaDeCanais);
                await ClicarElemento(driver, itemVtexListaDeCanais);
                await ClicarElemento(driver, botaoConfirmar);
                await AguardarElemento(driver, notificacaoSucessoEnvioProdutoCanal);
                // Verifica se o produto existe no submenu A Venda
                await driver.GotoAsync("https://stg2.it4360.com.br/products/sales?name=TesteAutomatizadoParaEnvioDeProdutos");
                await AguardarElemento(driver, botaoVisualizarProduto);
                string nomeProduto;
                string canal;
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.IsTrue(linhasTabelaGridPrincipal.Length != 0, "Não há registros nessa tabela");
                foreach (int linha in linhasTabelaGridPrincipal)
                {
                    nomeProduto = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(linha, (int)FuncoesProdutosAVenda.PosicaoCampoGridMaster.Nome));
                    canal = await RetornaTextoElemento(driver, RetornaCelulaGridPrincipal(1, (int)FuncoesProdutosAVenda.PosicaoCampoGridMaster.Canal));
                }
                // Tenta excluir o produto do submenu Disponíveis, contudo, essa ação não deve estar disponível, pois existe um produto no canal
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?idsProducts[]=" + codigoProduto + "&onlyProductsWithoutChannel=false");
                await AguardarElemento(driver, botaoVisualizarProduto);
                await ClicarElemento(driver, botaoAcoes);
                bool botaoRemoverEstaVisivel = await driver.Locator(botaoRemover).IsVisibleAsync();
                Assert.IsFalse(botaoRemoverEstaVisivel, "O botão excluir foi localizado");
                _conexaoBancoRedis.SetData("ProdutoParaEdicao", url, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("RefTesteAutoParaEnvioDeProdutos", "ReferenciaSkuTesteAutomatizadoParaEnvio");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutoParaEnvioDeProdutos", "ReferenciaSkuTesteAutomatizadoParaEnvio", "DescricaoImagemSku");
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
        public async Task A28_M_Disponiveis_EditarTodosOsCamposDoProduto()
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
                string chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaEdicao");
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                while (chaveRedis == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoParaEdicao");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirUrlRedis(driver, "ProdutoParaEdicao");
                await AguardarElemento(driver, campoNomeCadastroProduto);
                await LimparTextoEmEdicaoDoProduto(driver);
                // Atribui os novos valores aos campos
                await PreencherCampo(driver, campoPalavraChaveCadastroProduto, "PalavraChaveEdit");
                await driver.Keyboard.PressAsync("Enter");
                await PreencherCampo(driver, campoMetaTagsCadastroProduto, "MetaTagsEdit");
                await driver.Keyboard.PressAsync("Enter");
                await PreencherCampo(driver, campoDescricaoCurtaCadastroProduto, "Descrição curta editada");
                await PreencherCampo(driver, campoDescricaoLongaCadastroProduto, "Descrição longa editada");
                await ForcarClicarElemento(driver, campoCategoriaCadastroProduto);
                await PreencherCampoESelecionarItem(driver, campoCategoriaCadastroProduto, "Jóias", itemJoiasCategoriaProduto);
                await PreencherCampoESelecionarItem(driver, campoMarcaCadastroProduto, "leonardo", itemLeonardoMarcaProduto);
                await PreencherCampo(driver, campoReferenciaCadastroProduto, "RefTesteAutomatizadoEdicao");
                await PreencherCampo(driver, campoNomeCadastroProduto, "TesteAutomatizadoEdicao");
                await ClicarElemento(driver, botaoAtualizar);
                await AguardarElemento(driver, notificacaoSucessoAtualizaçãoProduto);
                string url = driver.Url;
                _conexaoBancoRedis.SetData("ProdutoEditado", url, 1800);
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?name=TesteAutomatizadoEdicao&onlyProductsWithoutChannel=false");
                await ClicarElemento(driver, botaoVisualizarProduto);
                await AguardarElemento(driver, botaoEditarProduto);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await driver.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
                await driver.WaitForLoadStateAsync(LoadState.Load);
                Thread.Sleep(1000);
                // Verifica se os campos editados de fato foram alterados
                await ClicarElemento(driver, botaoEditarProduto);
                await AguardarElemento(driver, campoNomeCadastroProduto);
                Assert.AreEqual("TesteAutomatizadoEdicao", await RetornaValorElemento(driver, campoNomeCadastroProduto), "O valor do campo 'Nome' do Produto não corresponde à 'TesteAutomatizado'");
                Assert.AreEqual("RefTesteAutomatizadoEdicao", await RetornaValorElemento(driver, campoReferenciaCadastroProduto), "O valor do campo 'Referencia' do Produto não corresponde à 'RefTesteAutomatizado'");
                Assert.AreEqual("leonardo", await RetornaTextoElemento(driver, campoMarcaEdicaoProdutoVerificacaoDisponiveis), "O valor do campo 'Marca' do Produto não corresponde à 'IT4 Solution'");
                Assert.AreEqual("Jóias", await RetornaTextoElemento(driver, campoCategoriaEdicaoProdutoVerificacaoDisponiveis), "O valor do campo 'Categoria' do Produto  não corresponde à 'Camisetas'");
                Assert.AreEqual("Descrição longa editada", await RetornaTextoElemento(driver, campoDescricaoLongaCadastroProduto), "O valor do campo 'Descrição Longa' do Produto não corresponde à 'Descrição longa para teste automatizado'");
                Assert.AreEqual("Descrição curta editada", await RetornaValorElemento(driver, campoDescricaoCurtaCadastroProduto), "O valor do campo 'Descrição Curta' do Produto não corresponde à 'Descrição curta para teste automatizado'");
                Assert.AreEqual("MetaTagsEdit", await RetornaTextoElemento(driver, campoMetaTagsEdicaoProdutoVerificacaoDisponiveis), "O valor do campo 'Meta Tags' do Produto não corresponde à 'TesteAutomatizado'");
                Assert.AreEqual("PalavraChaveEdit", await RetornaTextoElemento(driver, campoPalavrasChaveEdicaoProdutoVerificacaoDisponiveis), "O valor do campo 'Palavras Chave' do Produto não corresponde à 'TesteAutomatizado'");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("RefTesteAutomatizadoEdicao", "ReferenciaSkuTesteAutomatizadoParaEnvio");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizadoEdicao", "ReferenciaSkuTesteAutomatizadoParaEnvio", "DescricaoImagemSku");
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
        public async Task A29_M_Disponiveis_EditarTodosOsCamposDoSku()
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
                Stopwatch timer = new Stopwatch();
                timer.Start();
                long duracao = 0;
                // Aguarda até que o produto editado tenha sido gravado no Redis
                string chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoEditado");
                while (chaveRedis == null && duracao <= 180000)
                {
                    Thread.Sleep(2000);
                    chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoEditado");
                    duracao = timer.ElapsedMilliseconds;
                }
                await AbrirUrlRedis(driver, "ProdutoEditado");
                // Limpa todos campos do SKU e preenche com o novo modelo editado
                await AguardarElemento(driver, campoNomeCadastroProduto);
                await ClicarElemento(driver, botaoEditarSku);
                await LimparTextoEmEdicaoDoSku(driver);
                await PreencherCampoPorCaractere(driver, campoReferenciaCadastroSku, "RefSkuTesteAutomatizadoEdicao");
                await PreencherCampoPorCaractere(driver, campoEanCadastroSku, "EanSkuTesteAutomatizadoEdicao");
                await PreencherCampoPorCaractere(driver, campoPrecoDeCadastroSku, "30");
                await PreencherCampoPorCaractere(driver, campoPrecoPorCadastroSku, "30");
                await PreencherCampoPorCaractere(driver, campoDescricaoCadastroSku, "Descricao sku para teste automatizado edicao");
                await PreencherCampoPorCaractere(driver, campoPesoCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoPesoRealCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoPesoCubicoCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoAlturaCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoAlturaRealCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoLarguraCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoLarguraRealCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoComprimentoCadastroSku, "2");
                await PreencherCampoPorCaractere(driver, campoComprimentoRealCadastroSku, "2");
                await ClicarElemento(driver, botaoOk);
                await AguardarElemento(driver, notificacaoSucessoAtualizaçãoSku);
                // Atualiza a página e verifica se todos os campos estão de acordo com o editado
                await driver.ReloadAsync();
                await AguardarElemento(driver, botaoEditarSku);
                await ClicarElemento(driver, botaoEditarSku);
                await AguardarElemento(driver, campoReferenciaCadastroSku);
                Assert.AreEqual("RefSkuTesteAutomatizadoEdicao", await RetornaValorElemento(driver, campoReferenciaCadastroSku), "O valor do campo 'Referência' do SKU não corresponde à 'RefSkuTesteAutomatizadoEdicao'");
                Assert.AreEqual("EanSkuTesteAutomatizadoEdicao", await RetornaValorElemento(driver, campoEanCadastroSku), "O valor do campo 'Ean' do SKU não corresponde à 'EanSkuTesteAutomatizadoEdicao'");
                Assert.AreEqual("R$30,00", await RetornaValorElemento(driver, campoPrecoDeCadastroSku), "O valor do campo 'Preço De' do SKU não corresponde à 'R$30,00'");
                Assert.AreEqual("R$30,00", await RetornaValorElemento(driver, campoPrecoPorCadastroSku), "O valor do campo 'Preço Por' do SKU não corresponde à 'R$30,00'");
                Assert.AreEqual("Descricao sku para teste automatizado edicao", await RetornaValorElemento(driver, campoDescricaoCadastroSku), "O valor do campo 'Descrição' do SKU não corresponde à 'Descricao sku para teste automatizado edicao'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoPesoCadastroSku), "O valor do campo 'Peso' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoPesoRealCadastroSku), "O valor do campo 'Peso Real' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoPesoCubicoCadastroSku), "O valor do campo 'Peso Cúbico' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoAlturaCadastroSku), "O valor do campo 'Altura' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoAlturaRealCadastroSku), "O valor do campo 'Altura Real' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoLarguraCadastroSku), "O valor do campo 'Largura' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoLarguraRealCadastroSku), "O valor do campo 'Largura Real' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoComprimentoCadastroSku), "O valor do campo 'Comprimento' do SKU não corresponde à '2, 0000'");
                Assert.AreEqual("2,0000", await RetornaValorElemento(driver, campoComprimentoRealCadastroSku), "O valor do campo 'Comprimento Real' do SKU não corresponde à '2, 0000'");
                // Salva a edição no redis
                string url = driver.Url;
                _conexaoBancoRedis.SetData("ProdutoESkuEditado", url, 1800);
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("RefTesteAutomatizadoEdicao", "RefSkuTesteAutomatizadoEdicao");
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizadoEdicao", "RefSkuTesteAutomatizadoEdicao", "DescricaoImagemSku");
                Assert.IsTrue(false);
            }
            finally
            {
                await driver.CloseAsync();
                playwright.Dispose();
            }
        }


        //[Parallelizable]
        //[Test]
        //public async Task A30_RessincronizarEdicaoComOCanalDisponiveisMaster()
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
        //        Stopwatch timer = new Stopwatch();
        //        timer.Start();
        //        long duracao = 0;
        //        // Aguarda o produto e o SKU serem editados para que sua chave do redis seja resgatada
        //        string chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoESkuEditado");
        //        while (chaveRedis == null && duracao <= 350000)
        //        {
        //            Thread.Sleep(2000);
        //            chaveRedis = _conexaoBancoRedis.GetData<string>("ProdutoESkuEditado");
        //            duracao = timer.ElapsedMilliseconds;
        //        }
        //        await AbrirUrlRedis(driver, "ProdutoESkuEditado");
        //        // Ressincroniza atualização do submenu Disponíveis com A Venda
        //        await AguardarElemento(driver, botaoRessincronizarComOCanal);
        //        await ClicarElemento(driver, botaoRessincronizarComOCanal);
        //        await ClicarElemento(driver, botaoOk);
        //        await AguardarElementoNaoVisivel(driver, botaoOk);
        //        // Verifica se atualização foi refletida no submenu A Venda
        //        await driver.GotoAsync("https://stg2.it4360.com.br/products/sales?name=TesteAutomatizadoEdicao");
        //        await AguardarElemento(driver, botaoVisualizarProduto);
        //        await ClicarElemento(driver, botaoVisualizarProduto);
        //        await ClicarElemento(driver, botaoEditarProduto);
        //        Assert.AreEqual("TesteAutomatizadoEdicao", await RetornaValorElemento(driver, campoNomeCadastroProduto), "O valor do campo 'Nome' do Produto não corresponde à 'TesteAutomatizado'");
        //        Assert.AreEqual("RefTesteAutomatizadoEdicao", await RetornaValorElemento(driver, campoReferenciaCadastroProduto), "O valor do campo 'Referencia' do Produto não corresponde à 'RefTesteAutomatizado'");
        //        Assert.AreEqual("Acuo", await RetornaTextoElemento(driver, campoMarcaProdutoVerificacaoDisponiveis), "O valor do campo 'Marca' do Produto não corresponde à 'IT4 Solution'");
        //        Assert.AreEqual("Saúde", await RetornaTextoElemento(driver, campoCategoriaProdutoVerificacaoDisponiveis), "O valor do campo 'Categoria' do Produto  não corresponde à 'Camisetas'");
        //        Assert.AreEqual("Descrição longa editada", await RetornaTextoElemento(driver, campoDescricaoLongaCadastroProduto), "O valor do campo 'Descrição Longa' do Produto não corresponde à 'Descrição longa para teste automatizado'");
        //        Assert.AreEqual("Descrição curta editada", await RetornaValorElemento(driver, campoDescricaoCurtaCadastroProduto), "O valor do campo 'Descrição Curta' do Produto não corresponde à 'Descrição curta para teste automatizado'");
        //        Assert.AreEqual("MetaTagsEdit", await RetornaTextoElemento(driver, campoMetaTagsProdutoVerificacaoDisponiveis), "O valor do campo 'Meta Tags' do Produto não corresponde à 'TesteAutomatizado'");
        //        Assert.AreEqual("PalavraChaveEdit", await RetornaTextoElemento(driver, campoPalavrasChaveProdutoVerificacaoDisponiveis), "O valor do campo 'Palavras Chave' do Produto não corresponde à 'TesteAutomatizado'");
        //        await driver.CloseAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
        //        GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
        //        _funcoesBancoIt4360Stg.ExcluirProdutoAVenda("RefTesteAutomatizadoEdicao", "RefSkuTesteAutomatizadoEdicao");
        //        _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("RefTesteAutomatizadoEdicao", "RefSkuTesteAutomatizadoEdicao", "DescricaoImagemSku");
        //        await driver.CloseAsync();
        //        Assert.IsTrue(false);
        //    }
        //}


        [Parallelizable]
        [Test]
        public async Task A32_M_Disponiveis_TentarEnviarProdutoSelecionadoSemSkuParaOCanal()
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
                // Cadastro de produto
                await AbrirPaginaProdutosDisponiveis(driver);
                await AguardarElemento(driver, botaoVisualizarProduto);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await ClicarElemento(driver, botaoCadastrarProduto);

                await PreencherCamposCadastroProduto(driver,
                    "TesteAutomatizadoSemSkuParaEnvio", // Nome
                    "ReferenciaSemSkuParaEnvio", // Referência
                    "IT4 Solution", // Marca
                    itemIt4SolutionMarcaCriacaoProduto,// Item marca
                    "Camisetas", // Categoria
                    itemCamisetasCategoriaCriacaoProduto, // Item categoria
                    "Descrição longa para teste automatizado", // Descrição longa
                    "Descrição curta para teste automatizado", // Descrição curta
                    "TesteAutomatizado", // Metatags
                    "TesteAutomatizado"); // Palavras chave
                await ClicarElemento(driver, botaoCadastrar);
                await AguardarElemento(driver, notificacaoSucessoCriacaoProduto);
                // Enviar para o submenu A Venda
                await driver.GotoAsync("https://stg2.it4360.com.br/products/available?name=TesteAutomatizadoSemSkuParaEnvio&onlyProductsWithoutChannel=false");
                await AguardarElemento(driver, botaoVisualizarProduto);
                await ClicarElemento(driver, botaoCheckBox);
                await ClicarElemento(driver, botaoEnviarParaCanal);
                await ClicarElemento(driver, listaDeCanais);
                await ClicarElemento(driver, itemVtexListaDeCanais);
                await ClicarElemento(driver, botaoConfirmar);
                //await AguardarElemento(driver, notificacaoFalhaEnvioProdutoCanal);
                //// Verifica se o produto está no submenu A Venda
                await driver.GotoAsync("https://stg2.it4360.com.br/products/sales?name=TesteAutomatizadoSemSkuParaEnvio");
                Thread.Sleep(3000);
                await driver.WaitForLoadStateAsync(LoadState.NetworkIdle);
                int[] linhasTabelaGridPrincipal = await RetornarLinhasGrid(driver);
                Assert.AreEqual(linhasTabelaGridPrincipal.Length, 0, "Um registro com o nome 'TesteAutomatizadoSemSkuParaEnvio' foi encontrado no canal");
            }
            catch (Exception e)
            {
                await TirarScreenshot(driver, TestContext.CurrentContext.Test.Name.ToString());
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString() + "\n\n   URL: " + driver.Url);
                _funcoesBancoIt4360Stg.ExcluirProdutoDisponivel("ReferenciaSemSkuParaEnvio");
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
