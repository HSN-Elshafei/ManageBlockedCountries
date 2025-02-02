# ManageBlockedCountriesAPI

## **Overview**
This API allows the management of blocked countries, temporal blocking, and IP-based country lookups. It integrates with third-party geolocation APIs to fetch country data and maintains in-memory storage for block operations.

---

## **Features & Endpoints**

![All Endpoint]("C:\Users\HSN\Desktop\AllEndPoint.png")

### 1. Add a Blocked Country
- **Endpoint:** `POST /api/countries/block`
- **Input:** JSON Body (string)
  ```json
  "EG"
  ```
- **Action:** Blocks the specified country by its code.
- **Validation:** Prevents duplicate entries.

### 2. Delete a Blocked Country
- **Endpoint:** `DELETE /api/countries/block/{countryCode}`
- **Action:** Unblocks a specified country.
- **Error Handling:** Returns `404 Not Found` if the country is not blocked.

### 3. Get All Blocked Countries
- **Endpoint:** `GET /api/countries/blocked`
- **Query Parameters:**
  - `page` (default: 1)
  - `pageSize` (default: 10)
  - `searchTerm` (optional)
- **Action:** Lists blocked countries with pagination and optional filtering.

### 4. Find My Country via IP Lookup
- **Endpoint:** `GET /api/ip/lookup?ipAddress={ip}`
- **Action:** Fetches country details for the provided IP. Uses the caller's IP if omitted.
- **Validation:** Ensures valid IP format.

### 5. Verify If IP is Blocked
- **Endpoint:** `GET /api/ip/check-block`
- **Action:**
  1. Fetches the caller's external IP.
  2. Fetches the country code using the third-party API.
  3. Checks if the country is in the blocked list.
  4. Logs the attempt.

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

---

## **Setup Instructions**

### **Prerequisites**
- .NET 7 or higher installed
- API key from a third-party geolocation service (e.g., IPGeolocation.io)

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

## **Testing Instructions**

### **Unit Tests**
Run the unit tests using:
```bash
 dotnet test
```

### **Manual Testing via Swagger**
1. Navigate to `https://localhost:5001/swagger/index.html`.
2. Use the available endpoints to test functionality.

---

## **Error Handling**
- `400 Bad Request`: Invalid input or missing parameters.
- `404 Not Found`: Resource not found.
- `409 Conflict`: Duplicate temporal block attempts.
- `500 Internal Server Error`: Third-party API failures.

---

