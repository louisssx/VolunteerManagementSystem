# Volunteer Management System

This is a web-based application for managing volunteers, their tasks, and incident reports. It is built with ASP.NET Core and uses SQL Server for data storage.

## Prerequisites

Before you begin, ensure you have the following installed on your system:

*   [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
*   [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (Express, Developer, or any other edition)
*   A code editor like [Visual Studio 2022](https://visualstudio.microsoft.com/) or [Visual Studio Code](https://code.visualstudio.com/).
*   [Git](https://git-scm.com/downloads)

## Installation

Follow these steps to get a local copy up and running.

1.  **Clone the repository**

    ```sh
    git clone <your-repository-url>
    cd VolunteerManagementSystem
    ```
    *(Replace `<your-repository-url>` with the actual URL of your Git repository.)*

2.  **Configure the database connection**

    Open `appsettings.json` and update the `DefaultConnection` connection string with your SQL Server details.

    **Example for local SQL Server Express with Windows Authentication:**
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=.\\SQLEXPRESS;Database=VolunteerManagementDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
    }
    ```

    **Example with SQL Server Authentication:**
    ```json
    "ConnectionStrings": {
        "DefaultConnection": "Server=your_server_name;Database=VolunteerManagementDB;User Id=your_user;Password=your_password;TrustServerCertificate=True;MultipleActiveResultSets=true"
    }
    ```
    *Make sure the database `VolunteerManagementDB` exists on your server, or the user has permissions to create it.*

3.  **Apply Database Migrations**

    The application uses Entity Framework Core for database management. Run the following command in the root directory of the project to create the database schema:

    ```sh
    dotnet ef database update
    ```

4.  **Run the application**

    You can run the application using the .NET CLI:

    ```sh
    dotnet run
    ```

    Or by launching it from Visual Studio.

    The application will be available at `https://localhost:5001` or a similar address, which will be shown in the console.

## Deployment

To make the application accessible to end-users, it needs to be deployed to a web server.

1.  **Publish the Application**

    Run the `publish` command to package the application for deployment. This will compile the code and place the necessary files in a `publish` directory.

    ```sh
    dotnet publish --configuration Release
    ```

2.  **Host the Application**

    The contents of the `bin/Release/net8.0/publish/` directory can then be hosted on a web server. Common options include:
    *   **IIS (Internet Information Services)** on a Windows Server.
    *   **Azure App Service**
    *   **AWS Elastic Beanstalk**
    *   Using a reverse proxy like **Nginx** or **Apache** on a Linux server.

    The specific steps will depend on your chosen hosting environment.

## Usage

Once the application is running:

-   Navigate to the web application in your browser.
-   The system includes user registration and login functionalities handled by ASP.NET Core Identity.
-   You can register a new user to start using the application.

## Accessing the Application (for End-Users)

This is a web application. It is not installed on your personal computer or phone like a traditional desktop or mobile app.

To use the application, you simply need a web browser and the URL where the application is hosted. The system administrator who set up the application will provide you with this URL. 