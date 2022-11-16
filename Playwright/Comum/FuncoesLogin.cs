using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightAutomacao
{
    public class FuncoesLogin : PageObjectsLogin
    {
        private string emailLoginFilial = "testeautomatizadoit4@gmail.com";
        private string senhaLoginFilial = "superteste";
        private string emailLoginMaster = "testeit4360master@gmail.com";
        private string senhaLoginMaster = "superteste";

        public async Task LoginIT4FilialAsync(IPage driver)
        {
            await driver.GotoAsync("https://stg2.it4360.com.br/");
            await PreencherCampo(driver, campoEmail, emailLoginFilial);
            await PreencherCampo(driver, campoSenha, senhaLoginFilial);
            await ForcarClicarElemento(driver, botaoLogin);
            await driver.WaitForSelectorAsync("[class='ant-card-body']");
        }

        public async Task LoginIT4MasterAsync(IPage driver)
        {
            await driver.GotoAsync("https://stg2.it4360.com.br/");
            await PreencherCampo(driver, campoEmail, emailLoginMaster);
            await PreencherCampo(driver, campoSenha, senhaLoginMaster);
            await ForcarClicarElemento(driver, botaoLogin);
            await driver.WaitForSelectorAsync("[class='ant-card-body']");
        }
    }
}
