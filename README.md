# API de Suporte Técnico

Esta é uma API RESTful desenvolvida em ASP.NET Core para um sistema de cadastro e gerenciamento de chamados de suporte técnico.

## Funcionalidades

- **Autenticação de Usuários**: Sistema de registro e login com geração de token JWT.
- **Gerenciamento de Chamados**:
  - Criação de novos chamados de suporte.
  - Listagem e visualização de chamados existentes.
  - Avaliação da resolução de um chamado.
- **Integração com IA (Google Gemini)**: Utiliza o modelo Gemini para analisar o conteúdo dos chamados e fornecer insights ou sugestões.

## Tecnologias Utilizadas

- [ASP.NET Core 8](https://dotnet.microsoft.com/apps/aspnet)
- [Entity Framework Core 8](https://docs.microsoft.com/ef/core/)
- [SQL Server](https://www.microsoft.com/sql-server) via Docker
- [Google Gemini API](https://aistudio.google.com/)
- [JWT (JSON Web Tokens)](https://jwt.io/) para autenticação
- [Docker](https://www.docker.com/)

## Frontend

O front-end para esta API foi desenvolvido em React e está disponível em um repositório separado. Para clonar e executar o projeto front-end, siga as instruções no repositório correspondente.

- **Repositório:** [https://github.com/La-silva1/SuporteTecnicoIA-front-end](https://github.com/La-silva1/SuporteTecnicoIA-front-end)

## Como Executar o Projeto com Docker

Siga os passos abaixo para configurar e executar a aplicação localmente usando Docker.

### 1. Pré-requisitos

- [Docker](https://www.docker.com/) e [Docker Compose](https://docs.docker.com/compose/) instalados.
- [Git](https://git-scm.com/) para clonar o repositório.
- Uma chave de API do [Google Gemini](https://aistudio.google.com/).

### 2. Configuração Inicial

1.  **Clone o repositório:**
    ```bash
    git clone https://github.com/La-silva1/SuporteTecnicoIA.git
    cd SuporteTecnicoIA/ApiCadastro
    ```

2.  **Configure sua API Key:**
    Abra o arquivo `appsettings.Development.json` e adicione sua chave de API do Google Gemini.
    ```json
    {
      "GeminiApiKey": "SUA_API_KEY_DO_GEMINI_AQUI"
    }
    ```

### 3. Executando a Aplicação

1.  **Suba os contêineres:**
    Este comando irá construir a imagem da API e iniciar os contêineres da aplicação e do banco de dados em background.
    ```bash
    docker-compose up --build -d
    ```

2.  **Aplique as Migrations:**
    Para criar a estrutura do banco de dados, execute o comando de `update` do Entity Framework dentro do contêiner da aplicação.
    ```bash
    docker-compose exec app dotnet ef database update
    ```

3.  **Acesse a API:**
    A aplicação estará disponível no seu navegador.
    - **Swagger UI**: [http://localhost:8080/swagger/index.html](http://localhost:8080/swagger/index.html)

## Comandos Úteis do Docker

-   **Atualizar e Reconstruir (Pull & Build):**
    Para garantir que você tem a versão mais recente do código e reconstruir a imagem Docker.
    ```bash
    git pull
    docker-compose up -d --build
    ```

-   **Parar os Contêineres:**
    Para a execução da aplicação e do banco de dados.
    ```bash
    docker-compose down
    ```

-   **Visualizar Logs:**
    Para acompanhar os logs da aplicação em tempo real.
    ```bash
    docker-compose logs -f app
    ```

-   **Acessar o Shell do Contêiner:**
    Para executar comandos diretamente no contêiner da aplicação.
    ```bash
    docker-compose exec app bash
    ```

## Testes

O projeto possui uma suíte de testes para garantir a qualidade e o funcionamento correto da API. Para executar os testes, utilize o seguinte comando na raiz do projeto:

```bash
dotnet test
```

Os testes cobrem:
-   **Testes de Unidade**: Verificam a lógica de componentes isolados.
-   **Testes de Integração**: Garantem que os componentes funcionem em conjunto.
-   **Testes de Segurança**: Asseguram que endpoints protegidos exigem autenticação.
-   **Testes de Validação**: Validam as regras nos modelos de entrada (DTOs).

## Endpoints da API

A seguir estão os principais endpoints disponíveis. A maioria requer um token de autenticação no header `Authorization`.

### Autenticação

-   `POST /api/login`: Autentica um usuário e retorna um token JWT.
-   `POST /api/register`: Registra um novo usuário.

### Chamados (Tickets)

-   `POST /api/ticket`: Cria um novo chamado (requer autenticação).
-   `GET /api/ticket`: Lista todos os chamados do usuário autenticado.
-   `GET /api/ticket/{id}`: Obtém os detalhes de um chamado específico.
-   `PUT /api/ticket/{id}/avaliar`: Adiciona uma avaliação a um chamado (nota de 1 a 5).
