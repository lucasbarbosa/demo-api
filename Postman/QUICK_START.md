# 🎯 Quick Start Guide - Postman Collections

## 📊 Collections Comparison

| Feature | Swagger | JWT |
|---------|---------|-----|
| **Ports** | 5084 | 5100 (Local) / 5200 (Docker) |
| **Protocol** | HTTP/HTTPS | HTTP/HTTPS |
| **Authentication** | ❌ No | ✅ JWT Bearer |
| **Security Key** | - | `b5b622cd-9f73-43b8-8dce-aab520cf1a2b` |
| **Environments** | None | Local / Docker |
| **Auth Endpoints** | 0 | 1 |
| **Product Endpoints** | 5 | 5 |
| **Total Requests** | 5 | 6 |
| **Auto-save Token** | - | ✅ Yes |

---

## 🌍 Environments

| Environment | Port | Base URL | Use Case |
|-------------|------|----------|----------|
| **DemoApi - Local** | 5100 | `http://localhost:5100/api/v1` | Local development |
| **DemoApi - Docker** | 5200 | `http://localhost:5200/api/v1` | Docker container |

---

## 🚦 Usage Flow

### Collection: DemoApi-Swagger
```
┌─────────────────────────────────────┐
│  1. Import Collection               │
│  2. Execute any endpoint            │
│     • GET /products                 │
│     • GET /products/{id}            │
│     • POST /products                │
│     • PUT /products                 │
│     • DELETE /products/{id}         │
└─────────────────────────────────────┘
```

### Collection: DemoApi-JWT (with Local Environment)
```
┌─────────────────────────────────────┐
│  1. Import Collection               │
│  2. Import Environment              │
│     DemoApi-Local.postman_env.json  │
│  3. Select Environment              │
│     "DemoApi - Local" (top-right)   │
│  4. Generate JWT Token              │
│     POST /auth/token                │
│     Header: X-Security-Key          │
│     ↓                               │
│     Token saved in {{token}}        │
│  5. Execute Products endpoints      │
│     (token included automatically)  │
│     • GET /products                 │
│     • GET /products/{id}            │
│     • POST /products                │
│     • PUT /products                 │
│     • DELETE /products/{id}         │
└─────────────────────────────────────┘
```

### Collection: DemoApi-JWT (with Docker Environment)
```
┌─────────────────────────────────────┐
│  0. Start Docker Container          │
│     docker-compose up -d            │
│  1. Import Collection               │
│  2. Import Environment              │
│     DemoApi-Docker.postman_env.json │
│  3. Select Environment              │
│     "DemoApi - Docker" (top-right)  │
│  4. Generate JWT Token              │
│     POST /auth/token                │
│     Header: X-Security-Key          │
│     ↓                               │
│     Token saved in {{token}}        │
│  5. Execute Products endpoints      │
│     (token included automatically)  │
│     • GET /products                 │
│     • GET /products/{id}            │
│     • POST /products                │
│     • PUT /products                 │
│     • DELETE /products/{id}         │
└─────────────────────────────────────┘
```

---

## 🔑 Security Key

For the **JWT** collection, use the following header in the `/auth/token` endpoint:

```
X-Security-Key: b5b622cd-9f73-43b8-8dce-aab520cf1a2b
```

> **💡 Tip:** This value is already configured in the `{{securityKey}}` variable in both environments!

---

## 📦 Product Payload Example

### Create (POST)
```json
{
  "id": 0,
  "name": "Dell Inspiron Laptop",
  "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor",
  "price": 3499.99,
  "active": true
}
```

### Update (PUT)
```json
{
  "id": 1,
  "name": "Dell Inspiron Laptop - Updated",
  "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor - 11th Gen",
  "price": 3299.99,
  "active": true
}
```

---

## ⚡ Quick Start

### 1. Choose Your Collection
- **No authentication?** → Use `DemoApi-Swagger`
- **With JWT authentication?** → Use `DemoApi-JWT`

### 2. Import into Postman
- **Collection:** Drag the `.json` file to Postman or use the **Import** button
- **Environment:** (JWT only) Import the environment file for Local or Docker

### 3. Select Environment (JWT only)
- Click the environment dropdown (top-right corner)
- Select **DemoApi - Local** or **DemoApi - Docker**

### 4. Execute!
- **Swagger:** Execute Products endpoints directly
- **JWT:** First execute `Generate JWT Token`, then Products endpoints

---

## 🎨 Folder Structure

```
Postman/
├── DemoApi-Swagger.postman_collection.json       (No auth)
├── DemoApi-JWT.postman_collection.json           (JWT auth)
├── DemoApi-Local.postman_environment.json        (Local env - port 5100)
├── DemoApi-Docker.postman_environment.json       (Docker env - port 5200)
├── README.md                                      (Complete documentation)
├── QUICK_START.md                                (This file)
└── EXAMPLES.md                                   (Request/response examples)
```

---

## 🔄 Switching Environments

To switch between Local and Docker:

1. Click the **environment dropdown** (top-right corner)
2. Select the desired environment:
   - **DemoApi - Local** → Port 5100
   - **DemoApi - Docker** → Port 5200
3. All requests will now use the selected environment's `{{baseUrl}}`

> **💡 Tip:** You can open the same collection in multiple tabs with different environments!

---

## 🔧 Docker Commands

### Start Container
```bash
cd core/swagger-jwt-docker/docker
docker-compose up -d
```

### Check Status
```bash
docker ps
```

### View Logs
```bash
docker-compose logs -f
```

### Stop Container
```bash
docker-compose down
```
