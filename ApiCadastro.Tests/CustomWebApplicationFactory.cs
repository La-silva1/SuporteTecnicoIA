using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using System.IO;
using System.Reflection;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Define o diretório raiz do projeto principal para localizar arquivos como appsettings.json
        builder.UseContentRoot(FindProjectRootPath("ApiCadastro"));

        builder.ConfigureServices(services =>
        {
            // Caso precise substituir serviços (ex.: usar DbContext em memória), faça aqui.
        });
    }

    /// <summary>
    /// Localiza o diretório do projeto principal (ex.: ApiCadastro)
    /// a partir da estrutura de diretórios da solução.
    /// </summary>
    private static string FindProjectRootPath(string targetProjectName)
    {
        var testAssemblyPath = Assembly.GetExecutingAssembly().Location;
        var directoryInfo = new DirectoryInfo(Path.GetDirectoryName(testAssemblyPath)!);

        // Sobe os diretórios até encontrar a raiz da solução (SuporteTecnico)
        while (directoryInfo != null && directoryInfo.Name != "SuporteTecnico")
        {
            directoryInfo = directoryInfo.Parent;
        }

        if (directoryInfo == null)
        {
            throw new DirectoryNotFoundException($"Não foi possível localizar a raiz da solução 'SuporteTecnico'. Caminho atual: {testAssemblyPath}");
        }

        var appProjectDir = Path.Combine(directoryInfo.FullName, targetProjectName);

        if (Directory.Exists(appProjectDir))
        {
            return appProjectDir;
        }

        throw new DirectoryNotFoundException($"Não foi possível localizar o diretório do projeto: {appProjectDir}");
    }
}
