# Todos – Node.js Server

A **Node.js + Express** REST API for managing users and their tasks, with **JWT** authentication and JSON-file storage.

## Features

- Users – add, login, list, update and delete
- Tasks – list all, get by id, get by user, add, update and delete
- JWT authentication middleware that protects every route except login and registration
- Data stored in JSON files – no database setup needed

## Project Structure

```
app.js        # Express app, CORS, JSON parsing and the JWT middleware
routers/      # userRouter, todoRouter
actions/      # Business logic for users and tasks, JWT helpers
db/           # JSON data file and the read/write queries
```

## API

| Method | Endpoint | Description |
|---|---|---|
| POST | `/user/login` | Login – returns a JWT |
| POST | `/user/add` | Add a user |
| GET | `/user/all` | All users |
| GET | `/user/:id` | User by id |
| PUT | `/user/update/:id` | Update a user |
| DELETE | `/user/delete/:id` | Delete a user |
| GET | `/todo/all` | All tasks |
| GET | `/todo/:id` | Task by id |
| GET | `/todo/byUserId/:id` | Tasks of a user |
| POST | `/todo/add` | Add a task |
| PUT | `/todo/update/:id` | Update a task |
| DELETE | `/todo/delete/:id` | Delete a task |

Protected routes expect the token in the `Authorization` header.

## Tech Stack

Node.js, Express, jsonwebtoken, CORS, nodemon

## Getting Started

```bash
npm install
node app.js
```
