using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightAutomacao
{
    public class PageObjectsLogin : MetodosAuxiliares
    {
        public string campoEmail = "[id='email']";
        public string campoSenha = "[id='password']";
        public string botaoLogin = "[type='submit']";
    }
}
