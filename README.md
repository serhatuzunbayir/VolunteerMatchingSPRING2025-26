# Virtual Event Scheduler and Management Platform
**SE 410 — Spring 2025-26 | Group Project**

A full-stack virtual event scheduling platform built with ASP.NET Core, Windows Forms, and .NET MAUI.

---

## How to Run

### Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 9.0 or 10.0 |
| Git | any recent version |

**Mac users** also need the MAUI workload:
```bash
dotnet workload install maui
```

---

### Step 1 — Clone the repo

```bash
git clone <repo-url>
cd VolunteerMatchingSPRING2025-26/VirtualEventScheduler
```

---

### Step 2 — Start the API (both Mac & Windows)

```bash
dotnet run --project VirtualEventScheduler.API/VirtualEventScheduler.API.csproj
```

The API starts on **http://localhost:5202**  
Swagger UI: http://localhost:5202/swagger  
Default admin credentials: `admin@eventapp.com` / `Admin123!`

> The SQLite database (`EventScheduler.db`) is created automatically in the same folder as the executable.

---

### Step 3 — Start the Web App (both Mac & Windows)

Open a second terminal:

```bash
dotnet run --project VirtualEventScheduler.Web/VirtualEventScheduler.Web.csproj
```

Open your browser at **http://localhost:5203**

---

### Step 4 — Desktop App

**Windows** (Windows Forms):
```bash
dotnet run --project VirtualEventScheduler.Desktop/VirtualEventScheduler.Desktop.csproj
```

**Mac** (.NET MAUI):
```bash
dotnet run --project VirtualEventScheduler.Desktop.Maui/VirtualEventScheduler.Desktop.Maui.csproj -f net9.0-maccatalyst
```

> The desktop app connects to the API at `http://localhost:5202`. Make sure the API is running first.

---

## Project Structure

```
VirtualEventScheduler/
├── VirtualEventScheduler.API/          ← ASP.NET Core REST API (cross-platform)
├── VirtualEventScheduler.Data/         ← EF Core + SQLite (shared data layer)
├── VirtualEventScheduler.Web/          ← ASP.NET Core MVC web app (cross-platform)
├── VirtualEventScheduler.Desktop/      ← Windows Forms desktop (Windows only)
└── VirtualEventScheduler.Desktop.Maui/ ← .NET MAUI desktop (macOS)
```

## Features

- **Event Scheduler** — Create, edit, and cancel virtual events (Admin/Staff)
- **Participant Tracking** — View registered attendees per event (Admin/Staff)  
- **Notifications** — Delegate-based notification system logs all key actions
- **LINQ Filtering** — Filter events by date range and status
- **My Events** — Users see their registered events with upcoming reminders
- **Role-Based Access** — Admin, Staff, and Attendee roles

## Default Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@eventapp.com | Admin123! |

New registrations default to the **Attendee** role. Admins can promote users to Staff.
