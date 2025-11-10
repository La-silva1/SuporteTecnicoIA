# API de Suporte Técnico

Esta é uma API RESTful desenvolvida em ASP.NET Core para um sistema de cadastro e gerenciamento de chamados de suporte técnico.

## Funcionalidades

- *Autenticação de Usuários*: Sistema de registro e login com geração de token JWT.
- *Gerenciamento de Chamados*:
  - Criação de novos chamados de suporte.
  - Listagem e visualização de chamados existentes.
  - Avaliação da resolução de um chamado.
- *Integração com IA (Google Gemini)*: Utiliza o modelo Gemini para analisar o conteúdo dos chamados e fornecer insights ou sugestões.

## Tecnologias Utilizadas

- [ASP.NET Core 8](https://dotnet.microsoft.com/apps/aspnet)
- [Entity Framework Core 8](https://docs.microsoft.com/ef/core/)
- [SQL Server on Docker](https://hub.docker.com/_/microsoft-mssql-server)
- [Google Gemini API](https://aistudio.google.com/)
- [JWT (JSON Web Tokens)](https://jwt.io/) para autenticação

## Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) ou superior.
- [Docker](https://www.docker.com/) com uma instância do SQL Server em execução.
- API Key do Google Gemini.

## Como Executar o Projeto

### Executando com Docker Compose

1.  *Clone o repositório:*
    bash
    git clone git@github.com:La-silva1/SuporteTecnicoIA.git
    cd ApiCadastro
    

2.  *Configure as Chaves e Conexões:*
    - Adicione sua API Key do Gemini no arquivo appsettings.Development.json.
    json
    {
      "GeminiApiKey": "SUA_API_KEY_DO_GEMINI_AQUI"
    }
    

3.  *Inicie os contêineres:*
    bash
    docker-compose up --build -d
    

4.  *Aplique as Migrations:*
    Acesse o contêiner da aplicação e execute os comandos para criar as tabelas no banco de dados.
    bash
    docker-compose exec app bash
    cd /app/src
    dotnet tool restore
    dotnet ef database update
    exit
    

5.  *Acesse a aplicação:*
    A API estará disponível com a interface do Swagger em: http://localhost:8080/swagger/index.html

### Executando Localmente

1.  *Clone o repositório:*
    bash
    git clone git@github.com:La-silva1/SuporteTecnicoIA.git
    cd ApiCadastro
    

2.  *Configure as Chaves e Conexões:*
    - Adicione sua string de conexão do SQL Server e sua API Key do Gemini em um desses arquivos.

    A estrutura a ser adicionada (ou completada) no seu appsettings.json ou appsettings.Development.json é a seguinte. Ajuste a senha e outros parâmetros para o seu ambiente Docker.
    json
    {
      "Logging": {
        "LogLevel": {
          "Default": "Information",
          "Microsoft.AspNetCore": "Warning"
        }
      },
      "ConnectionStrings": {
        "DefaultConnection": "Server=localhost,1433;Database=ApiCadastroDB;User Id=sa;Password=Your_password123;TrustServerCertificate=True"
      },
      "GeminiApiKey": "SUA_API_KEY_DO_GEMINI_AQUI"
    }
    

3.  *Instale as dependências e ferramentas:*
    docker exec -it suportetecnicoia-app-1 bash
    dotnet restore
    dotnet tool restore

    

4.  *Aplique as Migrations:*
    Execute o comando abaixo para criar as tabelas no banco de dados.
    bash
    dotnet ef database update
    

5.  *Execute a aplicação:*
    bash
    dotnet run
    
    A API estará disponível com a interface do Swagger em: http://localhost:5268/swagger/index.html

## Testes

O projeto possui uma suíte de testes para garantir a qualidade e o funcionamento correto da API. Os testes cobrem:

-   *Testes de Unidade*: Verificam a lógica de componentes isolados, como os controllers.
-   *Testes de Integração*: Garantem que os diferentes componentes da API funcionem corretamente em conjunto, incluindo a interação com o banco de dados em memória.
-   *Testes de Segurança*: Verificam se os endpoints protegidos requerem autenticação e se o acesso é negado sem um token válido.
-   *Testes de Validação*: Asseguram que as regras de validação nos modelos de entrada (DTOs) funcionam como esperado, rejeitando dados inválidos.

Para executar os testes, utilize o seguinte comando na raiz do projeto:

bash
dotnet test


## Endpoints da API

A seguir estão os principais endpoints disponíveis na API. A maioria requer um token de autenticação no header Authorization.

### Autenticação

-   *POST /api/login*: Autentica um usuário.
    -   *Body*: { "email": "user@example.com", "password": "your_password" }
    -   *Retorno*: { "token": "jwt_token" }

-   *POST /api/register*: Registra um novo usuário.
    -   *Body*: { "username": "newuser", "email": "new@example.com", "password": "your_password" }

### Chamados (Tickets)

-   *POST /api/ticket* (Requer autenticação)
    -   *Descrição*: Cria um novo chamado.
    -   *Body*: { "titulo": "string", "descricao": "string" }

-   *GET /api/ticket* (Requer autenticação)
    -   *Descrição*: Lista todos os chamados do usuário autenticado.

-   *GET /api/ticket/{id}* (Requer autenticação)
    -   *Descrição*: Obtém os detalhes de um chamado específico.

-   *PUT /api/ticket/{id}/avaliar* (Requer autenticação)
    -   *Descrição*: Adiciona uma avaliação a um chamado.
    -   *Body*: { "nota": int, "comentario": "string" } (Nota de 1 a 5)