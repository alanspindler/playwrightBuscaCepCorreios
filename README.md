# Requisitos para execução

 Abrir a solução no Visual Studio 2019/2022
 
 Instalar o componente individual .Net Core 3.1
 
 Efetuar o build da solução
 
 Exibir o Developer PowerShell. Para isso, acesse View(Visualizar) - Terminal
 
 Executar o comando: cd "C:\Pastadoprojeto\TesteBuscaCorreios\bin\Debug\netcoreapp3.1" 
 renomeando a "pastadoprojeto" pelo o caminho do projeto
 
 Executar o comando: pwsh playwright.ps1 install
 
 Esse comando irá instalar os navegadores utilizados pelo Playwright
 
 Acessar o Menu Test - Test Explorer
 
 Clicar com o botão direito sobre os testes e clicar em Run (Executar).
 
 Obs: Os testes foram criados para execução paralela. Para isso, é necessário efetuar o comando Run na suite de testes,  
 ou selecionando multiplos testes.
 
 Para execução individual, roda o comando Run individualmente nos testes

