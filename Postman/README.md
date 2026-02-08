# 📬 Postman Collections - DemoApi

This directory contains ready-to-use Postman collections and environments for testing the Demo API in its different configurations.

## 📁 Available Collections

### 1. **DemoApi-Swagger.postman_collection.json**
Collection for the basic project with Swagger (no authentication).

- **Port:** `5084`
- **Base URL:** `http://localhost:5084/api/v1`
- **Authentication:** None
- **Endpoints:** Full Products CRUD

### 2. **DemoApi-JWT.postman_collection.json**
Collection for the project with JWT authentication. Works with both Local and Docker environments.

- **Environments:** 
  - **Local:** Port `5100`
  - **Docker:** Port `5200`
- **Authentication:** Bearer Token (JWT)
- **Security Key:** `b5b622cd-9f73-43b8-8dce-aab520cf1a2b`
- **Endpoints:** Authentication + Full Products CRUD

---

## 🌍 Available Environments

### 1. **DemoApi-Local.postman_environment.json**
Environment for local development testing.

- **Base URL:** `http://localhost:5100/api/v1`
- **Port:** `5100`
- **Use with:** `DemoApi-JWT` collection

### 2. **DemoApi-Docker.postman_environment.json**
Environment for Docker container testing.

- **Base URL:** `http://localhost:5200/api/v1`
- **Port:** `5200` (mapped to `8080` in container)
- **Use with:** `DemoApi-JWT` collection

---

## 🚀 How to Import Collections and Environments

### Step 1: Import the Collection

#### Method 1: Import via Postman Interface
1. Open Postman
2. Click **Import** (top left corner)
3. Select the **File** tab
4. Navigate to the `Postman` folder in the repository
5. Select the desired `.json` collection file
6. Click **Import**

#### Method 2: Drag and Drop
1. Open Postman
2. Drag the `.json` collection file to the Postman window
3. The collection will be imported automatically

### Step 2: Import the Environment

1. In Postman, click the **Environments** icon (left sidebar)
2. Click **Import**
3. Select the desired environment file:
   - `DemoApi-Local.postman_environment.json` for local testing
   - `DemoApi-Docker.postman_environment.json` for Docker testing
4. Click **Import**

### Step 3: Select the Environment

1. In the top-right corner of Postman, click the environment dropdown
2. Select either **DemoApi - Local** or **DemoApi - Docker**
3. The selected environment's variables will now be active

---

## 🔐 How to Use the JWT Collection

### For the **DemoApi-JWT** collection:

#### Step 1: Select Your Environment
1. Choose **DemoApi - Local** for local development (port 5100)
2. Choose **DemoApi - Docker** for Docker testing (port 5200)

#### Step 2: Generate JWT Token
1. Open the collection in Postman
2. Navigate to the **Authentication** folder
3. Execute the **Generate JWT Token** request
4. The token will be generated and **automatically saved** in the `{{token}}` environment variable

#### Step 3: Use Products Endpoints
1. Navigate to the **Products** folder
2. Execute any endpoint (Get All, Get By ID, Create, Update, Delete)
3. The token will be automatically included in the `Authorization: Bearer {{token}}` header

> **💡 Tip:** The post-request script in the `Generate JWT Token` endpoint automatically saves the token in the environment variable. You don't need to copy and paste it manually!

---

## 📝 Environment Variables

### DemoApi - Local
| Variable | Value |
|----------|-------|
| `baseUrl` | `http://localhost:5100/api/v1` |
| `token` | (automatically generated) |
| `securityKey` | `b5b622cd-9f73-43b8-8dce-aab520cf1a2b` |

### DemoApi - Docker
| Variable | Value |
|----------|-------|
| `baseUrl` | `http://localhost:5200/api/v1` |
| `token` | (automatically generated) |
| `securityKey` | `b5b622cd-9f73-43b8-8dce-aab520cf1a2b` |

### DemoApi - Swagger (No environment needed)
| Variable | Value |
|----------|-------|
| `baseUrl` | `http://localhost:5084/api/v1` (collection variable) |

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

### Authentication (JWT collection only)
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

\* JWT authentication required only for **DemoApi-JWT** collection

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

## 🐳 Prerequisites for Docker Environment

Before using the **DemoApi - Docker** environment, ensure that:

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

## 🔄 Switching Between Environments

To switch between Local and Docker:

1. Click the environment dropdown in the top-right corner of Postman
2. Select the desired environment:
   - **DemoApi - Local** for local development
   - **DemoApi - Docker** for Docker testing
3. All requests will now use the selected environment's `{{baseUrl}}`

> **💡 Best Practice:** You can have the same collection open in multiple tabs with different environments selected!

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
4. Ensure you have the correct environment selected

### Empty {{token}} Variable
If the `{{token}}` variable is not automatically filled:
1. Verify the **Generate JWT Token** endpoint returned `200 OK`
2. Check the Postman **Console** tab to see script logs
3. Ensure you have an environment selected (not "No Environment")
4. Manually copy the token from the response and paste it in the environment variable

### Wrong Port Being Used
If requests are going to the wrong port:
1. Check which environment is selected in the top-right dropdown
2. Verify the environment has the correct `baseUrl` value
3. Switch to the appropriate environment (Local or Docker)

---

## 📚 Additional Documentation

For more information about the API architecture and implementation, see:
- [Main README](../README.md)
- [Swagger README](../core/swagger/README.md)
- [JWT README](../core/swagger-jwt/README.md)
- [Docker README](../core/swagger-jwt-docker/README.md)

---

## 📄 Collection and Environment Format

All collections and environments were exported in **Postman Collection v2.1** and **Postman Environment** formats, ensuring compatibility with the latest Postman versions.
