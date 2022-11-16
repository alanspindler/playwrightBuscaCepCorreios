using System;
using System.Collections.Generic;
using System.Text;

namespace PlaywrightAutomacao
{
    public class PageObjectsProdutosDisponiveis : MetodosAuxiliares
    {
        #region Campo filtros (tela 'Disponíveis')
        public string botaoFiltros = "[class='ant-btn ant-btn-primary fixed-filter-button']";
        public string botaoLimpar = "[class='ant-btn btn-clear']";
        public string campoCodigoProduto = "[id=idsProducts]";
        public string campoReferenciaProduto = "[id=refsProducts]";
        public string campoNomeProduto = "[id=name]";
        public string campoMarcaProduto = "[id=idsBrands]";
        public string campoCategoriaProduto = "[id=idsCategories]";
        public string campoCanalProduto = "[id=idsChannels]";
        public string campoStatusProduto = "xpath=//div[2]/div/div/div[2]/div/span/div/div/span[2]";
        public string botaoBuscarFiltros = "button:has-text(\"Buscar\")";
        public string botaoFecharFiltros = "[aria-label=\"Close\"]";
        public string itemIt4SolutionMarcaProduto = "[title='IT4 Solution'] >> nth=0";
        public string itemLeonardoMarcaProduto = "[title='leonardo'] >> nth=0";
        public string itemJoiasCategoriaProduto = "[title='Jóias'] >> nth=0";
        public string itemRoupasCategoriaProduto = "[title='Roupas'] >> nth=0";
        public string itemSimProduto = "[title='Sim'] >> nth=0";
        public string campoApenasProdutoSemCanal = "[class='ant-select-selection-item']:has-text(\"Não\")";
        public string itemIt4SolutionVtexCanalProduto = "[class='ant-select-item-option-content']:has-text(\"IT4Solution VTEX\") >> nth=0";
        public string itemIt4MagentoCanalProduto = "[class='ant-select-item-option-content']:has-text(\"IT4Solution Magento\") >> nth=0";
        public string itemStatusProdutoDesabilitado = "[class='ant-select-item-option-content']:has-text(\"Desabilitado\") >> nth=0";
        public string itemStatusProdutoHabilitado = "[class='ant-select-item-option-content']:has-text(\"Habilitado\") >> nth=0";
        #endregion

        #region Campos da tela de Produtos Disponíveis
        public string botaoCadastrarProduto = "button:has-text(\"Cadastrar Produto\")";
        public string botaoAtualizarEstoque = "button:has-text(\"Atualizar Estoque\")";
        public string botaoAtualizarProdutos = "button:has-text(\"Atualizar Produtos\")";
        public string botaoEnviarFotos = "button:has-text(\"Enviar fotos\")";
        public string botaoExportarPlanilha = "button:has-text(\"Exportar planilha\")";
        public string botaoExportarProdutos = "button:has-text(\"Exportar Produtos\")";
        public string textoCabeçalhoImagem = "tr:nth-child(1) > .ant-table-cell:nth-child(2)";
        public string textoCabeçalhoCodigo = "tr:nth-child(1) > .ant-table-cell:nth-child(3)";
        public string textoCabeçalhoNome = "tr:nth-child(1) > .ant-table-cell:nth-child(4)";
        public string textoCabeçalhoMarca = "tr:nth-child(1) > .ant-table-cell:nth-child(5)";
        public string textoCabeçalhoCategoria = "tr:nth-child(1) > .ant-table-cell:nth-child(6)";
        public string textoCabeçalhoQtdSkus = "tr:nth-child(1) > .ant-table-cell:nth-child(7)";
        public string textoCabeçalhoCanais= "tr:nth-child(1) > .ant-table-cell:nth-child(8)";
        public string textoCabeçalhoStatus = "tr:nth-child(1) > .ant-table-cell:nth-child(9)";
        public string botaoUpload = "button:has-text(\"Upload\")";
        public string botaoFecharModal = "[class='anticon anticon-close ant-modal-close-icon']";
        public string botaoEnviarParaCanal = "button:has-text(\"Enviar p/ canal\") >> nth = 0";
        public string botaoEnviarSelecionadosAoCanal = "button:has-text(\"Enviar p/ canal\") >> nth = 1";
        public string botaoCheckBox = "[class='ant-table-cell ant-table-selection-column'] >> nth = 0";
        public string botaoVisualizarProduto = "[class='anticon anticon-select'] >> nth=0";
        public string botaoAcoes = "[class='anticon anticon-ellipsis ant-dropdown-trigger ic-config'] >> nth=0";
        public string divCarregando = "[class='ant-spin']";
        public string botaoRemover = "[class='btn-remove-refuse']:has-text(\"Remover\") >> nth=0";
        public string botaoConfirmar = "button:has-text(\"Confirmar\")";
        public string cabecalhoTabela = "[class='ant-table-thead']";
        public string listaDeCanais = "id=idChannel";
        public string itemVtexListaDeCanais = "[class='ant-select-item-option-content']:has-text(\"IT4Solution VTex\") >> nth=0";
        public string notificacaoSucessoEnvioProdutoCanal = "[class='ant-notification-notice-description']:has-text(\"Foram enviados para o canal 1 produtos e outros 0 produtos já estão no canal.\")";
        public string notificacaoFalhaEnvioProdutoCanal = "[class='ant-notification-notice-description']:has-text(\"Esse(s) produto(s) já existe(m) neste canal ou não possue(m) SKU(s).\")";
        public string menuDoBotaoAcoes = "[class='ant-dropdown-menu ant-dropdown-menu-light ant-dropdown-menu-root ant-dropdown-menu-vertical']";
        #endregion

        #region Campos da tela de cadastro de produto
        public string botaoCadastrar = "button:has-text(\"Cadastrar\")";
        public string campoNomeCadastroProduto = "[id=name]";
        public string campoReferenciaCadastroProduto = "[id=refProduct]";
        public string campoMarcaCadastroProduto = "[id=idBrand]";
        public string campoCategoriaCadastroProduto = "[id=idCategory]";
        public string campoDescricaoLongaCadastroProduto = "[id=longDescription]";
        public string campoDescricaoCurtaCadastroProduto = "[id=shortDescription]";
        public string campoMetaTagsCadastroProduto = "[id=metaTags]";
        public string campoPalavraChaveCadastroProduto = "[id=keyWords]";
        public string mensagemCampoObrigatorioNome = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher o nome!\")";
        public string mensagemCampoObrigatorioReferencia = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher a REF!\")";
        public string mensagemCampoObrigatorioMarca = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher o campo Marca!\")";
        public string mensagemCampoObrigatorioCategoria = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher a Categoria!\")";
        public string mensagemCampoObrigatorioDescricaoLonga = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher a descrição longa!\")";
        public string mensagemCampoObrigatorioDescricaoCurta = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher a descrição curta!\")";
        public string mensagemCampoObrigatorioMetaTags = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher as Meta Tags!\")";
        public string mensagemCampoObrigatorioPalavrasChave = "[class='ant-legacy-form-explain']:has-text(\"Favor, preencher as palavras chave!\")";
        public string notificacaoSucessoCriacaoProduto = "[class='ant-notification-notice-description']:has-text(\"Produto cadastrado com sucesso\")";
        public string botaoEditarProduto = "[class='ant-btn ant-btn-link']:has-text(\"Editar produto\")";
        public string itemCamisetasCategoriaCriacaoProduto = "[class='ant-select-item-option-content']:has-text(\"Camisetas\") >> nth=0";
        public string itemIt4SolutionMarcaCriacaoProduto = "[class='ant-select-item-option-content']:has-text(\"IT4 Solution\") >> nth=0";
        public string campoMarcaProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item']:has-text(\"IT4 Solution\")";
        public string campoCategoriaProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item']:has-text(\"Camisetas\")";
        public string campoMetaTagsProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item-content']:has-text(\"TesteAutomatizado\") >> nth=1";
        public string campoPalavrasChaveProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item-content']:has-text(\"TesteAutomatizado\") >> nth=0";
        public string botaoAtualizar = "button:has-text(\"Atualizar\")";
        #endregion

        #region Campos da tela de cadastro de sku
        public string botaoAdicionarSku = "button:has-text(\"Adicionar SKU\")";
        public string botaoOk = "button:has-text(\"OK\") >> nth=0";
        public string campoReferenciaCadastroSku = "[id=refSku]";
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
        public string mensagemCampoObrigatorioReferenciaSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher a REF!\")";
        public string mensagemCampoObrigatorioEanSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher EAN!\")";
        public string mensagemCampoObrigatorioPrecoDeSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher um preço válido!\") >> nth = 0";
        public string mensagemCampoObrigatorioPrecoPorSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher um preço válido!\") >> nth = 1";
        public string mensagemCampoObrigatorioDescricaoSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher a Descrição!\") >> nth = 0";
        //public string mensagemCampoObrigatorioPesoSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher o Peso!\")";
        public string mensagemCampoObrigatorioAlturaSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher a Altura!\") >> nth = 0";
        public string mensagemCampoObrigatorioLarguraSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher a Largura!\") >> nth = 0";
        //public string mensagemCampoObrigatorioComprimentoSku = "[class='ant-form-item-explain ant-form-item-explain-error']:has-text(\"Favor, preencher o Comprimento!\")";
        public string notificacaoSucessoCriacaoSku = "[class='ant-notification-notice-description']:has-text(\"Sku criado com sucesso!\")";
        public string labelSku1 = "[class='ant-collapse ant-collapse-icon-position-left content-collapse-sku']:has-text(\"Descricao sku para teste automatizado\")";
        public string labelSku2 = "[class='ant-collapse ant-collapse-icon-position-left content-collapse-sku']:has-text(\"Descricao sku para teste automatizado 2\")";
        public string botaoEditar1 = "[class='ant-collapse ant-collapse-icon-position-left content-collapse-sku']:has-text(\"Primeiro sku para teste automatizado\") >> button:has-text(\"Editar\")";
        public string botaoEditar2 = "[class='ant-collapse ant-collapse-icon-position-left content-collapse-sku']:has-text(\"Segundo sku para teste automatizado\") >> button:has-text(\"Editar\")";
        public string botaoAdicionarImagemPorUrl = "button:has-text(\"Adicionar imagem por url\")";
        public string campoUrlImagem = "[id=images_0_url]";
        public string campoNomeImagem = "[id=images_0_name]";
        public string campoDescricaoImagem = "[id=images_0_description]";
        public string botaoAdicionarImagemPorComputador = "button:has-text(\"Adicionar imagem do computador\")";
        //public string localDeArrasteParaImagem = "//p[contains(.,'Clique aqui ou arraste um arquivo para realizar o upload')]";
        public string imagemSku = "[class='sc-fKVqWL eOjNfz  drop-over'] >> img";
        public string localDeArrasteParaImagem = "[class='ant-upload ant-upload-drag']";
        #endregion

        #region Campos tela de edição de produto
        public string campoMarcaEdicaoProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item']:has-text(\"leonardo\")";
        public string campoCategoriaEdicaoProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item']:has-text(\"Jóias\")";
        public string campoMetaTagsEdicaoProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item-content']:has-text(\"MetaTagsEdit\") >> nth=0";
        public string campoPalavrasChaveEdicaoProdutoVerificacaoDisponiveis = "[class='ant-select-selection-item-content']:has-text(\"PalavraChaveEdit\") >> nth=0";
        public string notificacaoSucessoAtualizaçãoProduto = "[class='ant-notification-notice-description']:has-text(\"Produto atualizado com sucesso!\")";
        public string botaoRessincronizarComOCanal = "button:has-text(\"Sincronizar alterações nos canais\")";
        #endregion

        #region
        public string botaoEditarSku = "button:has-text(\"Editar\")";
        public string notificacaoSucessoAtualizaçãoSku = "[class='ant-notification-notice-description']:has-text(\"SKU atualizado com sucesso!\")";
        #endregion
    }
}
