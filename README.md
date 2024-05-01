# .NET Microservices Sample

| Resource      | URL                               |
| ------------- | --------------------------------- |
| SQL Server    | http://host.docker.internal:1433  |
| Mongo         | http://host.docker.internal:27017 |
| Postgres      | http://host.docker.internal:5432  |
| Kafka         | http://host.docker.internal:9092  |
| kafkadrop     | http://host.docker.internal:9000  |

```bash
# Endpoints
Grafana -> http://localhost:3000
Prometheus -> http://localhost:9090/targets
Loki -> http://localhost:3100/services | http://localhost:3100/metrics
Tempo -> http://localhost:3200/metrics
```

## Documentation Reference

[Trace logs with Grafana Tempo and Loki](https://grafana.com/docs/grafana/next/datasources/tempo/configure-tempo-data-source/#trace-to-logs)
