using Microsoft.Playwright;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PlaywrightAutomacao
{
    public class MetodosAuxiliares

    {
        #region Relações com elementos

        public async Task ClicarElemento(IPage page, string elemento)
        {
            await page.Locator(elemento).WaitForAsync();
            await page.ClickAsync(elemento);
        }

        public async Task AguardarElemento(IPage page, string elemento)
        {
            await page.WaitForSelectorAsync(elemento);
        }

        public async Task PreencherCampo(IPage page, string elemento, string texto)
        {
            await page.WaitForSelectorAsync(elemento);
            await page.FillAsync(elemento, texto);
        }      

        public async Task<string> RetornaTextoElemento(IPage page, string elemento)
        {
            string textoElemento;
            await page.Locator(elemento).WaitForAsync();
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            textoElemento = await page.Locator(elemento).TextContentAsync();
            return textoElemento;
        }       

        public async Task<bool> ElementoEstaVisivel(IPage page, string elemento)
        {    
            int elementos = await page.Locator(elemento).CountAsync();
            if (elementos > 0)
            {
                bool elementoEstaAtivo = await page.Locator(elemento).IsVisibleAsync();
                return elementoEstaAtivo;
            }
            else
            {
                return false;
            }
        }

        public async Task AguardarCarregarPagina(IPage page)
        {
            await page.WaitForLoadStateAsync(LoadState.Load);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
        }    

        #endregion

        #region Tratativas de erros

        private string DIRETORIO_APLICACAO
        {
            get
            {
                string diretorioAplicacao = string.Empty;
                if (string.IsNullOrEmpty(diretorioAplicacao))
                {
                    diretorioAplicacao = Path.GetFullPath(@"..\..\..\bin\Debug");
                }
                return diretorioAplicacao;
            }
        }

        private string CriarPasta(string nomePasta)
        {
            //Na primeira vez verifica se existe a pasta, senão cria
            var nomeArquivo = DIRETORIO_APLICACAO + $"\\{nomePasta}";
            if (!Directory.Exists(nomeArquivo))
            {
                Directory.CreateDirectory(nomeArquivo);
            }
            return nomeArquivo;
        }

        public void GravarLogErro(string texto)
        {
            var nomeArquivo = CriarPasta("TesteLogs") + $"\\Log-Erros.txt";
            string dataParaLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            texto = dataParaLog + " - " + texto + "\n\n";
            Random rnd = new Random();
            int random = rnd.Next(300, 800);
            Thread.Sleep(random);
            File.AppendAllText(nomeArquivo, texto);
        }

        public void GravarLogExecucao(string texto)
        {
            var nomeArquivo = CriarPasta("TesteLogs") + $"\\LogExecucao.txt";
            string dataParaLog = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            texto = dataParaLog + " - " + texto + "\n\n";
            Random rnd = new Random();
            int random = rnd.Next(300, 800);
            Thread.Sleep(random);
            File.AppendAllText(nomeArquivo, texto);
        }

        public async Task TirarScreenshot(IPage page, string nomeTela)
        {
            CriarPasta("TesteScreenShots");
            await page.ScreenshotAsync(new PageScreenshotOptions { Path = DIRETORIO_APLICACAO + "\\TesteScreenShots\\" + nomeTela + ".png" });
        }

        #endregion
    }
}

