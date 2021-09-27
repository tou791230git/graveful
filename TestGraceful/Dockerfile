FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["TestGraceful/TestGraceful.csproj", "TestGraceful/"]
RUN dotnet restore "TestGraceful/TestGraceful.csproj"
COPY . .
WORKDIR "/src/TestGraceful"
RUN dotnet build "TestGraceful.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestGraceful.csproj" -c Release -o /app/publish
RUN apt-get update
RUN apt-get install -y procps
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestGraceful.dll"]

#ENTRYPOINT ["tini", "--", "dotnet", "TestGraceful.dll"]



## Add Tini
#ENV TINI_VERSION v0.19.0
#ADD https://github.com/krallin/tini/releases/download/${TINI_VERSION}/tini /tini
#RUN chmod +x /tini
#ENTRYPOINT ["/tini", "--"]
## Run your program under Tini
#CMD ["dotnet", "TestGraceful.dll"]
## or docker run your-image /your/program ...
