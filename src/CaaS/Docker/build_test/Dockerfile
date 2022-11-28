FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-stage

WORKDIR /opt/CaaS
COPY ../../ .
RUN dotnet tool restore \
  && dotnet build \
  && mkdir /data \
  && dotnet publish -o dist

FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /opt/CaaS

COPY --from=build-stage /opt/NotesAPI/dist .
COPY --from=build-stage /data /data

ENV ASPNETCORE_URLS "http://0.0.0.0:5000"

CMD dotnet test

# docker build -t exercise2-api:latest .
# docker run --rm -it exercise2-api:latest
# docker run -rm -d -p  5000:5000 exercise2-api:latest
# docker run -rm -d -p  5000:5000 -v myvol:/data  exercise2-api:latest
