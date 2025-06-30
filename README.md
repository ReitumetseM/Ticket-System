OmnitakSupportHub
OmnitakSupportHub is a modern IT support ticketing and knowledge base web application built with ASP.NET Core (.NET 9), 
Entity Framework Core, and SQL Server. It supports role-based authentication, ticket management, team collaboration, and
an extensible knowledge base, designed for enterprise IT support environments.
---
Table of Contents
•	#features
•	#project-structure
•	#technology-stack
•	#getting-started
•	#prerequisites
•	#configuration
•	#database-setup
•	#running-the-application
•	#authentication--authorization
•	#key-components
•	#troubleshooting
•	#contributing
•	#license
---
Features
•	User Registration & Login: Secure registration and login with email and password.
•	Role-Based Access: Administrator, Support Manager, Support Agent, and User roles with granular permissions.
•	Ticket Management: Create, assign, and track support tickets.
•	Team Management: Organize users into support teams.
•	Knowledge Base: Create and manage articles for self-service support.
•	Audit Logging: Track key actions for security and compliance.
•	Responsive UI: Built with Bootstrap for modern, mobile-friendly design.
---
Project Structure
OmnitakSupportHub/
?
??? Controllers/                # MVC Controllers (Account, Home, AdminDashboard, AgentDashboard, etc.)
??? Models/                     # Entity and ViewModel classes (User, Role, Ticket, etc.)
??? Services/                   # Business logic and data access (AuthService, IAuthService)
??? Views/
?   ??? Shared/                 # Shared layout and partials (_Layout.cshtml)
?   ??? Account/                # Login, Register views
?   ??? AdminDashboard/         # Admin dashboard view
?   ??? ...                     # Other feature views
??? wwwroot/                    # Static files (CSS, JS, images)
??? appsettings.json            # Application configuration
??? Program.cs                  # Application startup and configuration
??? README.md                   # Project documentation
---
Technology Stack
•	.NET 9 (ASP.NET Core MVC)
•	Entity Framework Core (SQL Server)
•	Bootstrap 5 (UI)
•	Microsoft Identity / Cookie Authentication
•	C# 13
---
Getting Started
Prerequisites
•	.NET 9 SDK
•	SQL Server (or LocalDB)
•	Visual Studio 2022 (recommended)
Configuration
1.	Clone the repository:
   git clone https://github.com/your-org/OmnitakSupportHub.git
   cd OmnitakSupportHub
2.	Configure the database connection:
Edit appsettings.json:
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OmnitakITSupportDB;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
•	Change Server and Database as needed for your environment.
Database Setup
1.	Apply Migrations and Create Database:
Open the terminal in the project directory and run:
   dotnet ef database update
This will create the database and apply all migrations.
Running the Application
1.	Build and Run:
   dotnet run
2.	Access the App:
Open your browser and navigate to https://localhost:7080 (or the port shown in your console).
