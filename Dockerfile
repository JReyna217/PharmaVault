# 1. Compilation phase (uses the full .NET 10 SDK)
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy the project files and restore dependencies (using the Docker cache)
COPY ["src/PharmaVault.Core/PharmaVault.Core.csproj", "src/PharmaVault.Core/"]
COPY ["src/PharmaVault.Data/PharmaVault.Data.csproj", "src/PharmaVault.Data/"]
COPY ["src/PharmaVault.Web/PharmaVault.Web.csproj", "src/PharmaVault.Web/"]
RUN dotnet restore "src/PharmaVault.Web/PharmaVault.Web.csproj"

# Copy the rest of the source code and compile it
COPY . .
WORKDIR "/src/src/PharmaVault.Web"
RUN dotnet build "PharmaVault.Web.csproj" -c Release -o /app/build

# 2. Deployment Phase (Optimize the binaries)
FROM build AS publish
RUN dotnet publish "PharmaVault.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

#3. Production Stage (Use a very lightweight image with just the runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# .NET 8/10 in Docker uses port 8080 by default internally
EXPOSE 8080 

# Boot command
ENTRYPOINT ["dotnet", "PharmaVault.Web.dll"]