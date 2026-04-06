# Surveillance Microservices System

Architecture microservices basée sur .NET 8, RabbitMQ, Docker et Azure.

## Stack

- ASP.NET Core Web API
- MediatR (CQRS)
- RabbitMQ (Event Bus)
- SignalR (Real-time)
- Docker & Kubernetes
- Terraform (IaC)
- Azure DevOps (CI/CD)

## Architecture

- API Gateway (YARP)
- Identity Service (JWT)
- Alert Service (CQRS + Events)
- Notification Service (Consumers)

## Run

```bash
docker-compose up --build