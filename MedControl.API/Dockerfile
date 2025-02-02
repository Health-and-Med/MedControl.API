# Etapa 1: Build da aplicação
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 🔹 Copia todos os arquivos da solução (.sln) e de projetos (.csproj)
COPY *.sln ./
COPY MedControl.API/*.csproj MedControl.API/
COPY MedControl.Application/*.csproj MedControl.Application/
COPY MedControl.Domain/*.csproj MedControl.Domain/
COPY MedControl.Infrastructure/*.csproj MedControl.Infrastructure/
COPY MedControl.Tests/*.csproj MedControl.Tests/

# 🔹 Restaura as dependências
RUN dotnet restore MedControl.API/MedControl.API.csproj

# 🔹 Copia o restante do código
COPY . ./

# 🔹 Compila a aplicação
RUN dotnet publish MedControl.API/MedControl.API.csproj -c Release -o /publish

# Etapa 2: Runtime otimizado para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# 🔹 Copia os arquivos compilados
COPY --from=build /publish .

# 🔹 Define o ambiente
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# 🔹 Expor a porta da API
EXPOSE 80

# 🔹 Comando para iniciar a API
ENTRYPOINT ["dotnet", "MedControl.API.dll"]
