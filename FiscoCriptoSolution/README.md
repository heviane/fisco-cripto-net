# Biblioteca Fisco Cripto (`.dll`)

...

## Como Gerar a DLL

Execute os comandos dentro do diretório da solução:

```bash
dotnet build
```

Localize a DLL: Após a compilação, a DLL estará localizada em um caminho similar a: `fisco-cripto-net/bin/Debug/net9.0/fisco-cripto-net.dll`

## Como a Aplicação Cliente Usará a DLL

Um sistema cliente em C# agora pode referenciar esta DLL e utilizar o serviço da seguinte forma:

```csharp
// Exemplo de uso em uma aplicação cliente
// using FiscoCriptoNet; 

string xmlOriginal = "<NFe><infNFe Id=\"NFe4321...\">...</infNFe>...</NFe>";
string caminhoCertificado = "C:\\Certificados\\MeuCertificado.pfx";
string senha = "minhasenha";
string tagParaAssinar = "infNFe"; // Nó pai que contém o atributo 'Id'

try
{
    var servico = new FiscoCriptoService();
    string xmlAssinado = servico.AssinarDocumentoFiscalA1(
        xmlOriginal,
        caminhoCertificado,
        senha,
        tagParaAssinar
    );

    // xmlAssinado agora contém o bloco <Signature>
    Console.WriteLine("XML Assinado com sucesso!");
}
catch (Exception ex)
{
    Console.WriteLine($"Falha na assinatura: {ex.Message}");
}
```
