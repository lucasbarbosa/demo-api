# 🎯 Quick Start Guide - Postman Collections

## 📊 Collections Comparison

| Feature | Swagger | JWT | Docker |
|---------|---------|-----|--------|
| **Port** | 5084 | 5100 | 5200 |
| **Protocol** | HTTP/HTTPS | HTTP/HTTPS | HTTP |
| **Authentication** | ❌ No | ✅ JWT Bearer | ✅ JWT Bearer |
| **Security Key** | - | `b5b622cd-9f73-43b8-8dce-aab520cf1a2b` | `b5b622cd-9f73-43b8-8dce-aab520cf1a2b` |
| **Auth Endpoints** | 0 | 1 | 1 |
| **Product Endpoints** | 5 | 5 | 5 |
| **Total Requests** | 5 | 6 | 6 |
| **Auto-save Token** | - | ✅ Yes | ✅ Yes |
| **Environment** | Local | Local | Docker Container |

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

### Collection: DemoApi-JWT
```
┌─────────────────────────────────────┐
│  1. Import Collection               │
│  2. Generate JWT Token              │
│     POST /auth/token                │
│     Header: X-Security-Key          │
│     ↓                               │
│     Token saved in {{token}}        │
│  3. Execute Products endpoints      │
│     (token included automatically)  │
│     • GET /products                 │
│     • GET /products/{id}            │
│     • POST /products                │
│     • PUT /products                 │
│     • DELETE /products/{id}         │
└─────────────────────────────────────┘
```

### Collection: DemoApi-Docker
```
┌─────────────────────────────────────┐
│  0. Start Docker Container          │
│     docker-compose up -d            │
│  1. Import Collection               │
│  2. Generate JWT Token              │
│     POST /auth/token                │
│     Header: X-Security-Key          │
│     ↓                               │
│     Token saved in {{token}}        │
│  3. Execute Products endpoints      │
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

For **JWT** and **Docker** collections, use the following header in the `/auth/token` endpoint:

```
X-Security-Key: b5b622cd-9f73-43b8-8dce-aab520cf1a2b
```

> **💡 Tip:** This value is already configured in the `{{securityKey}}` variable in the collections!

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
- **With local JWT?** → Use `DemoApi-JWT`
- **With Docker?** → Use `DemoApi-Docker`

### 2. Import into Postman
Drag the `.json` file to Postman or use the **Import** button.

### 3. Execute!
- **Swagger:** Execute Products endpoints directly
- **JWT/Docker:** First execute `Generate JWT Token`, then Products endpoints

---

## 🎨 Folder Structure

```
Postman/
├── DemoApi-Swagger.postman_collection.json    (5.7 KB)
├── DemoApi-JWT.postman_collection.json        (15.1 KB)
├── DemoApi-Docker.postman_collection.json     (15.3 KB)
├── README.md                                   (Complete documentation)
├── QUICK_START.md                             (This file)
└── EXAMPLES.md                                (Request/response examples)
```

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
