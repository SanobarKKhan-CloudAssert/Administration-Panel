FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
ARG NUGET_SOURCE
ENV NUGET_SOURCE $NUGET_SOURCE
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG PIPELINE_USER
ENV PIPELINE_USER $PIPELINE_USER
ARG PIPELINE_TOKEN
ENV PIPELINE_TOKEN $PIPELINE_TOKEN
ARG NUGET_SOURCE
ENV NUGET_SOURCE $NUGET_SOURCE
WORKDIR /src
COPY ["src/MateMachine.AdminPanel.Api/MateMachine.AdminPanel.Api.csproj", "src/MateMachine.AdminPanel.Api/"]
COPY ["src/MateMachine.AdminPanel.Application/MateMachine.AdminPanel.Application.csproj", "src/MateMachine.AdminPanel.Application/"]
COPY ["src/MateMachine.AdminPanel.Domain/MateMachine.AdminPanel.Domain.csproj", "src/MateMachine.AdminPanel.Domain/"]
COPY ["src/MateMachine.AdminPanel.Infrastructure/MateMachine.AdminPanel.Infrastructure.csproj", "src/MateMachine.AdminPanel.Infrastructure/"]
RUN dotnet nuget locals all --clear
RUN dotnet dev-certs https --trust
RUN dotnet nuget add source --username "${PIPELINE_USER}" --password "${PIPELINE_TOKEN}" --store-password-in-clear-text --name github "${NUGET_SOURCE}"
RUN dotnet restore "src/MateMachine.AdminPanel.Api/MateMachine.AdminPanel.Api.csproj"
COPY . .
WORKDIR "/src/src/MateMachine.AdminPanel.Api"
RUN dotnet build "MateMachine.AdminPanel.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MateMachine.AdminPanel.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MateMachine.AdminPanel.Api.dll"]
