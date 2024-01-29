FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /App

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /App

COPY --from=build /App/out ./
ENTRYPOINT ["dotnet", "HandsOn.AspNetCore.Kubernetes.dll"]