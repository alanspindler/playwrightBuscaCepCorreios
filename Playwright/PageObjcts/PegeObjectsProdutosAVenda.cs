using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightAutomacao
{
    public class PageObjectsProdutosAVenda : MetodosAuxiliares
    {
        #region Campo filtros (tela 'A Venda')
        public string botaoFiltros = "[class='ant-btn ant-btn-primary fixed-filter-button']";
        public string campoCodigoProduto = "[id=idsProducts]";
        public string campoReferenciaProduto = "[id=refsProducts]";
        public string campoNomeProduto = "[id=name]";
        public string campoMarcaProduto = "[id=idsBrands]";
        public string campoCategoriaProduto = "[id=idsCategories]";
        public string campoCanalProduto = "[id=idsChannels]";
        public string campoStatusProduto = "[id=status]";
        public string campoStatusAtualizacaoProduto = "[id=updateStatus]";
        public string botaoBuscar = "button:has-text(\"Buscar\")";
        public string botaoFechar = "[aria-label=\"Close\"]";
        public string itemMarcaProduto = "[title='Teste do Alan Legal'] >> nth=0";
        public string itemCategoriaProduto = "[title='ACUO Canal'] >> nth=0";
        public string botaoBuscarFiltros = "button:has-text(\"Buscar\")";
        public string botaoFecharFiltros = "[aria-label=\"Close\"]";
        public string botaoLimpar = "[class='ant-btn btn-clear']";
        public string itemIt4SolutionMarcaProduto = "[title='IT4 Solution'] >> nth=0";
        public string itemAnyMarketMarcaProduto = "[title='MarcaTesteAnyMarket'] >> nth=0";
        public string itemRoupasCategoriaProduto = "[title='Roupas'] >> nth=0";
        public string itemTesteCategoriaProduto = "[title='Teste'] >> nth=0";
        public string itemIt4SolutionVtexCanalProduto = "[class='ant-select-item-option-content']:has-text(\"IT4Solution VTEX\") >> nth=0";
        public string itemIt4MagentoCanalProduto = "[class='ant-select-item-option-content']:has-text(\"IT4Solution Magento\") >> nth=0";
        public string itemHigienePessoalCategoriaProduto = "[title='Beleza > Higiene Pessoal'] >> nth=0";
        public string itemStatusProdutoNaoCurado = "[class='ant-select-item-option-content']:has-text(\"Não curado\") >> nth=0";
        public string itemStatusProdutoCurado = "[class='ant-select-item-option-content']:has-text(\"Curado\") >> nth=1";
        public string itemStatusProdutoSincronizando = "[class='ant-select-item-option-content']:has-text(\"Sincronizando\") >> nth=0";
        public string itemStatusProdutoErroNaIntegracao = "[class='ant-select-item-option-content']:has-text(\"Erro na integração\") >> nth=0";
        public string itemStatusProdutoAtivo = "[class='ant-select-item-option-content']:has-text(\"Ativo\") >> nth=0";
        public string itemStatusProdutoDesabilitado = "[class='ant-select-item-option-content']:has-text(\"Desabilitado\") >> nth=0";
        public string itemStatusProdutoNaoSincronizado = "[class='ant-select-item-option-content']:has-text(\"Não sincronizado\") >> nth=0";
        public string itemStatusProdutoSincronizado = "[class='ant-select-item-option-content']:has-text(\"Sincronizado\") >> nth=0";
        #endregion

        #region Campos da tela de Produtos A Venda
        public string divCarregando = "[class='ant-spin']";
        public string botaoVisualizarProduto = "[class='anticon anticon-select'] >> nth=0";
        public string botaoMaximizarMenu = "[class='anticon anticon-menu-unfold trigger']";
        public string botaoMenuProdutos = "[class='ant-menu-submenu-title']:has-text(\"Produtos\")";
        public string botaoMenuAVenda = "[class='ant-menu-item ant-menu-item-only-child ant-menu-item-selected']:has-text(\"A venda\")";
        public string botaoAtualizarProduto = "button:has-text(\"Atualizar Produtos via Planilha\")";
        public string botaoExportarPlanilha = "button:has-text(\"Exportar Planilha\")";
        public string botaoUpload = "button:has-text(\"Upload\")";
        public string botaoFecharJanela = "[class='anticon anticon-close ant-modal-close-icon']";
        public string botaoCheckBox = "[class='ant-table-cell ant-table-selection-column'] >> nth = 0";
        public string textoCabeçalhoImagem = "tr:nth-child(1) > .ant-table-cell:nth-child(2)";
        public string textoCabeçalhoCodigo = "tr:nth-child(1) > .ant-table-cell:nth-child(3)";
        public string textoCabeçalhoNome = "tr:nth-child(1) > .ant-table-cell:nth-child(4)";
        public string textoCabeçalhoMarca = "tr:nth-child(1) > .ant-table-cell:nth-child(5)";
        public string textoCabeçalhoCategoria = "tr:nth-child(1) > .ant-table-cell:nth-child(6)";
        public string textoCabeçalhoCanal = "tr:nth-child(1) > .ant-table-cell:nth-child(7)";
        public string textoCabeçalhoEmpresa = "tr:nth-child(1) > .ant-table-cell:nth-child(8)";
        public string textoCabeçalhoStatus = "tr:nth-child(1) > .ant-table-cell:nth-child(9)";
        public string textoCabeçalhoStatusAtualizacao = "tr:nth-child(1) > .ant-table-cell:nth-child(10)";
        public string labelQuadroStatusProduto = "[class='sc-iqseJM liOrHP']:has-text(\"Resumo de status do produto\")";
        public string labelQuadroStatusNaoCurado = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Não curado\") >> nth = 0";
        public string labelQuadroStatusCurado = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Curado\") >> nth = 0";
        public string labelQuadroStatusSincronizando = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Sincronizando\") >> nth = 0";
        public string labelQuadroStatusAtivo = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Ativo\") >> nth = 0";
        public string labelQuadroStatusErroNaIntegração = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Erro na integração\") >> nth = 0";
        public string labelQuadroStatusDesabilitado = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Desabilitado\") >> nth = 0";
        public string labelQuadroStatusAtualizacaoNaoSincronizado = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Não sincronizado\") >> nth = 0";
        public string labelQuadroStatusAtualizacaoSincronizando = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Sincronizando\") >> nth = 1";
        public string labelQuadroStatusAtualizacaoSincronizado = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Sincronizado\") >> nth = 0";
        public string labelQuadroStatusAtualizacaoErroNaIntegracao = "[class='ant-col sc-egiyK dBvtrD ant-col-xs-12 ant-col-md-10 ant-col-xl-5']:has-text(\"Erro na integração\") >> nth = 1";
        public string labelQuadroStatusAtualizacaoProduto = "[class='sc-iqseJM liOrHP']:has-text(\"Resumo de status de atualização\")";
        public string botaoAcoes = "[class='anticon anticon-ellipsis ant-dropdown-trigger ic-config'] >> nth=0";
        public string menuDoBotaoAcoes = "[class='ant-dropdown-menu ant-dropdown-menu-light ant-dropdown-menu-root ant-dropdown-menu-vertical']";
        public string botaoEditarInternoProduto = "[class='ant-btn ant-btn-link']:has-text(\"Editar produto\")";
        public string labelNomeDoProduto = "[class='label term']:has-text(\"Nome\") >> nth = 0";
        public string botaoEditarProduto = ".ant-dropdown-menu-item:nth-child(1):has-text(\"Editar\")";
        public string botaoRemoverProduto = ".ant-dropdown-menu-item:nth-child(2):has-text(\"Remover\")";
        public string botaoDesabilitarProduto = ".ant-dropdown-menu-item:nth-child(3):has-text(\"Desabilitar\")";
        public string botaoHabilitarProduto = ".ant-dropdown-menu-item:nth-child(4):has-text(\"Habilitar\")";
        public string botaoSincronizarAlteracoesProduto = ".ant-dropdown-menu-item:nth-child(5):has-text(\"Sincronizar alterações\")";

        #endregion

        #region Campos tela de edição do produto
        public string campoNomeCadastroProduto = "[id=name]";
        public string campoMarcaCadastroProduto = "[id=idChannelBrand]";
        public string campoCategoriaCadastroProduto = "[id=idChannelCategory]";
        public string campoDescricaoLongaCadastroProduto = "[id=longDescription]";
        public string campoDescricaoCurtaCadastroProduto = "[id=shortDescription]";
        public string campoMetaTagsCadastroProduto = "[id=metaTags]";
        public string campoPalavraChaveCadastroProduto = "[id=keyWords]";
        public string botaoMenuAtributos = "[class='ant-collapse-header']:has-text(\"Atributos do produto\")";
        public string campoAtributoSelecionavelProdutoCategSaude = "[id=channelProductAttributes_0216e13d-1be0-42d2-9371-695cf7f56c06_idChannelAttributeValue]";
        public string campoAtributoCampoAbertoProdutoCategSaude = "[id=channelProductAttributes_8e04cf35-2d12-47c4-84e1-133bdbe2b625_openFieldValue]";
        public string campoAtributoMultivaloradoProdutoCategSaude = "[id=channelProductAttributes_e204c45c-0d22-4f00-894a-717c6e3315ad_idChannelAttributeValue]";
        public string campoVerificacaoAtributoSelecionavelCategSaude = "[class='ant-select-selection-item']:has-text(\"ValorAtributoSelecionavel1_3\")";
        public string campoVerificacaoAtributoMultivaloradoCategSaude = "[class='ant-select-selection-item-content']:has-text(\"ValorAtributoMultivalorado2\")";
        public string itemValorAtributoSelecionavel1_3ProdutoCategSaude = "[class='ant-select-item-option-content']:has-text(\"ValorAtributoSelecionavel1_3\")";
        public string itemValorAtributoMultivalorado2ProdutoCategSaude = "[class='ant-select-item-option-content']:has-text(\"ValorAtributoMultivalorado2\")";
        public string campoAtributoSelecionavelProdutoCategEletronicos = "[id=channelProductAttributes_330f7920-5fcc-43a5-8724-69334574f765_idChannelAttributeValue]";
        public string campoAtributoCampoAbertoProdutoCategEletronicos = "[id=channelProductAttributes_00b1aed7-a641-4cc3-bf1d-893a38c0c3ee_openFieldValue]";
        public string campoAtributoMultivaloradoProdutoCategEletronicos = "[id=channelProductAttributes_2aee5aee-95cd-4c5f-9bef-685e7f47823e_idChannelAttributeValue]";
        public string campoVerificacaoAtributoSelecionavelCategEletronicos = "[class='ant-select-selection-item']:has-text(\"ValorAtributoSelecionavel1_3\")";
        public string campoVerificacaoAtributoMultivaloradoCategEletronicos = "[class='ant-select-selection-item-content']:has-text(\"ValorAtributoMultivalorado1_2\")";
        public string itemValorAtributoSelecionavel1_3ProdutoCategEletronicos = "[class='ant-select-item-option-content']:has-text(\"ValorAtributoSelecionavel1_3\")";
        public string itemValorAtributoMultivalorado1_2ProdutoCategEletronicos = "[class='ant-select-item-option-content']:has-text(\"ValorAtributoMultivalorado1_2\")";
        public string botaoAtualizar = "button:has-text(\"Atualizar\")";
        public string mensagemCampoObrigatorioCampoAbertoProduto = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Este campo é obrigatório\") >> nth = 0";
        public string mensagemCampoObrigatorioMultivaloradoProduto = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Este campo é obrigatório\") >> nth = 1";
        public string mensagemCampoObrigatorioSelecionavelProduto = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Este campo é obrigatório\") >> nth = 2";
        public string notificacaoSucessoEnvioProdutoCanal = "[class='ant-notification-notice-description']:has-text(\"Produto atualizado com sucesso\")";
        public string botaoEditarSku = "[class='ant-collapse ant-collapse-icon-position-left content-collapse-sku']:has-text(\"Primeiro sku para teste automatizado\") >> button:has-text(\"Editar\")";
        public string itemLeonardoMarcaProduto = "[title='leonardo'] >> nth=0";
        public string itemEletronicosCategoriaProduto = "[title='Eletronicos'] >> nth=0";
        public string notificacaoSucessoAtualizaçãoProduto = "[class='ant-notification-notice-description']:has-text(\"Produto atualizado com sucesso\")";
        public string campoVerificacaoMetaTagsEdicaoProduto = "[class='ant-select-selection-item-content']:has-text(\"MetaTagEditada\") >> nth=0";
        public string campoVerificacaoPalavraChaveEdicaoProduto = "[class='ant-select-selection-item-content']:has-text(\"PalavraChaveEditada\") >> nth=0";
        public string checkBoxCurado = "[id=cured]";
        #endregion

        #region Campos tela de edição do Sku
        public string campoEanCadastroSku = "[id=ean]";
        public string campoPrecoDeCadastroSku = "[id=priceOf]";
        public string campoPrecoPorCadastroSku = "[id=priceBy]";
        public string campoDescricaoCadastroSku = "[id=description]";
        public string campoPesoCadastroSku = "[id=weight]";
        public string campoPesoRealCadastroSku = "[id=realWeight]";
        public string campoPesoCubicoCadastroSku = "[id=cubicWeight]";
        public string campoAlturaCadastroSku = "[id=height]";
        public string campoAlturaRealCadastroSku = "[id=realHeight]";
        public string campoLarguraCadastroSku = "[id=width]";
        public string campoLarguraRealCadastroSku = "[id=realWidth]";
        public string campoComprimentoCadastroSku = "[id=length]";
        public string campoComprimentoRealCadastroSku = "[id=realLength]";
        public string botaoEditarSkuTesteAutoAVendaDescricao = "[class='ant-row ant-row-space-between ant-row-middle']:has-text(\"TesteAutoAVendaDescricao\") >> button:has-text(\"Editar\")";
        public string botaoEditarSkuTesteAutoDescEditado = "[class='ant-row ant-row-space-between ant-row-middle']:has-text(\"TesteAutoDescEditado\") >> button:has-text(\"Editar\")";
        public string campoAtributoCampoAbertoSku = "xpath=(//input[@id='attributes.8e04cf35-2d12-47c4-84e1-133bdbe2b625.openFieldValue'])[2]";
        public string campoAtributoMultivaloradoSku = "xpath=(//input[@id='attributes.e204c45c-0d22-4f00-894a-717c6e3315ad.idChannelAttributeValue'])[2]";
        public string campoAtributoSelecionavelSku = "xpath=(//input[@id='attributes.0216e13d-1be0-42d2-9371-695cf7f56c06.idChannelAttributeValue'])[2]";
        public string itemValorAtributoSelecionavel1_3Sku = "[class='ant-select-item-option-content']:has-text(\"ValorAtributoSelecionavel1_3\")";
        public string itemValorAtributoMultivalorado2Sku = "[class='ant-select-item-option-content']:has-text(\"ValorAtributoMultivalorado2\") >> nth = 1";
        public string itemValorAtributoMultivalorado1_2Sku = "[class='ant-select-item-option-content']:has-text(\"ValorAtributoMultivalorado1_2\") >> nth = 1";
        public string mensagemCampoObrigatorioCampoAbertoSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Este campo é obrigatório\") >> nth = 0";
        public string mensagemCampoObrigatorioMultivaloradoSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Este campo é obrigatório\") >> nth = 1";
        public string mensagemCampoObrigatorioSelecionavelSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Este campo é obrigatório\") >> nth = 2";
        public string botaoAtualizarSku = "button:has-text(\"Atualizar\") >> nth = 1";
        public string notificacaoSucessoAtualizaçãoSku = "[class='ant-notification-notice-description']:has-text(\"SKU atualizado com sucesso\")";
        public string campoAtributoMultivaloradoSkuVerificacao = "[class='ant-select-selection-item-content']:has-text(\"ValorAtributoMultivalorado2\") >> nth = 2";
        public string campoAtributoSelecionavelSkuVerificacao = "[class='ant-select-selection-item']:has-text(\"ValorAtributoSelecionavel1_3\") >> nth = 2";
        public string campoAtributoMultivaloradoSkuCategEletronicos = "xpath=(//input[@id='attributes.2aee5aee-95cd-4c5f-9bef-685e7f47823e.idChannelAttributeValue'])[2]";
        public string campoAtributoSelecionavelSkuCategEletronicos = "xpath=(//input[@id='attributes.330f7920-5fcc-43a5-8724-69334574f765.idChannelAttributeValue'])[2]";
        public string campoAtributoCampoAbertoSkuCategEletronicos = "xpath=(//input[@id='attributes.00b1aed7-a641-4cc3-bf1d-893a38c0c3ee.openFieldValue'])[2]";
        public string campoAtributoMultivaloradoSkuCategEletronicosVerificacao = "[class='ant-select-selection-item-content']:has-text(\"ValorAtributoMultivalorado1_2\") >> nth = 2";
        public string campoAtributoSelecionavelSkuCategEletronicosVerificacao = "[class='ant-select-selection-item']:has-text(\"ValorAtributoSelecionavel1_3\") >> nth = 2";
        #endregion
    }
}