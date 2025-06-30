OmnitakSupportHub
OmnitakSupportHub is a robust IT support ticketing and knowledge base application built with ASP.NET Core (.NET 9), Entity Framework Core, and SQL Server. Designed for enterprise environments, it empowers support teams with efficient ticketing workflows, role-based access control, and an extensible knowledge base.

📋 Features
Secure Authentication: Email/password login secured with cookie-based authentication.

Role-Based Access Control: Fine-grained permissions for Admins, Support Managers, Agents, and Users.

Ticket Management: Create, assign, escalate, and track support tickets with full history.

Team Collaboration: Assign users to support teams and streamline response management.

Knowledge Base: Author and publish support articles for self-service.

Audit Logging: Record key actions for compliance and traceability.

Responsive UI: Built with Bootstrap 5 for a modern, mobile-friendly interface.

🧱 Project Structure
plaintext
OmnitakSupportHub/
├── Controllers/           // MVC controllers (Account, Home, AdminDashboard, etc.)
├── Models/                // Core entities and ViewModels (User, Role, Ticket, etc.)
├── Services/              // Business logic (e.g., AuthService)
├── Views/                 // Razor Views
│   ├── Shared/            // Layouts and partials (_Layout.cshtml)
│   ├── Account/           // Login, registration, etc.
│   ├── AdminDashboard/    // Admin views
├── wwwroot/               // Static assets (CSS, JS, images)
├── appsettings.json       // Configuration settings
├── Program.cs             // Application entry point
├── README.md              // Project documentation
⚙️ Technology Stack
.NET 9 / ASP.NET Core MVC

Entity Framework Core (SQL Server)

C# 13

Bootstrap 5

Microsoft Identity (Cookie Authentication)

🚀 Getting Started
Prerequisites
.NET 9 SDK

SQL Server or LocalDB

Visual Studio 2022+ (recommended)

Configuration
Clone the repository:

bash
git clone https://github.com/your-org/OmnitakSupportHub.git
cd OmnitakSupportHub
Edit appsettings.json:

json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=OmnitakITSupportDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
Database Setup
Apply migrations and create the database:

bash
dotnet ef database update
Run the Application
bash
dotnet run
Then navigate to: https://localhost:7080

🔐 Authentication & Authorization
Configured via Program.cs using cookie authentication

Role-based redirection upon login

Logout requires POST form submission (not links) due to [HttpPost] constraint:

html
<form asp-controller="Account" asp-action="Logout" method="post">
    <button type="submit" class="btn btn-link">Logout</button>
</form>
🧩 Key Components
Controllers
AccountController – login, logout, registration

AdminDashboardController – admin interface

AgentDashboardController – agent workflow

HomeController – public pages

Services
AuthService – registration, login, audit logging

Models
User, Role, Ticket, SupportTeam, AuditLog, KnowledgeBase, Feedback

🛠️ Troubleshooting
Login issues: Ensure logout uses a POST form.

DB connection errors: Check connection string and SQL Server availability.

404s: Confirm controller/view naming conventions (case-sensitive on some platforms).

Authentication issues: Ensure UseAuthentication() and UseAuthorization() are correctly called in Program.cs.

🤝 Contributing
Fork the repository.

Create a branch: git checkout -b feature/my-feature

Commit your changes.

Push and open a Pull Request.

📄 License
This project is licensed under the MIT License.