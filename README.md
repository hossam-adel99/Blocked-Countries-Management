# 🌍 Country Blocking & IP GeoLocation API

[![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet?logo=dotnet)](https://dotnet.microsoft.com/)
[![C#](https://img.shields.io/badge/Language-C%23-green)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![Swagger](https://img.shields.io/badge/API-Docs-brightgreen?logo=swagger)](https://swagger.io/)
[![Status](https://img.shields.io/badge/Status-Active-success)](#)

---

## 📘 Overview

**WebApplication1** is a modern **ASP.NET Core Web API** for managing **country blocking** and performing **IP geolocation** lookups.  
It integrates with [ipapi.co](https://ipapi.co/json/) to identify the location and organization details of an IP address.

### ✨ Highlights
- 🌍 Detect visitor’s country and organization from IP  
- 🚫 Block countries permanently or temporarily  
- 🧾 Log all incoming requests (IP, country, timestamp, User-Agent)  
- 🔁 Automatically remove expired temporary blocks  
- 🧩 Interactive API documentation with **Swagger UI**

---

## 🚀 Features

| Feature | Description |
|----------|-------------|
| 🌎 **GeoLocation Lookup** | Retrieves IP info (city, region, country, org) using `ipapi.co` |
| 🚫 **Country Blocking** | Supports both permanent and temporary blocks |
| 🧾 **Request Logging** | Tracks every request with timestamp & User-Agent |
| 🔁 **Background Cleanup** | Removes expired temporary blocks automatically |
| 🧩 **Swagger UI** | Test APIs directly from your browser |

---

## 🧱 Project Architecture

**WebApplication1/**<br>
│<br>
├── **Controllers/**<br>
│ ├── CountriesController.cs # Manage country blocking/unblocking<br>
│ ├── IpController.cs # Lookup & check IP block status<br>
│ └── LogsController.cs # Retrieve logs<br>
│<br>
├── **Models/**<br>
│ ├── BlockedCountry.cs # Represents a blocked country<br>
│ ├── IpLookupResponse.cs # IP API response mapping<br>
│ └── LogEntry.cs # Represents request log data<br>
│<br>
├── **Services/**<br>
│ ├── IBlockedCountryService.cs # Interface for country block logic<br>
│ ├── BlockedCountryService.cs # Implements in-memory block store<br>
│ ├── IGeoLocationService.cs # Interface for Geo lookup services<br>
│ ├── IpApiGeoLocationService.cs # Calls ipapi.co API<br>
│ ├── ILogService.cs # Interface for logging service<br>
│ ├── LogService.cs # Handles logging<br>
│ └── TemporalBlockCleaner.cs # Background service for cleanup<br>
│<br>
├── Program.cs # App startup & dependency injection<br>
└── WebApplication1.csproj # Project file<br>

---

## 🔄 System Data Flow

[Client Request]<br>
↓<br>
[IpController] → [IGeoLocationService] → ipapi.co API<br>
↓<br>
[BlockedCountryService] → Checks if the country is blocked<br>
↓<br>
[LogService] → Logs request info (IP, country, blocked, UA)<br>
↓<br>
[Response → Client]<br>

---

## ⚙️ API Endpoints
### 🔍 1. Lookup IP Location
```http
GET /api/ip/lookup?ipAddress=8.8.8.8
```
Response:
```json
{
  "ip": "8.8.8.8",
  "city": "Mountain View",
  "region": "California",
  "country": "United States",
  "countryCode": "US",
  "org": "Google LLC"
}
```
### 🚫 2. Check If IP Is Blocked
```http
GET /api/ip/check-block
```
Response:
```json
{
  "ip": "156.193.xxx.xxx",
  "country": "Egypt",
  "countryCode": "EG",
  "blocked": false
}
```
### 🧱 3. Block a Country Permanently
```http
POST /api/countries/block
```
Body:
```json
{
  "countryCode": "US",
  "countryName": "United States"
}
```
Response:
```swift
201 Created
```
### 🔓 4. Unblock a Country
```http
DELETE /api/countries/block/{countryCode}
```
Example:
```http
DELETE /api/countries/block/US
```
Response:
```swift
204 No Content
```
### ⏳ 5. Temporarily Block a Country
```http
POST /api/countries/temporal-block
```
Body:
```json
{
  "countryCode": "EG",
  "countryName": "Egypt",
  "durationMinutes": 10
}
```
Response:
```swift
200 OK
```
### 🧾 6. Retrieve Logs of Access Attempts
```http
GET /api/logs/blocked-attempts
```
Response Example:
```json
{
  "total": 5,
  "page": 1,
  "pageSize": 10,
  "items": [
    {
      "ip": "8.8.8.8",
      "timestampUtc": "2025-10-16T11:10:00Z",
      "countryCode": "US",
      "blocked": true,
      "userAgent": "Mozilla/5.0 ..."
    }
  ]
}
```

---

## 🧠 Background Service – TemporalBlockCleaner

This background worker runs every **5 minutes** to remove expired temporary country blocks.
```csharp
protected override async Task ExecuteAsync(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        var expired = _blockedSrv.GetBlockedCountries()
            .Where(c => c.TemporalUntilUtc <= DateTime.UtcNow)
            .Select(c => c.CountryCode)
            .ToList();

        foreach (var code in expired)
            _blockedSrv.TryRemoveBlockedCountry(code);

        await Task.Delay(TimeSpan.FromMinutes(5), token);
    }
}
```

---

## 🧰 Tech Stack

| Technology | Purpose |
|-------------|----------|
| .NET 8 / ASP.NET Core Web API | Backend framework |
| C# | Main programming language |
| HttpClient | External IP lookup requests |
| Swagger / Swashbuckle | Auto-generated API docs |
| BackgroundService | Periodic cleanup service |
| ConcurrentDictionary | In-memory, thread-safe data store |

---

## 💻 Run Locally

1. Clone the repository
```bach
git clone https://github.com/<your-username>/WebApplication1.git
cd WebApplication1
```
2. Restore dependencies
```bach
dotnet restore
```
3. Run the project
```bach
dotnet run
```
4. Open Swagger UI
```bach
https://localhost:5001/swagger
```

---

## 🔒 Optional API Security
You can add authentication to protect endpoints (like /block and /unblock) using an **API Key** or **JWT Token**.
Example:
```csharp
[ApiKeyAuth]
[HttpPost("block")]
public IActionResult BlockCountry(...) { ... }
```

---

## 👨‍💻 Author

**Hossam Adel**
💼 .NET Developer
📧 [LinkedIn](https://www.linkedin.com/in/hossam-adel99) | 💻 [GitHub](https://github.com/hossam-adel99)

---

## ⭐ Contribute

If you like this project:
- ⭐ Star the repo
- 🪄 Fork it and improve it
- 💬 Open an issue or PR for feedback

---

Made with ❤️ using **.NET 8**, **C#**, and **Swagger**
Secure • Scalable • Developer-Friendly
