# UAV Simulation System

A .NET 9.0 Web API for simulating UAV (Unmanned Aerial Vehicle) flight operations and mission management.

## Features

- **UAV Management**: Create, retrieve, and update UAV entities with telemetry data
- **Mission Management**: Handle mission creation and assignment to UAVs
- **Flight Path Simulation**: Advanced flight path calculation with different flight phases
- **MongoDB Integration**: Persistent storage using MongoDB
- **RESTful API**: Clean API endpoints for all operations

## Project Structure

```
Simulation/
├── Controllers/          # API controllers
├── Database/            # MongoDB service and configuration
├── Dto/                 # Data Transfer Objects
├── Factories/           # Flight phase strategy factory
├── Mappers/             # Object mapping utilities
├── Models/              # Domain models
├── Ro/                  # Response Objects
├── Services/            # Business logic services
│   ├── Flight Path/     # Flight path calculation services
│   └── helpers/         # Utility helpers
└── Common/              # Shared constants and enums
```

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- MongoDB instance running locally or remotely

### Installation

1. Clone the repository
2. Navigate to the Simulation directory
3. Update the MongoDB connection string in `appsettings.json`
4. Run the application:

```bash
dotnet run
```

The API will be available at `http://localhost:5023`

### API Endpoints

- `POST /api/UAV/create` - Create a new UAV
- `GET /api/UAV/{id}` - Get UAV by ID
- `POST /api/Mission/create` - Create a new mission
- `GET /api/Mission/{id}` - Get mission by ID
- `POST /api/TestFlight/calculate` - Calculate flight path

## Configuration

Update the MongoDB connection string in `appsettings.json`:

```json
{
  "MongoDbSettings": {
    "ConnectionString": "mongodb://localhost:27017",
    "DatabaseName": "UAVSimulation"
  }
}
```

## Technologies Used

- .NET 9.0
- ASP.NET Core Web API
- MongoDB Driver
- Serilog for logging
- OpenAPI/Swagger for API documentation
