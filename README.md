# CRUDExample

This is a simple ASP.NET Core application that demonstrates the implementation of CRUD (Create, Read, Update, Delete) operations with proper use of filters, dependency injection, and service configuration.

## Project Highlights

The project is organized into distinct folders for better readability and scalability:

- **Controllers**: Contains the logic for handling incoming HTTP requests. For example, `CountriesController.cs` and `PersonsController.cs` manage requests related to countries and persons respectively.
- **Filters**: Custom filters for implementing cross-cutting concerns like action filtering (`ActionFilters`), authorization filtering (`AuthorizationFilters`), result handling (`ResultFilters`), and exception handling (`ExceptionFilters`).
- **Entities**: Contains the database context (`ApplicationDbContext.cs`) and entity models like `Country.cs` and `Person.cs`.
- **RepoCon**: Defines interfaces like `ICountriesRepository` and `IPersonsRepository` for repository operations.
- **Repos**: Implements the repositories with classes like `CountriesRepo.cs` and `PersonsRepository.cs` to handle data access logic.
- **ServiceContracts**: Contains service interfaces such as `ICountriesService` and `IPersonsService`.
- **Services**: Implements business logic in classes like `CountriesService.cs` and `PersonsService.cs`.
- **StartupExtensions**: Includes the `ConfigureServicesExtension` class to centralize service registration.
- **CRUDTests**: Contains unit and integration tests for ensuring the correctness of the application. For example, `PersonsControllerIntegrationTest.cs` and `CountriesServiceTest.cs`.

## How to Run the Application

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/moameddtaha/crud-operations.git
   ```

2. **Set Up the Database**:
   - Ensure you have a SQL Server instance running.
   - Update the `DefaultConnection` string in `appsettings.json` with your database details.
   - Run the following command to apply migrations:
     ```bash
     dotnet ef database update
     ```

3. **Build and Run**:
   ```bash
   dotnet run
   ```

## Snippets of the Running Application
### Example:
![alt text](Pics/pic1.png)
![alt text](Pics/pic2.png)
![alt text](Pics/pic3.png)
![alt text](Pics/pic4.png)
![alt text](Pics/pic5.png)

## Technologies Used

- **ASP.NET Core**
- **Entity Framework Core**
- **SQL Server**
- **Dependency Injection**
- **Custom Filters**
- **HTTP Logging**
