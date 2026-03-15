# 1. Etapa de Construcción (Usa el SDK completo de .NET 10)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copiamos los archivos de proyecto y restauramos dependencias (aprovecha la caché de Docker)
COPY ["src/PharmaVault.Core/PharmaVault.Core.csproj", "src/PharmaVault.Core/"]
COPY ["src/PharmaVault.Data/PharmaVault.Data.csproj", "src/PharmaVault.Data/"]
COPY ["src/PharmaVault.Web/PharmaVault.Web.csproj", "src/PharmaVault.Web/"]
RUN dotnet restore "src/PharmaVault.Web/PharmaVault.Web.csproj"

# Copiamos el resto del código fuente y compilamos
COPY . .
WORKDIR "/src/src/PharmaVault.Web"
RUN dotnet build "PharmaVault.Web.csproj" -c Release -o /app/build

# 2. Etapa de Publicación (Optimiza los binarios)
FROM build AS publish
RUN dotnet publish "PharmaVault.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# 3. Etapa de Producción (Usa una imagen súper ligera solo con el Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# .NET 8/10 en Docker usa el puerto 8080 por defecto internamente
EXPOSE 8080 

# Comando de arranque
ENTRYPOINT ["dotnet", "PharmaVault.Web.dll"]