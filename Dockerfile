FROM mcr.microsoft.com/dotnet/aspnet:6.0  AS runtime

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

COPY ./**/*.csproj ./
COPY ./**/**/*.csproj ./
RUN for file in $(ls *.csproj); do echo ${file} && dotnet restore ${file}; done
RUN rm *.csproj

COPY . .
WORKDIR "/src/MasterFab.Web"
RUN dotnet publish "MasterFab.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "MasterFab.Web.dll"]