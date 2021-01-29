docker container stop core-api
docker container rm core-api
docker build -t core-api:latest .
docker run -d --name core-api --env ASPNETCORE_ENVIRONMENT=Development -p 8080:80 core-api:latest
