# 💊 PharmaVault

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Blazor](https://img.shields.io/badge/Blazor-Interactive_Server-5C2D91?style=for-the-badge&logo=blazor&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-316192?style=for-the-badge&logo=postgresql&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap_5-7952B3?style=for-the-badge&logo=bootstrap&logoColor=white)

PharmaVault is a robust, web-based personal inventory management system designed to track medication stock, control expiration dates, and maintain a centralized catalog of pharmaceutical products. 

## 🎯 Project Purpose
Managing personal or family medical supplies often leads to expired medications or unnecessary duplicate purchases. PharmaVault solves this by providing a unified dashboard where users can monitor their inventory health, receive visual alerts for medicines expiring soon, and manage a master catalog of drugs with their respective pharmaceutical forms and dosages.

## 🏗️ Architecture
This project implements a **Clean Architecture / N-Tier** approach to ensure a strict separation of concerns, maintainability, and scalability:
* **PharmaVault.Core:** Contains the domain models, Data Transfer Objects (DTOs), and interfaces. It represents the heart of the business logic with zero external dependencies.
* **PharmaVault.Data:** The infrastructure layer responsible for data persistence. It implements the Repository Pattern (DAOs) communicating directly with PostgreSQL.
* **PharmaVault.Web:** The presentation layer built with ASP.NET Core Blazor, handling the UI, user interactions, and state management.

## 💻 Tech Stack & Rationale
* **Frontend/Backend:** **Blazor Web App (Interactive Server)**. Chosen for its ability to build rich, interactive web UIs using C# instead of JavaScript, sharing models between the client and server seamlessly.
* **Database:** **PostgreSQL**. Selected for its reliability, data integrity, and excellent support for date/time operations.
* **Data Access:** **ADO.NET with Custom Extensions / Dapper-like approach**. Opted for raw SQL and lightweight mapping to ensure maximum execution speed and optimized queries over heavy ORMs.
* **UI/UX:** **Bootstrap 5** for a responsive, mobile-first layout, and **ApexCharts** for rendering interactive, real-time analytical dashboards.

## ✨ Key Features
* **Authentication & Authorization:** Secure user sessions.
* **Master Catalog Management:** Centralized repository for drug names, dosages, and pharmaceutical forms.
* **Smart Inventory CRUD:** Add, edit, and delete stock linked to specific users.
* **Real-time Search:** Client-side filtering using LINQ for zero-latency searches.
* **Analytics Dashboard:** Visual representation of inventory health (Total, Good Condition, Expiring Soon, Expired).

---

# 🚀 Getting Started (Local Development)

## 1. Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download) or higher.
* PostgreSQL server running locally or via Docker.
* An IDE like Visual Studio, VS Code, or JetBrains Rider.

## 2. Database Setup
Execute the SQL scripts found in the `/docs/sql` or `Database` folder to generate the schema for `medicine_catalog` and `inventory` tables.

## 3. Connection String Setup (User Secrets)
For security reasons, the database connection string is not tracked in source control. You must configure it locally using the .NET Secret Manager.

Open your terminal, navigate to the Web project folder (`src/PharmaVault.Web`), and run the following commands:

```bash
# Initialize user secrets for the project
dotnet user-secrets init

# Set your PostgreSQL connection string
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;Database=PharmaVaultDb;Username=your_user;Password=your_password"
```
## 4. Run the Application
```bash
# Build the project
dotnet build

# Run the application
dotnet run --project src/PharmaVault.Web

# Or run with Hot Reload enabled
dotnet watch --project src/PharmaVault.Web (Hot Reload)
```

## 🗺️ Roadmap / Future Enhancements
* [ ] Implement medicine consumption logging (subtract stock).
* [ ] Export inventory reports to Excel/PDF.
* [ ] Email notifications for expiring medications.

# 📄 License
This project is licensed under the Apache-2.0 license - see the LICENSE file for details.

# 🌍 Production Deployment (Ubuntu + Docker + Nginx)
This project is optimized for deployment on a Linux server using Docker for the application runtime and a native PostgreSQL installation.

## 1. Clone the Repository on your Server
SSH into your production server and clone the source code:
```bash
git clone https://github.com/JReyna217/PharmaVault.git
cd PharmaVault
```
## 2. Build the Docker Image Locally
To ensure the image is compiled for your server's specific CPU architecture (e.g., avoiding ARM vs x86_64 conflicts), build the image directly on the host:
```bash
docker build -t pharmavault-app:latest .
```
## 3. Run the Container
Run the container in detached mode. We map the internal Blazor port (8080) to a local server port (e.g., 5010).

_Note: The --add-host flag allows the isolated Docker container to communicate with the PostgreSQL database if is installed natively on the host machine._

```bash
docker run -d \
  --name pharmavault-web \
  --restart unless-stopped \
  -p 5010:8080 \
  --add-host=host.docker.internal:host-gateway \
  -e "ConnectionStrings__DefaultConnection=Host=host.docker.internal;Database=PharmaVaultDb;Username=YOUR_DB_USER;Password=YOUR_DB_PASSWORD" \
  pharmavault-app:latest
```
## 4. Configure Nginx as a Reverse Proxy
To expose the application to the internet securely, use Nginx to proxy the traffic to port 5010.

A template for the Nginx configuration can be found in the repository under /infrastructure/nginx/pharmavault.conf. This configuration includes the necessary headers to support Blazor Server's WebSockets (SignalR) and enforces HTTPS redirection.

Enable the site and reload Nginx:
```bash
sudo ln -s /etc/nginx/sites-available/pharmavault /etc/nginx/sites-enabled/
sudo nginx -t
sudo systemctl reload nginx
```
