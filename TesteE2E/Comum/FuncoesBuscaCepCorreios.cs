using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaCepCorreios
{
    public class FuncoesBuscaCepCorreios : PageObjectsBuscaCepCorreios
    {
        private string urlBuscaCepCorreios = "http://www.buscacep.correios.com.br/";
        public struct dadosEnderecoRetornado { public string endereco; public string bairroDistrito; public string localidadeUf; public string cepResultado; }

        public async Task AcessarBuscaCepCorreios(IPage page)
        {
            await page.GotoAsync(urlBuscaCepCorreios);
            await AguardarCarregarPagina(page);
        }

        public async Task<dadosEnderecoRetornado> PesquisarEnderecoPorCepEndereco(IPage page, string cepEndereco)
        {
            dadosEnderecoRetornado resultado;
            await PreencherCampo(page, campoCepEndereco, cepEndereco);
            await ClicarElemento(page, botaoBuscar);
            await AguardarElemento(page, mensagemResultado);
            await AguardarCarregarPagina(page);            
            if (await ElementoEstaVisivel(page, mensagemResultadoAlerta))            {
                resultado.endereco = "";
                resultado.bairroDistrito = "";
                resultado.localidadeUf = "";
                resultado.cepResultado = "";
                return resultado;
            }
            else
            {                
                resultado.endereco = await RetornaTextoElemento(page, resultadoEndereco);
                resultado.bairroDistrito = await RetornaTextoElemento(page, resultadoBairroDistrito);
                resultado.localidadeUf = await RetornaTextoElemento(page, resultadoLocalidadeUf);
                resultado.cepResultado = await RetornaTextoElemento(page, resultadoCep);
                return resultado;
            }
        }
    }
}
