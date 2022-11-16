
using System;

namespace PlaywrightAutomacao
{
    public class FuncoesBancoIt4360Stg : ConexaoBancoIt4360Stg
    {

        string[] refsProdutosBase = { "RefTesteAutomatizado", "RefTesteAutoParaEnvioDeProdutos", "RefTesteAutomatizadoEdicao", "ReferenciaSemSkuParaEnvio", "RefTesteParaEnvioDeUmProduto", "TesteAutoAVenda" };
        string[] refsSkusBase = { "ReferenciaSkuTesteAutomatizado", "ReferenciaSkuTesteAutomatizado2", "ReferenciaSkuTesteAutomatizadoParaEnvio", "RefSkuTesteAutomatizadoEdicao", "ReferenciaSkuTesteEnvioDeUmProduto", "TesteAutoAVendaSku" };
        string[] descricoesImagens = { "DescricaoImagemSku" };

        public void ExcluirProdutoDisponivel()
        {
            foreach (string descricaoImagem in descricoesImagens)
            {
                ExecutarComandoSQL($"DELETE FROM SkuImage WHERE [Description] = '{descricaoImagem}'");
            }
            foreach (string refSkuBase in refsSkusBase)
            {
                ExecutarComandoSQL($"DELETE FROM Sku WHERE RefSku= '{refSkuBase}'");
            }
            foreach (string refProdutoBase in refsProdutosBase)
            {
                ExecutarComandoSQL($"DELETE FROM Product WHERE RefProduct = '{refProdutoBase}'");
            }
        }

        public void ExcluirProdutoDisponivel(string refProdutoBase, string? refSkuBase = null, string? descricaoImagem = null)
        {
            if (descricaoImagem != null)
            {
                ExecutarComandoSQL($"DELETE FROM SkuImage WHERE [Description] = '{descricaoImagem}'");
                ExecutarComandoSQL($"DELETE FROM Sku WHERE RefSku= '{refSkuBase}'");
                ExecutarComandoSQL($"DELETE FROM Product WHERE RefProduct = '{refProdutoBase}'");
            }
            else if (refsSkusBase != null)
            {
                ExecutarComandoSQL($"DELETE FROM Sku WHERE RefSku= '{refSkuBase}'");
                ExecutarComandoSQL($"DELETE FROM Product WHERE RefProduct = '{refProdutoBase}'");
            }
            else
            {
                ExecutarComandoSQL($"DELETE FROM Product WHERE RefProduct = '{refProdutoBase}'");
            }
        }


        string[] refsProdutosCanal = { "RefTesteAutoParaEnvioDeProdutos", "RefTesteAutomatizadoEdicao", "RefTesteParaEnvioDeUmProduto", "TesteAutoAVenda" };
        string[] refsSkusCanal = { "ReferenciaSkuTesteAutomatizadoParaEnvio", "ReferenciaSkuTesteEnvioDeUmProduto", "TesteAutoAVendaSku" };

        public void ExcluirProdutoAVenda()
        {
            // As referências do canal, neste teste, são iguais as visualizadas em disponíveis
            foreach (string refSkuCanal in refsSkusCanal)
            {
                ExecutarComandoSQL($"DELETE FROM ChannelSkuAttributeValue WHERE IdChannelSkuAttribute in (SELECT Id FROM ChannelSkuAttribute WHERE IdChannelSku in (SELECT Id FROM ChannelSku  WHERE IdSku in (SELECT Id FROM Sku WHERE RefSku = '{refSkuCanal}')))");
            }
            foreach (string refProdutoCanal in refsProdutosCanal)
            {
                ExecutarComandoSQL($"DELETE FROM ChannelProductAttributeValue WHERE IdChannelProductAttribute in (SELECT Id FROM ChannelProductAttribute WHERE IdChannelProduct in (SELECT Id FROM ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}')))");
            }
            foreach (string refSkuCanal in refsSkusCanal)
            {
                ExecutarComandoSQL($"DELETE FROM ChannelSkuAttribute WHERE IdChannelSku in (SELECT Id FROM ChannelSku  WHERE IdSku in (SELECT Id FROM Sku WHERE RefSku = '{refSkuCanal}'))");
            }
            foreach (string refProdutoCanal in refsProdutosCanal)
            {
                ExecutarComandoSQL($"DELETE FROM ChannelProductAttribute WHERE IdChannelProduct in (SELECT Id from ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}'))");
            }
            foreach (string refProdutoCanal in refsProdutosCanal)
            {
                ExecutarComandoSQL($"DELETE FROM ChannelProductState where IdChannelProduct in (SELECT Id from ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}'))");
            }
            foreach (string refSkuCanal in refsSkusCanal)
            {
                ExecutarComandoSQL($"DELETE FROM ChannelSku WHERE IdSku in (SELECT Id FROM Sku WHERE RefSku = '{refSkuCanal}')");
            }
            foreach (string refProdutoCanal in refsProdutosCanal)
            {
                ExecutarComandoSQL($"DELETE FROM ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}')");
            }
        }

        public void ExcluirProdutoAVenda(string refProdutoCanal, string refSkuCanal)
        {
            ExecutarComandoSQL($"DELETE FROM ChannelSkuAttributeValue WHERE IdChannelSkuAttribute in (SELECT Id FROM ChannelSkuAttribute WHERE IdChannelSku in (SELECT Id FROM ChannelSku  WHERE IdSku in (SELECT Id FROM Sku WHERE RefSku = '{refSkuCanal}')))");
            ExecutarComandoSQL($"DELETE FROM ChannelProductAttributeValue WHERE IdChannelProductAttribute in (SELECT Id FROM ChannelProductAttribute WHERE IdChannelProduct in (SELECT Id FROM ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}')))");
            ExecutarComandoSQL($"DELETE FROM ChannelSkuAttribute WHERE IdChannelSku in (SELECT Id FROM ChannelSku  WHERE IdSku in (SELECT Id FROM Sku WHERE RefSku = '{refSkuCanal}'))");
            ExecutarComandoSQL($"DELETE FROM ChannelProductAttribute WHERE IdChannelProduct in (SELECT Id from ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}'))");
            ExecutarComandoSQL($"DELETE FROM ChannelProductState where IdChannelProduct in (SELECT Id from ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}'))");
            ExecutarComandoSQL($"DELETE FROM ChannelSku WHERE IdSku in (SELECT Id FROM Sku WHERE RefSku = '{refSkuCanal}')");
            ExecutarComandoSQL($"DELETE FROM ChannelProduct WHERE IdProduct in (SELECT Id FROM Product WHERE RefProduct = '{refProdutoCanal}')");
        }

        public void CriarProduto(string nomeProduto, string referenciaProduto, string referenciaSku, string eanSku)
        {
            var idProdutoDisponivel = Guid.NewGuid().ToString();
            var queryCriacaoProduto = $@"
                INSERT INTO Product 
	                  (
	                    Id,
		                IdCompany,
		                IdBrand,
		                IdCategory,
		                [Name],
		                ShortDescription,
		                LongDescription,
		                [Url], 
                        MetaTags,
		                KeyWords,
		                RefProduct,
		                [Status],
		                CreatedAt,
		                HasChangesToSynchronize
	                  )
                VALUES(
		                '{idProdutoDisponivel}',
	                    '50d9dfe9-7fcb-4d59-ba5d-058815d2b380',
	                    '50d9zxc9-7fcb-4d59-ba5d-058815d2b386',
	                    '50d9zxc9-7fcb-4d59-ba5d-058815d2j316',
	                    '{nomeProduto}',
	                    'TesteAutomatizadoAVenda Descricao Curta',
	                    'TesteAutomatizadoAVenda Descricao Longa',
	                    'TesteAutomatizadoAVenda',
	                    'TesteAutomatizado',
	                    'TesteAutomatizado',
                        '{referenciaProduto}',
	                    1,
	                    GETDATE(),
	                    0
	                  )";
            try
            {
                ExecutarComandoSQL(queryCriacaoProduto);
                CriarSkuDiponivel(idProdutoDisponivel, nomeProduto, referenciaProduto, referenciaSku, eanSku);
            }
            catch (Exception ex)
            {
                GravarLogErro(ex.Message.ToString());
                ExcluirProdutoDisponivel(referenciaProduto);
            }
        }

        public void CriarSkuDiponivel(string idProdutoDisponivel, string nomeProduto, string referenciaProduto, string referenciaSku, string eanSku)
        {
            var idSkuProdutoDisponivel = Guid.NewGuid().ToString();
            var queryCriacaoSkuDisponivel = $@"
                INSERT INTO Sku
	                   (
		                  Id,
		                  IdProduct,
		                  [Description],
		                  RefSku,
		                  [Weight],
		                  Height,
		                  Width,
		                  [Length],
		                  Ean,
		                  CreatedAt,
		                  PriceOf,
		                  PriceBy,
		                  IdCompany,
		                  Position
	                   )
                VALUES 
	                   (
		                  '{idSkuProdutoDisponivel}',
		                  '{idProdutoDisponivel}',
		                  'TesteAutoAVendaDescricao',
		                  '{referenciaSku}',
		                  1,
		                  1,
		                  1,
		                  1,
		                  '{eanSku}',
		                  GETDATE(),
		                  '20',
		                  '20',
		                  '50d9dfe9-7fcb-4d59-ba5d-058815d2b380',
		                  0
	                   )";
            try
            {
                ExecutarComandoSQL(queryCriacaoSkuDisponivel);
                CriarImagemSkuDisponivel(idSkuProdutoDisponivel, idProdutoDisponivel, nomeProduto, referenciaProduto, referenciaSku, eanSku);
            }
            catch (Exception ex)
            {
                GravarLogErro(ex.Message.ToString());
                ExcluirProdutoDisponivel(referenciaProduto, referenciaSku);
            }
        }

        public void CriarImagemSkuDisponivel(string idSkuProdutoDisponivel, string idProdutoDisponivel, string nomeProduto, string referenciaProduto, string referenciaSku, string eanSku)
        {
            var idSkuImagemProduto = Guid.NewGuid().ToString();
            var queryCriacaoSkuImagem = $@"
                INSERT INTO SkuImage
	                   (
		                  Id,
		                  IdSku,
		                  [Url],
		                  [Description],
		                  [Name],
		                  Position
	                   )
                VALUES 
	                   (
		                  '{idSkuImagemProduto}',
		                  '{idSkuProdutoDisponivel}',
		                  'https://360storagedev.blob.core.windows.net/360-sku-images/50d9dfe9-7fcb-4d59-ba5d-058815d2b380/da425f66-e19f-4017-9f65-416ddf40acb9/images/imagem_teste_automatizado.jpg',
		                  'DescricaoImagemSku',
		                  'camiseta teste automatizado',
		                  0
	                   )";
            try
            {
                ExecutarComandoSQL(queryCriacaoSkuImagem);
                CriarProdutoAVenda(idProdutoDisponivel, idSkuProdutoDisponivel, nomeProduto, referenciaProduto, referenciaSku, eanSku);
            }
            catch (Exception ex)
            {
                GravarLogErro(ex.Message.ToString());
                ExcluirProdutoDisponivel(referenciaProduto, referenciaSku, "DescricaoImagemSku");
            }
        }

        public void CriarProdutoAVenda(string idProdutoDisponivel, string idSkuProdutoDisponivel, string nomeProduto, string referenciaProduto, string referenciaSku, string eanSku)
        {
            var idProdutoAVenda = Guid.NewGuid().ToString();
            var queryCriacaoProdutoAVenda = $@"
                INSERT INTO ChannelProduct 
	                    (
		                   Id,
		                   IdProduct,
		                   IdCompanyChannelConnection,
		                   IdChannelCategory,
		                   ShortDescription,
		                   LongDescription,
		                   [Url],
		                   MetaTags,
		                   KeyWords,
		                   CreatedAt,
		                   IdChannelBrand,
		                   [Name],
		                   RefChannelProduct
	                    )
                VALUES
	                    (
	                       '{idProdutoAVenda}',
		                   '{idProdutoDisponivel}',
		                   '50d9dfe9-7fcb-4d59-ba5d-058815d2b310',
		                   '50d9dfe9-7fdf-4d59-ba5d-058815d2b394',
		                   'TesteAutomatizadoAVenda Descricao Curta',
		                   'TesteAutomatizadoAVenda Descricao Longa',
		                   'TesteAutomatizadoAVenda',
		                   'TesteAutomatizado',
		                   'TesteAutomatizado',
		                   GETDATE(),
		                   '5e4f21fc-65f6-4457-aca7-d179bb5a8999',
		                   '{nomeProduto}',
		                   '{referenciaProduto}'
                        )";
            try
            {
                ExecutarComandoSQL(queryCriacaoProdutoAVenda);
                CriarSkuAVenda(idProdutoAVenda, idSkuProdutoDisponivel, referenciaProduto, referenciaSku, eanSku);
            }
            catch (Exception ex)
            {
                GravarLogErro(ex.Message.ToString());
                ExcluirProdutoAVenda(referenciaProduto, referenciaSku);
                ExcluirProdutoDisponivel(referenciaProduto, referenciaSku, "DescricaoImagemSku");
            }
        }

        public void CriarSkuAVenda(string idProdutoAVenda, string idSkuProdutoDisponivel, string referenciaProduto, string referenciaSku, string eanSku)
        {
            var idSkuProdutoAVenda = Guid.NewGuid().ToString();
            var queryCriacaoSkuAVenda = $@"
                INSERT INTO ChannelSku
		                (
			                Id,
			                IdChannelProduct,
			                IdSku,
			                [Description],
			                CreatedAt,
			                [Weight],
			                Height,
			                Width,
			                [Length],
			                Ean,
			                PriceOf,
			                PriceBy,
			                RefChannelSku
		                )
                VALUES
		                (
			                '{idSkuProdutoAVenda}',
			                '{idProdutoAVenda}',
			                '{idSkuProdutoDisponivel}',
			                'TesteAutoAVendaDescricao',
			                GETDATE(),
			                1,
			                1,
			                1,
			                1,
			                '{eanSku}',
			                20,
			                20,
			                '{referenciaSku}'
		                )";
            try
            {
                ExecutarComandoSQL(queryCriacaoSkuAVenda);
                CriarStatusDoProdutoAVenda(idProdutoAVenda, referenciaProduto, referenciaSku);
                ConexaoBancoRedis conexaoBancoRedis = new ConexaoBancoRedis();
                conexaoBancoRedis.SetData("IdSkuAVenda", idSkuProdutoAVenda, 1800);
            }
            catch (Exception ex)
            {
                GravarLogErro(ex.Message.ToString());
                ExcluirProdutoAVenda(referenciaProduto, referenciaSku);
                ExcluirProdutoDisponivel(referenciaProduto, referenciaSku, "DescricaoImagemSku");
            }
        }

        public void CriarStatusDoProdutoAVenda(string idProdutoAVenda, string referenciaProduto, string referenciaSku)
        {
            var idStatusDoProdutoNoCanal = Guid.NewGuid().ToString();
            var queryStatusProdutoAVenda = $@"
                INSERT INTO ChannelProductState 
		                (
			                Id,
			                IdChannelProduct,
			                [Date],
			                [Status]
		                )
                VALUES
		                (
			                '{idStatusDoProdutoNoCanal}',
			                '{idProdutoAVenda}',
			                GETDATE(),
			                0
		                )";
            try
            {
                ExecutarComandoSQL(queryStatusProdutoAVenda);
                ConexaoBancoRedis conexaoBancoRedis = new ConexaoBancoRedis();
                conexaoBancoRedis.SetData("IdProdutoAVenda", idProdutoAVenda, 1800);
            }
            catch (Exception ex)
            {
                GravarLogErro(ex.Message.ToString());
                ExcluirProdutoAVenda(referenciaProduto, referenciaSku);
                ExcluirProdutoDisponivel(referenciaProduto, referenciaSku, "DescricaoImagemSku");
            }
        }
    }
}
