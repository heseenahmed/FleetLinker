---
description: How to run and test the FleetLinker API with localization
---

# Running and Testing FleetLinker API with Localization

## Build the Project
// turbo
```powershell
cd d:\FleetLinker
dotnet build
```

## Run the API
```powershell
cd d:\FleetLinker\FleetLinker
dotnet run
```

The API will start and show the URL in the console (e.g., `https://localhost:7xxx`).

## Open Swagger UI
Open your browser and navigate to:
```
https://localhost:7xxx/swagger
```
(Replace `7xxx` with the actual port shown in the console)

## Testing Localization

### Default Language (Arabic)
All API responses will be in Arabic by default. No special header needed.

### Switch to English
Add the `Accept-Language: en` header to your requests:

- **In Swagger UI**: Click on any endpoint, then add a custom header in the request.
- **In Postman**: Add header `Accept-Language` with value `en`.
- **In curl**:
```bash
curl -H "Accept-Language: en" https://localhost:7xxx/api/endpoint
```

### Test Endpoints

1. **Test Arabic (default)** - Login with invalid credentials:
   - POST `/api/Auth/login` with invalid username/password
   - Expected response: `{"msg": "المستخدم غير موجود."...}`

2. **Test English** - Same request with Accept-Language header:
   - POST `/api/Auth/login` with header `Accept-Language: en`
   - Expected response: `{"msg": "User not found."...}`

3. **Test Unauthorized (Arabic)**:
   - GET `/api/Roles/GetAllRoles` without authentication
   - Expected: `{"msg": "غير مصرح"...}`

4. **Test Unauthorized (English)**:
   - GET `/api/Roles/GetAllRoles` with header `Accept-Language: en`
   - Expected: `{"msg": "Unauthorized"...}`
