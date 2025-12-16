# Henry's C# Web API Template

This is a template for building C# Web API projects using .NET. It provides a solid foundation with a clean architecture, essential features, and configurations to kickstart your development.

## Features

*   **Clean Architecture:** The template follows the principles of Clean Architecture, separating concerns into distinct layers: `Domain`, `Application`, `Infrastructure`, and `Presentation` (implicitly in the Web API project).
*   **ASP.NET Core:** Built on the latest version of ASP.NET Core, a powerful and modern framework for building web applications.
*   **.NET:** Utilizes the latest version of .NET for performance, security, and modern language features.
*   **Entity Framework Core:** Includes Entity Framework Core for data access, with a pre-configured `ApplicationDbContext`.
*   **Authentication & Authorization:** Comes with JWT-based authentication and authorization ready to be configured.
*   **Database:** Configured to use PostgreSQL by default, but can be easily changed to any other database supported by Entity Framework Core.
*   **Settings Management:** Uses `appsettings.json` and environment variables for configuration, with strongly-typed settings classes.
*   **Dependency Injection:** Makes extensive use of dependency injection for loose coupling and testability.
*   **OpenAPI (Scalar):** Integrated with Scalar for API documentation and testing.
*   **Hangfire:** Includes Hangfire for background job processing.
*   **Rate Limiting:** Pre-configured with rate limiting to protect your API from abuse.
*   **CORS:** CORS (Cross-Origin Resource Sharing) is configured to allow requests from your frontend applications.
*   **Testing:** Includes projects for unit, integration, and architecture tests.
*   **Dotnet Template:** Packaged as a `dotnet new` template for easy project creation.

## Getting Started

### Prerequisites

*   [.NET SDK](https://dotnet.microsoft.com/download)
*   [Git](https://git-scm.com/downloads/)

### Installation

1.  Clone the repository:
    ```bash
    git clone https://github.com/henrychris/henrycsharptemplate.git
    ```
2.  Navigate to the root directory:
    ```bash
    cd henrycsharptemplate
    ```
3.  Install the template:
    ```bash
    dotnet new install .
    ```

### Usage

1.  Create a new project from the template:
    ```bash
    dotnet new henrycsharptemplate -n YourProjectName
    ```
2.  Navigate to your new project's `src` directory:
    ```bash
    cd YourProjectName/src
    ```

## Configuration

The application is configured using an `.env` file.

1.  Create a `.env` file in the `src` directory by copying the `.env.example` file:
    ```bash
    cp .env.example .env
    ```
2.  Update the `.env` file with your configuration values for the database, authentication, and other settings.

## Running the Application

1.  Navigate to the `src` directory of your newly created project.
2.  Run the application:
    ```bash
    dotnet run
    ```
3.  The API will be available at the URLs specified in `src/Properties/launchSettings.json`. To access the Scalar UI, navigate to `http://localhost:5051/scalar/v1`.

## Running Tests

The template includes three test projects:

*   `tests/HenryCsharpTemplate.Unit.Tests`
*   `tests/HenryCsharpTemplate.Integration.Tests`
*   `tests/HenryCsharpTemplate.Architecture.Tests`

To run all tests, navigate to the root of your new project and run:

```bash
dotnet test
```

## Project Structure

The project is organized into the following layers:

*   **`src/Domain`**: Contains the core business logic, entities, and interfaces.
*   **`src/Application`**: Contains application-specific logic, services, and DTOs.
*   **`src/Infrastructure`**: Contains implementations of interfaces defined in the `Application` layer, such as data access, external services, and background jobs.
*   **`src/HenryCsharpTemplate` (Web API)**: The entry point of the application, responsible for handling HTTP requests and responses.
*   **`tests`**: Contains the test projects.

## Architecture Reference

- [How To Structure a Scalable .Net Application](https://dev.to/mashrulhaque/how-to-design-a-maintainable-net-solution-structure-for-growing-teams-284n)
- [Vertical Slice Architecture](https://www.milanjovanovic.tech/blog/vertical-slice-architecture)
- [Shared Logic in Vertical Slice Architecture](https://www.milanjovanovic.tech/blog/vertical-slice-architecture-where-does-the-shared-logic-live)