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
---
Authentication & Authorization
•	Cookie Authentication is configured in Program.cs.
•	Login/Logout handled by AccountController.
•	Role-based redirects: After login, users are redirected to dashboards based on their role.
•	Logout: Must be triggered via a POST form (not a link) due to [HttpPost] on the action.
Example logout form:
  <form asp-controller="Account" asp-action="Logout" method="post" style="display:inline;">
      <button type="submit" class="btn btn-link">Logout</button>
  </form>
---
Key Components
Controllers
•	AccountController: Handles registration, login, and logout.
•	AdminDashboardController: Admin dashboard and management actions.
•	AgentDashboardController: Support agent dashboard.
•	HomeController: Public pages (Home, Privacy, etc.).
Services
•	AuthService: Handles authentication, registration, user approval, and audit logging.
Models
•	User, Role, Ticket, SupportTeam, AuditLog, KnowledgeBase, Feedback: Core entities for the support system.
Views
•	Account/Login.cshtml: Login page.
•	Account/Register.cshtml: Registration page.
•	AdminDashboard/Index.cshtml: Admin dashboard.
•	Shared/_Layout.cshtml: Main layout for all pages.
---
Troubleshooting
•	Login/Logout Issues: Ensure logout is triggered via a POST form. If you see a 404 or 405 error, check that the form is used instead of a link.
•	Database Connection Errors: Verify your connection string and that SQL Server is running.
•	Page Not Found (404): Ensure the controller and view names match exactly (case-sensitive on some systems).
•	Authentication Redirects: Confirm UseAuthentication() and UseAuthorization() are called in Program.cs.
---
Contributing
1.	Fork the repository.
2.	Create a feature branch (git checkout -b feature/your-feature).
3.	Commit your changes.
4.	Push to your branch.
5.	Open a pull request.
---
License
This project is licensed under the MIT License.
---
For any questions or issues, please open an issue on the repository or contact the maintainer.
---