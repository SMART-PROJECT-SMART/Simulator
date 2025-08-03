# 🛩️ UAV Flight Simulation System

A comprehensive **Unmanned Aerial Vehicle (UAV) Flight Simulation System** built with **.NET 9** and **ASP.NET Core**. This advanced simulation platform provides realistic flight path calculations, real-time telemetry tracking, and multi-UAV mission management capabilities.

[![.NET 9](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-9.0-green.svg)](https://docs.microsoft.com/en-us/aspnet/core/)
[![Quartz.NET](https://img.shields.io/badge/Quartz.NET-3.14.0-orange.svg)](https://www.quartz-scheduler.net/)
[![C#](https://img.shields.io/badge/C%23-13.0-purple.svg)](https://docs.microsoft.com/en-us/dotnet/csharp/)

## 🎯 Project Overview

This simulation system models realistic UAV operations including:
- **Advanced flight physics** with aerodynamic calculations
- **Real-time telemetry** and position tracking
- **Multi-UAV mission management** with concurrent operations
- **Sophisticated scheduling** using Quartz.NET background jobs
- **REST API** for mission control and monitoring

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
```

### **UAV Types Supported**

#### **🔍 Surveillance UAVs**
- **Searcher** - Lightweight reconnaissance drone
- **Hermes 450** - Medium-range surveillance platform
- **SurveillanceUAV** base class with sensor integration

#### **⚔️ Armed UAVs**
- **Hermes 900** - Armed reconnaissance UAV
- **Heron TP** - Large multi-role combat drone
- **ArmedUAV** base class with weapon systems

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

| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/simulation/simulate` | Start a new UAV mission |
| `POST` | `/api/simulation/switch` | Change UAV destination mid-flight |
| `POST` | `/api/simulation/pause/{tailId}` | Pause specific mission |
| `POST` | `/api/simulation/resume/{tailId}` | Resume paused mission |
| `POST` | `/api/simulation/abort/{tailId}` | Abort specific mission |
| `POST` | `/api/simulation/abort-all` | Emergency abort all missions |

### **Monitoring**

| Method | Endpoint | Description |
|--------|----------|-------------|
| `GET` | `/api/simulation/status` | Get system status and active UAVs |
| `GET` | `/api/simulation/run` | Demo single UAV mission |
| `GET` | `/api/simulation/run-multi` | Demo multiple UAV missions |

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

#### **Get System Status**
```json
GET /api/simulation/status

Response:
{
  "activeUAVs": 2,
  "activeJobs": 2,
  "activeTailIds": [1, 2]
}
```

## 🛠️ Key Features

### **🎮 Advanced Flight Simulation**
- **Realistic Physics**: Aerodynamic forces, drag, and lift calculations
- **Flight Phases**: Takeoff, cruise, navigation, and landing
- **Environmental Factors**: Altitude effects, air density variations
- **Fuel Management**: Consumption based on throttle and thrust

### **📊 Comprehensive Telemetry**
- **19 Telemetry Fields**: Position, orientation, engine data, sensors
- **Real-time Updates**: 1-second interval position calculations
- **Data Compression**: Efficient telemetry storage and transmission
- **Signal Strength**: Realistic communication range modeling

### **🎯 Mission Control**
- **Dynamic Destination Switching**: Change targets mid-flight
- **Multi-UAV Coordination**: Concurrent mission management
- **Mission States**: Pause, resume, abort capabilities
- **Collision Avoidance**: Safety considerations in path planning

### **⚡ Background Processing**
- **Quartz.NET Integration**: Reliable job scheduling
- **Graceful Shutdown**: Waits for job completion on exit
- **Error Recovery**: Robust error handling and logging
- **Scalable Architecture**: Handles multiple concurrent UAVs

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
    // ... additional flight parameters
}
```

## 📁 Project Structure

```
Simulation/
├── Controllers/
│   └── SimulationController.cs        # REST API endpoints
├── Services/
│   ├── UAVManager/                    # Mission management
│   ├── Quartz/                        # Job scheduling
│   ├── Flight_Path/                   # Flight calculations
│   │   ├── Motion_Calculator/         # Position updates
│   │   ├── Speed_Controller/          # Velocity management
│   │   └── Orientation_Calculator/    # Attitude control
│   └── Helpers/                       # Utility functions
├── Models/
│   ├── UAVs/                         # UAV definitions
│   │   ├── SurveillanceUAV/          # Reconnaissance drones
│   │   └── ArmedUAV/                 # Combat drones
│   └── Location.cs                   # Geographic coordinates
├── Common/
│   ├── constants/                    # System constants
│   └── Enums/                        # Type definitions
└── Dto/                              # Data transfer objects
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

## 🚧 Future Enhancements

- [ ] **3D Visualization**: Real-time flight path rendering
- [ ] **Weather Integration**: Wind, turbulence, weather effects
- [ ] **Formation Flying**: Multi-UAV coordination algorithms
- [ ] **Obstacle Avoidance**: Terrain and no-fly zone integration
- [ ] **Mission Planning**: Waypoint-based route planning
- [ ] **Real Hardware Integration**: Connect to actual UAV systems

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🎓 Technical Specifications

- **Framework**: ASP.NET Core 9.0
- **Language**: C# 13.0
- **Job Scheduling**: Quartz.NET 3.14.0
- **Logging**: Serilog
- **API Documentation**: OpenAPI/Swagger
- **Architecture**: Clean Architecture with DI
- **Concurrency**: Thread-safe operations with ConcurrentDictionary

---

**Built for advanced UAV simulation and research applications** 🚁✈️
