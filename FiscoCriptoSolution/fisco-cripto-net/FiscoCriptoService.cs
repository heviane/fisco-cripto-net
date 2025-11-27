using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace fisco_cripto_net
{
    /// <summary>
    /// Serviço de alto nível para orquestrar o carregamento do certificado e a assinatura de documentos fiscais.
    /// Esta é a principal classe a ser utilizada pelas aplicações clientes.
    /// </summary>
    public class FiscoCriptoService
    {
        private readonly CertificadoManager _manager;
        private readonly AssinadorXml _assinador;

        public FiscoCriptoService()
        {
            _manager = new CertificadoManager();
            _assinador = new AssinadorXml();
        }

        /// <summary>
        /// Carrega um certificado A1 e assina o conteúdo XML do documento fiscal.
        /// </summary>
        /// <param name="xmlConteudo">O XML do documento fiscal (NFe, NFSe, etc.) não assinado.</param>
        /// <param name="caminhoCertificadoA1">Caminho completo do arquivo .pfx ou .p12.</param>
        /// <param name="senhaCertificado">Senha de proteção do certificado.</param>
        /// <param name="tagIdASerAssinada">O nome do atributo 'Id' do nó a ser assinado (Ex: 'infNFe', 'LoteRps').</param>
        /// <returns>O XML assinado como string.</returns>
        public string AssinarDocumentoFiscalA1(
            string xmlConteudo, 
            string caminhoCertificadoA1, 
            string senhaCertificado, 
            string tagIdASerAssinada)
        {
            try
            {
                // 1. Carregar o Certificado
                _manager.CarregarCertificadoA1(caminhoCertificadoA1, senhaCertificado);
                
                // 2. Assinar o XML
                X509Certificate2 certificado = _manager.Certificado;
                string xmlAssinado = _assinador.Assinar(xmlConteudo, certificado, tagIdASerAssinada);

                return xmlAssinado;
            }
            catch (CryptographicException ex)
            {
                // Erros específicos de criptografia (senha errada, chave privada inacessível, etc.)
                throw new Exception($"Erro de Criptografia durante o processamento do certificado: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                // Erros específicos da assinatura (tag ID não encontrada, formato incorreto)
                 throw new Exception($"Erro na estrutura do XML ou na assinatura: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Outros erros genéricos
                throw new Exception($"Erro geral no serviço de criptografia: {ex.Message}");
            }
        }
    }
}