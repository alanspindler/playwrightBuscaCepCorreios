# Requisitos para execução

 Abrir a solução no Visual Studio 2019/2022
 
 Instalar o componente individual .Net Core 3.1
 
 Efetuar o build da solução
 
 Exibir o Developer PowerShell. Para isso, acesse View(Visualizar) - Terminal
 
 Executar o comando: cd "C:\Pastadoprojeto\TesteBuscaCorreios\bin\Debug\netcoreapp3.1" 
 renomeando a "pastadoprojeto" pelo o caminho do projeto
 
 Executar o comando: pwsh playwright.ps1 install
 
 Caso esse comando gere o erro: "pwsh : O termo 'pwsh' não é reconhecido como nome de cmdlet, função, arquivo de script ou programa operável."
 Será necessário atualizar o PowerShell para a versão 6.0 ou superior.
 
 https://github.com/PowerShell/PowerShell/releases/download/v7.3.0/PowerShell-7.3.0-win-x64.msi

 Após isso, reinicie o Visual Studio e repita esse passo.
 
 Esse comando irá instalar os navegadores utilizados pelo Playwright
 
 Acessar o Menu Test - Test Explorer
 
 Clicar com o botão direito sobre os testes e clicar em Run (Executar).
 
 Obs: Os testes foram criados para execução paralela. Para isso, é necessário efetuar o comando Run na suite de testes,  
 ou selecionando multiplos testes.
 
 Para execução individual, roda o comando Run individualmente nos testes
 
 

