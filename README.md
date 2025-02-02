# ManageBlockedCountriesAPI

## **Overview**
This API allows the management of blocked countries, temporal blocking, and IP-based country lookups. It integrates with third-party geolocation APIs to fetch country data and maintains in-memory storage for block operations.

---

## **Features & Endpoints**

![All Endpoint](https://github.com/user-attachments/assets/e1c01289-6ef9-461c-8b74-bb3f6a4f9389)

### 1. Add a Blocked Country
- **Endpoint:** `POST /api/countries/block`
- **Input:** JSON Body (string)
  ```json
  "EG"
  ```
- **Action:** Blocks the specified country by its code.
- **Validation:** Prevents duplicate entries.
  
![Add a Blocked Country](https://github.com/user-attachments/assets/24fb63df-8543-4229-ab68-558513f97a5e)


### 2. Delete a Blocked Country
- **Endpoint:** `DELETE /api/countries/block/{countryCode}`
- **Action:** Unblocks a specified country.
- **Error Handling:** Returns `404 Not Found` if the country is not blocked.
  
![Delete a Blocked Country](https://github.com/user-attachments/assets/9641366f-6d05-4964-8010-d528ec52cb2e)

### 3. Get All Blocked Countries
- **Endpoint:** `GET /api/countries/blocked`
- **Query Parameters:**
  - `page` (default: 1)
  - `pageSize` (default: 10)
  - `searchTerm` (optional)
- **Action:** Lists blocked countries with pagination and optional filtering.

![Get All Blocked Countries](https://github.com/user-attachments/assets/eb08eb24-0be7-4f71-9975-53bc0ba3667d)

### 4. Find My Country via IP Lookup
- **Endpoint:** `GET /api/ip/lookup?ipAddress={ip}`
- **Action:** Fetches country details for the provided IP. Uses the caller's IP if omitted.
- **Validation:** Ensures valid IP format.

![Find My Country via IP Lookup](https://github.com/user-attachments/assets/efe3630d-265f-4337-adee-dd5041af1094)

![Find My Country via IP Lookup](https://github.com/user-attachments/assets/e20c092b-d384-4ff9-ba9f-9fe5bb7b7cef)

### 5. Verify If IP is Blocked
- **Endpoint:** `GET /api/ip/check-block`
- **Action:**
  1. Fetches the caller's external IP.
  2. Fetches the country code using the third-party API.
  3. Checks if the country is in the blocked list.
  4. Logs the attempt.

![Verify If IP is Blocked](https://github.com/user-attachments/assets/17470672-1d93-4c54-ab55-1f6b6371dfe2)

### 6. Log Failed Blocked Attempts
- **Endpoint:** `GET /api/logs/blocked-attempts`
- **Query Parameters:**
  - `page` (default: 1)
  - `pageSize` (default: 10)
- **Action:** Returns a paginated list of blocked attempts.
- **Log Entry Details:**
  - IP address
  - Timestamp
  - Country code
  - Blocked status
  - User-Agent

![Log Failed Blocked Attempts](https://github.com/user-attachments/assets/2d257c0e-df76-49b6-8828-4f8c8e80b99c)

### 7. Temporarily Block a Country
- **Endpoint:** `POST /api/countries/temporal-block`
- **Request:**
  ```json
  {
    "countryCode": "US",
    "durationMinutes": 120
  }
  ```
- **Action:** Temporarily blocks a country for a specific duration.
- **Validation:**
  - Ensures duration is between 1 and 1440 minutes.
  - Rejects invalid country codes.
  - Prevents duplicate temporal blocks (returns `409 Conflict`).

![Temporarily Block a Country](https://github.com/user-attachments/assets/6b5e606d-c6ed-4ca9-a464-97132c444e54)

![Temporarily Block a Country](https://github.com/user-attachments/assets/efbebced-2f1b-4318-bb02-d5175464904d)

---

## **Setup Instructions**

### **Configuration**
Update the `appsettings.json` file:
```json
{
  "GeolocationApi": {
    "ApiKey": "your-api-key",
    "BaseUrl": "https://api.ipgeolocation.io",
    "countryCodeBaseUrl": "https://restcountries.com/v3.1/alpha"
  }
}
```

### **Running the API**
1. Clone the repository.
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Run the application:
   ```bash
   dotnet run
   ```
4. Open Swagger UI at `https://localhost:5001/swagger/index.html`.

---

## **Error Handling**
- `400 Bad Request`: Invalid input or missing parameters.
- `404 Not Found`: Resource not found.
- `409 Conflict`: Duplicate temporal block attempts.
- `500 Internal Server Error`: Third-party API failures.

---

