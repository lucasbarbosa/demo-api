# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files (csproj) and restore dependencies
COPY ["Directory.Packages.props", "."]
COPY ["src/DemoApi.Api/DemoApi.Api.csproj", "src/DemoApi.Api/"]
COPY ["src/DemoApi.Application/DemoApi.Application.csproj", "src/DemoApi.Application/"]
COPY ["src/DemoApi.Domain/DemoApi.Domain.csproj", "src/DemoApi.Domain/"]
COPY ["src/DemoApi.Infra.CrossCutting/DemoApi.Infra.CrossCutting.csproj", "src/DemoApi.Infra.CrossCutting/"]
COPY ["src/DemoApi.Infra/DemoApi.Infra.Data.csproj", "src/DemoApi.Infra/"]

RUN dotnet restore "src/DemoApi.Api/DemoApi.Api.csproj"

# Copy all source code and build
COPY . .
WORKDIR "/src/src/DemoApi.Api"
RUN dotnet build "DemoApi.Api.csproj" -c Release -o /app/build

# Publish Stage
FROM build AS publish
RUN dotnet publish "DemoApi.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final Stage (Runtime)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Configure non-root user for security (default in .NET 10)
USER app

COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DemoApi.Api.dll"]