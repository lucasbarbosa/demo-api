# 📬 Postman Collections - DemoApi

This directory contains ready-to-use Postman collections for testing the Demo API in its three different configurations.

## 📁 Available Collections

### 1. **DemoApi-Swagger.postman_collection.json**
Collection for the basic project with Swagger (no authentication).

- **Port:** `5084`
- **Base URL:** `http://localhost:5084/api/v1`
- **Authentication:** None
- **Endpoints:** Full Products CRUD

### 2. **DemoApi-JWT.postman_collection.json**
Collection for the project with JWT authentication.

- **Port:** `5100`
- **Base URL:** `http://localhost:5100/api/v1`
- **Authentication:** Bearer Token (JWT)
- **Security Key:** `b5b622cd-9f73-43b8-8dce-aab520cf1a2b`
- **Endpoints:** Authentication + Full Products CRUD

### 3. **DemoApi-Docker.postman_collection.json**
Collection for the project running in Docker with JWT authentication.

- **Port:** `5200` (mapped to `8080` in container)
- **Base URL:** `http://localhost:5200/api/v1`
- **Authentication:** Bearer Token (JWT)
- **Security Key:** `b5b622cd-9f73-43b8-8dce-aab520cf1a2b`
- **Endpoints:** Authentication + Full Products CRUD

---

## 🚀 How to Import Collections

### Method 1: Import via Postman Interface
1. Open Postman
2. Click **Import** (top left corner)
3. Select the **File** tab
4. Navigate to the `Postman` folder in the repository
5. Select the desired `.json` collection file
6. Click **Import**

### Method 2: Drag and Drop
1. Open Postman
2. Drag the `.json` collection file to the Postman window
3. The collection will be imported automatically

---

## 🔐 How to Use Collections with JWT

### For **DemoApi-JWT** and **DemoApi-Docker** collections:

#### Step 1: Generate JWT Token
1. Open the collection in Postman
2. Navigate to the **Authentication** folder
3. Execute the **Generate JWT Token** request
4. The token will be generated and **automatically saved** in the `{{token}}` variable

#### Step 2: Use Products Endpoints
1. Navigate to the **Products** folder
2. Execute any endpoint (Get All, Get By ID, Create, Update, Delete)
3. The token will be automatically included in the `Authorization: Bearer {{token}}` header

> **💡 Tip:** The post-request script in the `Generate JWT Token` endpoint automatically saves the token in the collection variable. You don't need to copy and paste it manually!

---

## 📝 Collection Variables

Each collection has pre-configured variables:

### DemoApi-Swagger
| Variable | Value |
|----------|-------|
| `baseUrl` | `http://localhost:5084/api/v1` |

### DemoApi-JWT
| Variable | Value |
|----------|-------|
| `baseUrl` | `http://localhost:5100/api/v1` |
| `token` | (automatically generated) |
| `securityKey` | `b5b622cd-9f73-43b8-8dce-aab520cf1a2b` |

### DemoApi-Docker
| Variable | Value |
|----------|-------|
| `baseUrl` | `http://localhost:5200/api/v1` |
| `token` | (automatically generated) |
| `securityKey` | `b5b622cd-9f73-43b8-8dce-aab520cf1a2b` |

---

## 🧪 Request Body Examples

### ProductViewModel (Create)
```json
{
  "id": 0,
  "name": "Dell Inspiron Laptop",
  "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor",
  "price": 3499.99,
  "active": true
}
```

### ProductViewModel (Update)
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

## 📊 Available Endpoints

### Authentication (JWT and Docker only)
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/auth/token` | Generates a JWT token for authentication |

**Required Headers:**
- `X-Security-Key`: Security key configured in appsettings

### Products (all collections)
| Method | Endpoint | Description | Authentication |
|--------|----------|-------------|----------------|
| `GET` | `/products` | List all products | JWT* |
| `GET` | `/products/{id}` | Get product by ID | JWT* |
| `POST` | `/products` | Create a new product | JWT* |
| `PUT` | `/products` | Update an existing product | JWT* |
| `DELETE` | `/products/{id}` | Remove a product | JWT* |

\* JWT authentication required only for **DemoApi-JWT** and **DemoApi-Docker** collections

---

## 🔍 Expected Status Codes

### Success
- `200 OK` - Successful request (GET, POST /auth/token)
- `201 Created` - Resource successfully created (POST /products)
- `204 No Content` - Successful operation with no response body (PUT, DELETE)

### Client Errors
- `400 Bad Request` - Invalid data or validation error
- `401 Unauthorized` - Invalid or missing token
- `404 Not Found` - Resource not found
- `412 Precondition Failed` - Business rule validation failure

---

## 🐳 Prerequisites for Docker Collection

Before using the **DemoApi-Docker** collection, ensure that:

1. **Docker Desktop** is installed and running
2. The application container is running:
   ```bash
   cd core/swagger-jwt-docker/docker
   docker-compose up -d
   ```
3. Verify the container is running:
   ```bash
   docker ps
   ```

---

## 🛠️ Troubleshooting

### Expired Token
If you receive a `401 Unauthorized` error, the JWT token may have expired. Solutions:
1. Execute the **Generate JWT Token** endpoint again
2. The token has a validity of **60 minutes** (configurable in appsettings)

### Connection Error
If you receive a connection error:
1. Verify the application is running on the correct port
2. For Docker, verify the container is running
3. Check if there's no firewall blocking the ports

### Empty {{token}} Variable
If the `{{token}}` variable is not automatically filled:
1. Verify the **Generate JWT Token** endpoint returned `200 OK`
2. Check the Postman **Console** tab to see script logs
3. Manually copy the token from the response and paste it in the collection variable

---

## 📚 Additional Documentation

For more information about the API architecture and implementation, see:
- [Main README](../README.md)
- [Swagger README](../core/swagger/README.md)
- [JWT README](../core/swagger-jwt/README.md)
- [Docker README](../core/swagger-jwt-docker/README.md)

---

## 📄 Collection Format

All collections were exported in **Postman Collection v2.1** format, ensuring compatibility with the latest Postman versions.
