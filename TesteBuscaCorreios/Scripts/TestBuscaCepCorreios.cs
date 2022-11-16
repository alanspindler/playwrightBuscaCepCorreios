using Microsoft.Playwright;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

[assembly: LevelOfParallelism(3)]

namespace BuscaCepCorreios
{
    [TestFixture]
    public class TestBuscaCepCorreios : FuncoesBuscaCepCorreios
    {
        [Parallelizable]
        [Test]
        public async Task PesquisarPorCep()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var page = await context.NewPageAsync();
            await page.SetViewportSizeAsync(1920, 1080);
            page.SetDefaultNavigationTimeout(30000);
            try
            {
                await AcessarBuscaCepCorreios(page);
                dadosEnderecoRetornado dadosEndereco = await PesquisarEnderecoPorCepEndereco(page, "69082-640");
                Assert.IsTrue(dadosEndereco.endereco == "Rua Doutor Elviro Dantas");
                Assert.IsTrue(dadosEndereco.bairroDistrito == "Coroado");
                Assert.IsTrue(dadosEndereco.localidadeUf == "Manaus/AM");
                Assert.IsTrue(dadosEndereco.cepResultado == "69082-640");
                await TirarScreenshot(page, TestContext.CurrentContext.Test.Name.ToString() + " Sucesso");
                GravarLogExecucao(TestContext.CurrentContext.Test.Name.ToString() + " Sucesso");
            }
            catch (Exception e)
            {
                await TirarScreenshot(page, TestContext.CurrentContext.Test.Name.ToString() + " Erro");
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString());
                Assert.False(true);
            }
            finally
            {
                await page.CloseAsync();
                await browser.DisposeAsync();
                await context.DisposeAsync();
                playwright.Dispose();
            }
        }

        [Parallelizable]
        [Test]
        public async Task PesquisarPorNomeEmpresa()
        {
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = false });
            var context = await browser.NewContextAsync(new BrowserNewContextOptions { IgnoreHTTPSErrors = true });
            var page = await context.NewPageAsync();
            await page.SetViewportSizeAsync(1920, 1080);
            page.SetDefaultNavigationTimeout(30000);
            try
            {
                await AcessarBuscaCepCorreios(page);
                dadosEnderecoRetornado dadosEndereco = await PesquisarEnderecoPorCepEndereco(page, "Instituto Creathus");
                Assert.IsTrue(dadosEndereco.endereco == "");
                Assert.IsTrue(dadosEndereco.bairroDistrito == "");
                Assert.IsTrue(dadosEndereco.localidadeUf == "");
                Assert.IsTrue(dadosEndereco.cepResultado == "");
                Assert.IsTrue(await RetornaTextoElemento(page, mensagemResultadoAlerta) == "Dados não encontrado");
                await TirarScreenshot(page, TestContext.CurrentContext.Test.Name.ToString() + " Sucesso" );
                GravarLogExecucao(TestContext.CurrentContext.Test.Name.ToString() + " Sucesso");
            }
            catch (Exception e)
            {
                await TirarScreenshot(page, TestContext.CurrentContext.Test.Name.ToString() + " Erro" );
                GravarLogErro(TestContext.CurrentContext.Test.Name.ToString() + " " + e.ToString());
                Assert.False(true);
            }
            finally
            {
                await page.CloseAsync();
                await browser.DisposeAsync();
                await context.DisposeAsync();
                playwright.Dispose();
            }
        }
    }
}

