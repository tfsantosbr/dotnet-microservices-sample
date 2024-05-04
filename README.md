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

## Run Docker Dependencies

```bash
docker-compose -f docker-compose.infra.yml up -d
docker-compose -f docker-compose.otel.yml up -d
docker-compose up -d
```

## k6

```bash
# influxdb
k6 run --out influxdb=http://localhost:8086 tests/script.js

#prometheus
K6_PROMETHEUS_RW_SERVER_URL=http://localhost:9090/api/v1/write
k6 run -o experimental-prometheus-rw script.js
```

## Documentation Reference

[Trace logs with Grafana Tempo and Loki](https://grafana.com/docs/grafana/next/datasources/tempo/configure-tempo-data-source/#trace-to-logs)

## Grafana Dashboards

[New K6 Load Testing Results -> Mais Preciso](https://grafana.com/grafana/dashboards/14796-new-k6-load-testing-results)
[k6 Load Testing Results -> Mais Popular](https://grafana.com/grafana/dashboards/2587-k6-load-testing-results)

## Métricas

### Basket

- [x] Total de Carrinhos Criados | `Counter`
- [x] Total de Carrinhos Removidos | `Counter`
- [ ] Quantidade de Produtos por Carrinho (tag categorias) | `Histograma` - TODO add categoria nos produtos

### Products

- [ ] Total de Produtos Adicionados | `Counter`
- [ ] Total de Produtos Atualizados | `Counter`
- [ ] Duração da Importação de Produtos | `Histograma`
- [ ] Duração da Importação de Imagens de Produtos | `Histograma`
- [ ] Total de Produtos Sendo Processados | `UpDownCounter`
- [ ] Total do Estoque de Produtos | `Gauge`
  
### Pedidos

- [x] Duração da Criação de Pedidos | `Histograma`
- [x] Total de Pedidos Criados | `Gauge`
- [x] Total de Pedidos Pendentes | `Gauge`
- [x] Total de Pedidos Confirmados | `Gauge`
- [ ] Quantidade de Produtos Por Pedido | `Histograma`
- [ ] Valores por Pedido | `Histograma`
- [x] Tempo de Confirmação de Pedido | `Histograma`
