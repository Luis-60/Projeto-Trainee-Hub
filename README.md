# Projeto Trainee Hub

O projeto Trainee Hub, tem como principal objetivo auxiliar empresas de médio e grande porte armazenarem seus treinamentos organizacionais de modo que possuam uma interface direta com seu colaborador. A plataforma será desenvolvida com uma interface web que permitirá aos colaboradores acessarem seus treinamentos, sejam eles, treinamentos pendentes, agendados ou concluídos, para consulta, colaborando com a disseminação e retenção de informações essenciais, tais como uso de Equipamentos de Proteção Individual (EPIs) e informações a respeito de segurança do trabalho. 

---

## Índice

- [Pré-requisitos](#pré-requisitos)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Configuração do Backend](#configuração-do-backend)
- [Docker](#docker)
- [Execução e deploy](#execução-e-deploy)

---

## Pré-Requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/pt-br/download/dotnet/9.0)
- [Git](https://git-scm.com/)
- [Docker](https://www.docker.com/)

---

## Estrutura do Projeto

---

```bash
Projeto-Trainee-Hub/
├───.vscode/
├───Controllers/
├───Helper/
├───Models/
│   └───MasterContext.cs
├───Properties/
├───Repository/
├───ViewModel/
├───Views/
├───wwwroot/
├───bin/
├───obj/
├───.gitignore
├───Program.cs
├───Projeto-Trainee-Hub.csproj
├───Projeto-Trainee-Hub.sln
├───README.md
├───appsettings.Development.json
├───appsettings.json
├───docker-compose.yml
└───site64574.db
```
---

## Configuração do Backend

---

1.**Restaure as dependências e compile o projeto**

  ```bash
  dotnet restore
  dotnet build
  ```

---

## Docker

O Dockercompose na pasta raiz cria o banco de dados:

```dockercompose

services:
  mssql:
    image: mcr.microsoft.com/mssql/server:2022-latest
    container_name: mssql_server
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Password123
    ports:
      - "1433:1433"  
    networks:
      - sqlnet
    volumes:
      - mssql_data:/var/opt/mssql  

networks:
  sqlnet:
    driver: bridge

volumes:
  mssql_data:
    driver: local
```

Para rodar os containers:

```bash
docker compose up --build -d
```
Para ligar a imagem:

```bash
docker start mssql_server
```
Ao criar a imagem, a imagem irá se estabelecer em um determinado IP, que deve ser inspecionado com o comando:

```bash
docker inspect mssql_server | grep "IPAddress"
```

Substitua o IP recebido pelo input dentro da DefaultConnection no arquivo appsettings.json que se encontra no diretório raiz, e dentro do MasterContext.cs, que se encontra no diretório Models.

---

## Execução e Deploy

- **Backend**
Para executar o projeto:

```bash
dotnet watch run
```

O projeto estará disponível na porta configurada: `http://localhost:5104`.
