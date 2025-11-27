# Notes

...

## `CertificadoManager.cs`

Classe principal para o carregamento do certificado digital (Passo fundamental da criptografia).

Este é o passo mais crucial. O módulo de criptografia precisa ser capaz de carregar e gerenciar a chave privada do certificado digital, seja ele A1 (arquivo .pfx ou .p12) ou A3 (Smartcard/Token).

No .NET (C#), a classe principal para lidar com certificados é a X509Certificate2, encontrada no namespace System.Security.Cryptography.X509Certificates.

## `AssinadorXml.cs`

módulo de Assinatura XML Digital (XMLDSig/XAdES), que é a aplicação da chave privada carregada no XML do documento fiscal.

Com o certificado carregado, o próximopasso é criar a lógica para aplicar a assinatura digital ao documento fiscal (XML).
Para documentos fiscais brasileiros, o padrão é o XML Digital Signature (XMLDSig), geralmente implementado como XAdES (XML Advanced Electronic Signatures).

No C#/.NET, o namespace System.Security.Cryptography.Xml fornece as classes principais para essa tarefa, sendo a SignedXml a mais importante.

O código desta classe encapsula a complexidade de encontrar o nó a ser assinado (URI="#ID_...), aplicar a chave privada e injetar o bloco <Signature> no XML.

## `FiscoCriptoService.cs`

classe de serviço de alto nível que orquestra o carregamento do certificado e a assinatura do XML. Isso tornará a utilização da sua DLL fisco-cripto-net.dll muito mais simples para os sistemas clientes.

classe que centraliza a chamada aos métodos criados, oferecendo uma interface de uso limpa e única.

método principal AssinarDocumentoFiscalA1 (usando PFX/P12 como exemplo) e gerencia as exceções.
