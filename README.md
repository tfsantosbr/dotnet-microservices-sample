# .NET Microservices Sample

| Resource      | URL                               |
| ------------- | --------------------------------- |
| SQL Server    | http://host.docker.internal:1433  |
| Mongo         | http://host.docker.internal:27017 |
| Postgres      | http://host.docker.internal:5432  |
| Kafka         | http://host.docker.internal:9092  |
| kafkadrop     | http://host.docker.internal:9000  |


Kibana -> http://localhost:5601

curl -L -O https://raw.githubusercontent.com/elastic/apm-server/8.13/apm-server.docker.yml

docker run -d -p 8200:8200 --name=apm-server --user=apm-server --volume="${PWD}/apm-server.docker.yml:/usr/share/apm-server/apm-server.yml:ro" docker.elastic.co/apm/apm-server:8.13.2 --strict.perms=false -e -E output.elasticsearch.hosts=["elasticsearch:9200"]