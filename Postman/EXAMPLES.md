# 📋 Response Examples - DemoApi

This file contains response examples for all endpoints in the collections.

---

## 🔐 Authentication Endpoints

### POST /auth/token

#### Request
```http
POST http://localhost:5100/api/v1/auth/token
X-Security-Key: b5b622cd-9f73-43b8-8dce-aab520cf1a2b
Content-Type: application/json
```

#### Response 200 OK
```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJEZW1vQXBpIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdCIsIm5iZiI6MTcwNzMxODAwMCwiZXhwIjoxNzA3MzIxNjAwfQ.example_signature_here",
    "tokenType": "Bearer",
    "expiresIn": 3600,
    "created": "2026-02-07T15:00:00Z",
    "expires": "2026-02-07T16:00:00Z"
  }
}
```

#### Response 401 Unauthorized (Missing Security Key)
```json
{
  "success": false,
  "errors": [
    "Security key is required. Please provide X-Security-Key header."
  ]
}
```

#### Response 401 Unauthorized (Invalid Security Key)
```json
{
  "success": false,
  "errors": [
    "Invalid security key. Authentication failed."
  ]
}
```

---

## 📦 Products Endpoints

### GET /products

#### Request
```http
GET http://localhost:5084/api/v1/products
Content-Type: application/json
Authorization: Bearer {token}  # Only for JWT and Docker
```

#### Response 200 OK (Empty List)
```json
{
  "success": true,
  "data": []
}
```

#### Response 200 OK (With Products)
```json
{
  "success": true,
  "data": [
    {
      "id": 1,
      "name": "Dell Inspiron Laptop",
      "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor",
      "price": 3499.99,
      "active": true
    },
    {
      "id": 2,
      "name": "Logitech MX Master 3 Mouse",
      "description": "Wireless ergonomic mouse for productivity",
      "price": 549.99,
      "active": true
    },
    {
      "id": 3,
      "name": "Keychron K2 Mechanical Keyboard",
      "description": "Wireless mechanical keyboard with Gateron Brown switches",
      "price": 899.99,
      "active": false
    }
  ]
}
```

---

### GET /products/{id}

#### Request
```http
GET http://localhost:5084/api/v1/products/1
Content-Type: application/json
Authorization: Bearer {token}  # Only for JWT and Docker
```

#### Response 200 OK
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Dell Inspiron Laptop",
    "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor",
    "price": 3499.99,
    "active": true
  }
}
```

#### Response 404 Not Found
```json
{
  "success": false,
  "errors": [
    "Product not found"
  ]
}
```

---

### POST /products

#### Request
```http
POST http://localhost:5084/api/v1/products
Content-Type: application/json
Authorization: Bearer {token}  # Only for JWT and Docker

{
  "id": 0,
  "name": "Dell Inspiron Laptop",
  "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor",
  "price": 3499.99,
  "active": true
}
```

#### Response 201 Created
```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Dell Inspiron Laptop",
    "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor",
    "price": 3499.99,
    "active": true
  }
}
```

#### Response 400 Bad Request (Validation)
```json
{
  "success": false,
  "errors": [
    "The Name field is required.",
    "The Price field must be greater than 0."
  ]
}
```

#### Response 412 Precondition Failed (Business Rule)
```json
{
  "success": false,
  "errors": [
    "A product with this name already exists."
  ]
}
```

---

### PUT /products

#### Request
```http
PUT http://localhost:5084/api/v1/products
Content-Type: application/json
Authorization: Bearer {token}  # Only for JWT and Docker

{
  "id": 1,
  "name": "Dell Inspiron Laptop - Updated",
  "description": "Dell Inspiron 15 Laptop with Intel Core i7 processor - 11th Gen",
  "price": 3299.99,
  "active": true
}
```

#### Response 204 No Content
```
(No response body)
```

#### Response 404 Not Found
```json
{
  "success": false,
  "errors": [
    "Product not found"
  ]
}
```

#### Response 400 Bad Request (Validation)
```json
{
  "success": false,
  "errors": [
    "The Name field is required.",
    "The Price field must be greater than 0."
  ]
}
```

---

### DELETE /products/{id}

#### Request
```http
DELETE http://localhost:5084/api/v1/products/1
Content-Type: application/json
Authorization: Bearer {token}  # Only for JWT and Docker
```

#### Response 204 No Content
```
(No response body)
```

#### Response 404 Not Found
```json
{
  "success": false,
  "errors": [
    "Product not found"
  ]
}
```

---

## 🔒 Authentication Errors (JWT and Docker)

### 401 Unauthorized (Missing Token)
```json
{
  "success": false,
  "errors": [
    "Unauthorized"
  ]
}
```

### 401 Unauthorized (Invalid Token)
```json
{
  "success": false,
  "errors": [
    "Unauthorized"
  ]
}
```

### 401 Unauthorized (Expired Token)
```json
{
  "success": false,
  "errors": [
    "Unauthorized"
  ]
}
```

---

## 📊 Standard Response Structure

### Success Response (with data)
```json
{
  "success": true,
  "data": {
    // Resource data
  }
}
```

### Success Response (list)
```json
{
  "success": true,
  "data": [
    // Array of resources
  ]
}
```

### Error Response
```json
{
  "success": false,
  "errors": [
    "Error message 1",
    "Error message 2"
  ]
}
```

### Response 204 No Content
```
(No response body - status code only)
```

---

## 🎯 Status Codes Used

| Status Code | Meaning | When It Occurs |
|-------------|---------|----------------|
| `200 OK` | Success | GET, POST /auth/token |
| `201 Created` | Resource created | POST /products |
| `204 No Content` | Success without body | PUT, DELETE |
| `400 Bad Request` | Invalid data | Validation failed |
| `401 Unauthorized` | Not authenticated | Missing/invalid token |
| `404 Not Found` | Resource not found | ID doesn't exist |
| `412 Precondition Failed` | Business rule | Business validation failed |

---

## 💡 Tips

### JWT Token
- The token has a validity of **60 minutes** (configurable)
- After expiration, generate a new token by executing `POST /auth/token`
- The post-request script automatically saves the token in the `{{token}}` variable

### Validations
- **Name**: Required, minimum 3 characters
- **Description**: Required, minimum 10 characters
- **Price**: Required, greater than 0
- **Active**: Required, boolean

### IDs
- IDs are automatically generated by the system
- When creating a product, use `"id": 0`
- When updating, use the product's real ID

---

**📚 For more information, see the complete [README.md](README.md).**
