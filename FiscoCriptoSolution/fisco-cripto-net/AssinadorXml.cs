
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Xml;
using System.Security.Cryptography;

namespace fisco_cripto_net
{
    /// <summary>
    /// Classe responsável por aplicar a Assinatura XML Digital (XMLDSig) no documento fiscal.
    /// </summary>
    public class AssinadorXml
    {
        /// <summary>
        /// Assina o documento XML de forma canônica, buscando o nó a ser assinado pelo seu ID.
        /// </summary>
        /// <param name="xmlConteudo">O documento XML (NFe, NFSe, CTe, etc.) como string.</param>
        /// <param name="certificado">O objeto X509Certificate2 contendo a chave privada.</param>
        /// <param name="tagIdASerAssinada">O nome do atributo 'ID' do nó que deve ser assinado (Ex: 'infNFe', 'LoteRps').</param>
        /// <returns>A string do XML assinado.</returns>
        public string Assinar(string xmlConteudo, X509Certificate2 certificado, string tagIdASerAssinada)
        {
            if (certificado == null || !certificado.HasPrivateKey)
            {
                throw new ArgumentException("O certificado é inválido ou não possui chave privada acessível.");
            }

            // 1. Carregar o XML na estrutura DOM (Document Object Model)
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.PreserveWhitespace = true; // Preservar espaços é crucial para a canonicidade
            xmlDoc.LoadXml(xmlConteudo);

            // 2. Encontrar o nó (tag) raiz a ser assinado e obter o ID
            // Exemplo para NFe: O nó é 'infNFe' e o ID é o valor do atributo 'Id'.
            XmlNodeList tagLista = xmlDoc.GetElementsByTagName(tagIdASerAssinada);

            if (tagLista.Count == 0)
            {
                throw new InvalidOperationException($"Nó com a tag '{tagIdASerAssinada}' não encontrado no XML.");
            }

            XmlElement elementoASerAssinado = (XmlElement)tagLista[0];
            string idAssinatura = elementoASerAssinado.GetAttribute("Id");
            
            if (string.IsNullOrEmpty(idAssinatura))
            {
                throw new InvalidOperationException($"O nó '{tagIdASerAssinada}' deve ter um atributo 'Id' definido.");
            }

            // 3. Criar o objeto SignedXml para assinar
            SignedXml signedXml = new SignedXml(xmlDoc);

            // Setar a chave de assinatura (chave privada do certificado)
            signedXml.SigningKey = certificado.PrivateKey;
            
            // Setar o Certificate Serial Number (Exigência do fisco)
            KeyInfo keyInfo = new KeyInfo();
            keyInfo.AddClause(new KeyInfoX509Data(certificado));
            signedXml.KeyInfo = keyInfo;

            // 4. Configurar a Referência (URI e Algoritmos)
            Reference reference = new Reference();
            reference.Uri = "#" + idAssinatura; // O URI aponta para o ID do nó que será assinado.

            // Algoritmo de Digest (Hash) (SHA-256 é o padrão atual, mas verifique o manual do fisco)
            reference.DigestMethod = "http://www.w3.org/2001/04/xmlenc#sha256";

            // Transformação Canônica (Elimina espaços em branco, etc.)
            reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
            reference.AddTransform(new XmlDsigC14NTransform()); // Canonicalização C14N

            signedXml.AddReference(reference);

            // 5. Algoritmo de Assinatura (RSA SHA-256)
            signedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256";

            // 6. Computar a Assinatura
            signedXml.ComputeSignature();

            // 7. Obter o elemento <Signature> e importá-lo no XML original
            XmlElement signatureElement = signedXml.GetXml();
            
            // Adicionar a assinatura como filho do nó raiz
            xmlDoc.DocumentElement.AppendChild(signatureElement);

            // Retornar o XML completo e assinado
            return xmlDoc.OuterXml;
        }
    }
}