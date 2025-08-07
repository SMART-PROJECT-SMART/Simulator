# 🛩️ UAV Flight Simulation System

A comprehensive **Unmanned Aerial Vehicle (UAV) Flight Simulation System** built with **.NET 9** and **ASP.NET Core**. This advanced simulation platform provides realistic flight path calculations, real-time telemetry tracking, and multi-UAV mission management capabilities.

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-green.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Quartz.NET](https://img.shields.io/badge/Quartz.NET-3.14.0-orange.svg)](https://www.quartz-scheduler.net/)
[![C#](https://img.shields.io/badge/C%23-13.0-purple.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

## 🎯 Project Overview

This simulation system models realistic UAV operations including:

- **Advanced flight physics** with aerodynamic calculations
- **Real-time telemetry** and position tracking (21 telemetry fields)
- **Multi-UAV mission management** with concurrent operations
- **Sophisticated scheduling** using Quartz.NET background jobs
- **REST API** for mission control and monitoring
- **Interface Control Document (ICD)** support for standardized communication protocols
- **Port management** for communication channel handling

## 🏗️ Architecture

### **Core Components**

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Controllers   │────│   UAV Manager    │────│  Quartz Manager │
│  (REST API)     │    │ (Mission Logic)  │    │ (Job Scheduling)│
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                │
                ┌───────────────┼───────────────┐
                │               │               │
        ┌───────▼──────┐ ┌──────▼──────┐ ┌─────▼──────┐
        │Flight Path   │ │   Motion    │ │ Speed      │
        │Service       │ │ Calculator  │ │Controller  │
        └──────────────┘ └─────────────┘ └────────────┘
                                │
                        ┌───────▼──────┐
                        │ Orientation  │
                        │ Calculator   │
                        └──────────────┘
```

### **UAV Types Supported**

#### **🔍 Surveillance UAVs**

- **Searcher** - Lightweight reconnaissance drone (120kg, 180 km/h max speed, 600m cruise altitude)
- **Hermes 450** - Medium-range surveillance platform (450kg, 220 km/h max speed, 550m cruise altitude)
- **SurveillanceUAV** base class with sensor integration and data storage capabilities

#### **⚔️ Armed UAVs**

- **Hermes 900** - Armed reconnaissance UAV (1100kg, 220 km/h max speed, 900m cruise altitude)
- **Heron TP** - Large multi-role combat drone (4650kg, 220 km/h max speed, 135m cruise altitude)
- **ArmedUAV** base class with weapon systems (Hellfire, Spike NLOS, Griffin, JDAM)

## 🚀 Getting Started

### **Prerequisites**

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)

### **Installation**

1. **Clone the repository**

   ```bash
   git clone <repository-url>
   cd Simulation
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Build the project**

   ```bash
   dotnet build
   ```

4. **Run the application**

   ```bash
   dotnet run
   ```

5. **Access the API**
   - **Development**: `https://localhost:5001`
   - **Swagger UI**: `https://localhost:5001/swagger` (Development only)

## 📡 API Endpoints

### **Mission Management**

| Method | Endpoint                          | Description                       |
| ------ | --------------------------------- | --------------------------------- |
| `POST` | `/api/simulation/simulate`        | Start a new UAV mission           |
| `POST` | `/api/simulation/switch`          | Change UAV destination mid-flight |
| `POST` | `/api/simulation/pause/{tailId}`  | Pause specific mission            |
| `POST` | `/api/simulation/resume/{tailId}` | Resume paused mission             |
| `POST` | `/api/simulation/abort/{tailId}`  | Abort specific mission            |
| `POST` | `/api/simulation/abort-all`       | Emergency abort all missions      |

### **Communication Management**

| Method | Endpoint                          | Description                |
| ------ | --------------------------------- | -------------------------- |
| `POST` | `/api/communication/switch-ports` | Switch communication ports |

### **Monitoring**

| Method | Endpoint                    | Description                       |
| ------ | --------------------------- | --------------------------------- |
| `GET`  | `/api/simulation/status`    | Get system status and active UAVs |
| `GET`  | `/api/simulation/run`       | Demo single UAV mission           |
| `GET`  | `/api/simulation/run-multi` | Demo multiple UAV missions        |

### **Example Usage**

#### **Start a Mission**

```json
POST /api/simulation/simulate
{
  "UAV": {
    "TailId": 1,
    "StartLocation": {
      "Latitude": 40.6413,
      "Longitude": -73.7781,
      "Altitude": 10.0
    }
  },
  "Destination": {
    "Latitude": 40.6460,
    "Longitude": -73.7785,
    "Altitude": 100.0
  },
  "MissionId": "MISSION_001"
}
```

#### **Switch Communication Ports**

```json
POST /api/communication/switch-ports
{
  "SourcePort": 8001,
  "TargetPort": 8002
}
```

#### **Get System Status**

```json
GET /api/simulation/status

Response:
{
  "ActiveUAVs": 2,
  "ActiveJobs": 2,
  "ActiveTailIds": [1, 2]
}
```

## 🛠️ Key Features

### **🎮 Advanced Flight Simulation**

- **Realistic Physics**: Aerodynamic forces, drag, and lift calculations
- **Flight Phases**: Takeoff, cruise, navigation, and landing
- **Environmental Factors**: Altitude effects, air density variations
- **Fuel Management**: Consumption based on throttle and thrust
- **Engine Simulation**: RPM and engine degrees calculations
- **Flight Envelope**: Pitch/roll limits, turn rates, climb/descent rates

### **📊 Comprehensive Telemetry (21 Fields)**

- **Position Data**: Latitude, longitude, altitude
- **Flight Dynamics**: Speed, yaw, pitch, roll
- **Engine Data**: Throttle percentage, thrust, RPM, engine degrees
- **System Status**: Landing gear, fuel amount, flight time
- **Communication**: Signal strength, nearest sleeve ID
- **Aerodynamic Data**: Drag coefficient, lift coefficient
- **Surveillance Data**: Data storage usage (for surveillance UAVs)

### **🎯 Mission Control**

- **Dynamic Destination Switching**: Change targets mid-flight
- **Multi-UAV Coordination**: Concurrent mission management
- **Mission States**: Pause, resume, abort capabilities
- **Collision Avoidance**: Safety considerations in path planning
- **Real-time Updates**: 1-second interval position calculations

### **⚡ Background Processing**

- **Quartz.NET Integration**: Reliable job scheduling
- **Graceful Shutdown**: Waits for job completion on exit
- **Error Recovery**: Robust error handling and logging
- **Scalable Architecture**: Handles multiple concurrent UAVs

### **🔌 Communication & ICD Support**

- **Port Management**: Dynamic port assignment and switching
- **ICD Integration**: Support for North/South ICD protocols
- **Telemetry Compression**: Efficient data transmission
- **Channel Management**: Multi-channel communication support

## 🔧 Configuration

### **UAV Specifications**

Each UAV type has detailed specifications in `SimulationConstants.cs`:

```csharp
// Example: Searcher UAV Configuration
public static class Searcher_Constants
{
    public const double MaxAcceleration = 3.0;          // m/s²
    public const double MaxCruiseSpeedKmph = 180;       // km/h
    public const double CruiseAltitude = 600.0;         // meters
    public const double FuelTankCapacity = 120.0;       // kg
    public const double Mass = 120.0;                   // kg
    public const double DataStorageCapacityGB = 250.0;  // GB
    // ... additional parameters
}
```

### **Flight Parameters**

Key simulation parameters:

```csharp
public static class FlightPath
{
    public const double DELTA_SECONDS = 1.0;           // Update interval
    public const double MISSION_COMPLETION_RADIUS_M = 10.0;  // Target accuracy
    public const double MAX_PITCH_DEG = 30.0;          // Flight envelope
    public const double MAX_ROLL_DEG = 45.0;           // Banking limits
    public const double GRAVITY_MPS2 = 9.81;           // Earth gravity
    // ... additional flight parameters
}
```

### **ICD Configuration**

Interface Control Documents define telemetry field specifications:

```json
{
  "name": "Latitude",
  "type": "Double",
  "unit": "deg",
  "minValue": -90.0,
  "maxValue": 90.0,
  "startBitArrayIndex": 55,
  "bitLength": 25
}
```

## 📁 Project Structure

```
Simulation/
├── Controllers/
│   ├── SimulationController.cs        # Mission management API
│   └── CommunicationController.cs     # Communication API
├── Services/
│   ├── UAVManager/                    # Mission management
│   │   ├── UAVManager.cs             # Core UAV coordination
│   │   └── IUAVManager.cs            # Interface definition
│   ├── Quartz/                        # Job scheduling
│   │   ├── QuartzFlightJobManager.cs # Background job management
│   │   └── Jobs/
│   │       └── FlightPathUpdateJob.cs # Flight path updates
│   ├── Flight Path/                   # Flight calculations
│   │   ├── FlightPathService.cs      # Main flight logic
│   │   ├── Motion Calculator/        # Position updates
│   │   ├── Speed Controller/         # Velocity management
│   │   └── Orientation Calculator/   # Attitude control
│   ├── ICDDirectory/                  # Interface definitions
│   ├── PortManager/                   # Communication ports
│   └── Helpers/                       # Utility functions
├── Models/
│   ├── UAVs/                         # UAV definitions
│   │   ├── SurveillanceUAV/          # Reconnaissance drones
│   │   ├── ArmedUAV/                 # Combat drones
│   │   └── UAV.cs                    # Base UAV class
│   ├── Channels/                     # Communication channels
│   └── Location.cs                   # Geographic coordinates
├── Common/
│   ├── constants/                    # System constants
│   │   └── SimulationConstants.cs    # All UAV specs & parameters
│   └── Enums/                        # Type definitions
├── Dto/                              # Data transfer objects
├── Files/
│   └── ICD/                          # Interface Control Documents
└── Configuration/                    # App settings
```

## 🧪 Testing

### **Quick Test Commands**

```bash
# Start single UAV mission
curl -X GET "https://localhost:5001/api/simulation/run"

# Start multiple UAV missions
curl -X GET "https://localhost:5001/api/simulation/run-multi"

# Check system status
curl -X GET "https://localhost:5001/api/simulation/status"

# Pause a mission
curl -X POST "https://localhost:5001/api/simulation/pause/1"

# Switch communication ports
curl -X POST "https://localhost:5001/api/communication/switch-ports" \
  -H "Content-Type: application/json" \
  -d '{"SourcePort": 8001, "TargetPort": 8002}'
```

## 📋 System Requirements

### **Runtime Requirements**

- **.NET 9 Runtime**
- **Memory**: 512MB minimum, 2GB recommended
- **CPU**: Multi-core recommended for concurrent UAV simulations
- **Network**: HTTP/HTTPS support

### **Development Requirements**

- **Visual Studio 2022** (17.8+) or **VS Code**
- **.NET 9 SDK**
- **C# 13.0** language features

## 🔍 Logging and Monitoring

The system uses **Serilog** for comprehensive logging:

```csharp
// Example log output
[INFO] UAV 1 | Lat 40.641500 | Lon -73.778200 | Alt 85.5m | Spd 120.0km/h |
       Yaw 270.0° | Pitch 5.2° | Roll 0.0° | Rem 125.3m | Fuel 45.230kg
```

**Log Categories:**

- 🛩️ **Flight Operations**: Position updates, navigation events
- ⚙️ **System Events**: Mission start/stop, job scheduling
- ⚠️ **Warnings**: Fuel low, signal loss, mission issues
- 🚨 **Errors**: System failures, calculation errors

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 🎓 Technical Specifications

- **Framework**: ASP.NET Core 9.0
- **Language**: C# 13.0
- **Job Scheduling**: Quartz.NET 3.14.0
- **Logging**: Serilog 4.3.1
- **JSON Processing**: Newtonsoft.Json 13.0.4
- **API Documentation**: OpenAPI/Swagger
- **Architecture**: Clean Architecture with Dependency Injection
- **Concurrency**: Thread-safe operations with ConcurrentDictionary
- **Telemetry**: 21-field comprehensive data model
- **Communication**: Multi-port channel management

## 🔧 Dependencies

```xml
<PackageReference Include="Quartz" Version="3.14.0" />
<PackageReference Include="Serilog" Version="4.3.1-dev-02373" />
<PackageReference Include="Newtonsoft.Json" Version="13.0.4-beta1" />
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="9.0.7" />
<PackageReference Include="JetBrains.Annotations" Version="2025.2.0" />
```

---

**Built for advanced UAV simulation and research applications** 🚁✈️
