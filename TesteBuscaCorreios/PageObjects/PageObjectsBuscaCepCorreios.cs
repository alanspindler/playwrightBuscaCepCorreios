using PlaywrightAutomacao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuscaCepCorreios
{
    public class PageObjectsBuscaCepCorreios : MetodosAuxiliares
    {
        public string campoCepEndereco = "[id='endereco']";
        public string botaoBuscar = "[id='btn_pesquisar']";
        public string mensagemResultado = "[id=mensagem-resultado]";
        public string mensagemResultadoAlerta = "[id=mensagem-resultado-alerta]";
        public string resultadoEndereco = "td:nth-child(1)";
        public string resultadoBairroDistrito = "td:nth-child(2)";
        public string resultadoLocalidadeUf = "td:nth-child(3)";
        public string resultadoCep = "td:nth-child(4)";        
    }
}
