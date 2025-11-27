using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace fisco_cripto_net
{
    /// <summary>
    /// Gerencia o carregamento e acesso aos certificados digitais (A1 e A3).
    /// </summary>
    public class CertificadoManager
    {
        private X509Certificate2 _certificado;

        /// <summary>
        /// Carrega o certificado digital a partir de um arquivo PFX/P12 (Certificado A1).
        /// </summary>
        /// <param name="caminhoArquivo">Caminho completo do arquivo .pfx ou .p12.</param>
        /// <param name="senha">Senha de proteção do certificado.</param>
        public void CarregarCertificadoA1(string caminhoArquivo, string senha)
        {
            // O sinalizador Exportable permite que a chave seja usada para assinatura
            // A flag PersistKeySet é importante para persistir a chave privada
            _certificado = new X509Certificate2(
                caminhoArquivo,
                senha,
                X509KeyStorageFlags.Exportable | X509KeyStorageFlags.PersistKeySet
            );

            // Validação de segurança: verifica se a chave privada está acessível
            if (!_certificado.HasPrivateKey)
            {
                throw new CryptographicException("O certificado carregado não possui chave privada acessível.");
            }
        }

        /// <summary>
        /// Carrega o certificado digital a partir do Windows Certificate Store (Geralmente usado para A3 ou A1 instalado).
        /// </summary>
        /// <param name="subjectName">O nome do titular do certificado (Ex: "CN=NOME DA EMPRESA LTDA:12345678901234").</param>
        public void CarregarCertificadoStore(string subjectName)
        {
            // Abrir o local de armazenamento dos certificados no Windows
            using (var store = new X509Store(StoreName.My, StoreLocation.CurrentUser))
            {
                store.Open(OpenFlags.ReadOnly);

                // Encontrar o certificado pelo nome do titular
                X509Certificate2Collection certificadosEncontrados = store.Certificates.Find(
                    X509FindType.FindBySubjectDistinguishedName, 
                    subjectName, 
                    false // Não ignorar certificados expirados ou inválidos aqui
                );

                if (certificadosEncontrados.Count == 0)
                {
                    throw new CryptographicException($"Certificado com o nome '{subjectName}' não encontrado no Store.");
                }

                // Consideramos que o primeiro certificado válido é o correto
                _certificado = certificadosEncontrados[0];
            }

            if (!_certificado.HasPrivateKey)
            {
                 throw new CryptographicException("O certificado encontrado não possui chave privada acessível.");
            }
        }
        
        /// <summary>
        /// Retorna o certificado carregado (Propriedade somente leitura).
        /// </summary>
        public X509Certificate2 Certificado => _certificado;
    }
}
