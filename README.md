# ToDo List – Minimal API

A task management application with a **React** client and an **ASP.NET Core Minimal API** server (.NET 9), using **MySQL** and **JWT** authentication.

## Features

- Create, update, complete and delete tasks
- User registration and login with a JWT token
- Swagger UI for exploring the API
- CORS configured for the React client

## Tech Stack

- ASP.NET Core Minimal API (.NET 9)
- Entity Framework Core with Pomelo MySQL provider, and Dapper for queries
- JWT Bearer authentication
- MySQL
- React client

## API

| Method | Endpoint | Description |
|---|---|---|
| GET | `/` | All tasks |
| POST | `/` | Add a task |
| PUT | `/{id}` | Update a task |
| PUT | `/{id}/{isComplete}` | Mark a task as complete / not complete |
| DELETE | `/{id}` | Delete a task |
| POST | `/register` | Register |
| POST | `/login` | Login – returns a JWT |

## Getting Started

1. Create a MySQL database named `tododb`.
2. Update the connection string `ToDoDB` in `ServerSide/appsettings.json`.
3. Run the server:
   ```bash
   cd ServerSide
   dotnet run
   ```
