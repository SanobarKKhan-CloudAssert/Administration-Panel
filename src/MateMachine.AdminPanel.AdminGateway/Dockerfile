FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG Env="Development"
ARG NUGET_SOURCE
ENV NUGET_SOURCE $NUGET_SOURCE

ARG PIPELINE_USER
ENV PIPELINE_USER $PIPELINE_USER

ARG PIPELINE_TOKEN
ENV PIPELINE_TOKEN $PIPELINE_TOKEN
WORKDIR /src
COPY ["./src/MateMachine.AdminPanel.AdminGateway/MateMachine.AdminPanel.AdminGateway.csproj", "MateMachine.AdminPanel.AdminGateway/"]
RUN dotnet nuget locals all --clear
RUN dotnet dev-certs https --trust
RUN dotnet nuget add source --username ${PIPELINE_USER} --password ${PIPELINE_TOKEN} --store-password-in-clear-text --name github "${NUGET_SOURCE}"
RUN dotnet restore "./MateMachine.AdminPanel.AdminGateway/MateMachine.AdminPanel.AdminGateway.csproj"
COPY ./src/ ./
COPY "./src/MateMachine.AdminPanel.AdminGateway/Ocelot/${Env}/Ocelot.global.json" "MateMachine.AdminPanel.AdminGateway/Ocelot/Ocelot.global.json"
COPY "./src/MateMachine.AdminPanel.AdminGateway/Ocelot/${Env}" "MateMachine.AdminPanel.AdminGateway/Ocelot"
WORKDIR "/src/MateMachine.AdminPanel.AdminGateway"
RUN dotnet build "MateMachine.AdminPanel.AdminGateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MateMachine.AdminPanel.AdminGateway.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MateMachine.AdminPanel.AdminGateway.dll"]
