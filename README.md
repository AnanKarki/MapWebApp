# MapWebApp

---
## 🛠️ Prerequisites

Make sure you have the following installed:

- [Node.js](https://nodejs.org/) (v16 or higher)
- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) or later
- [Git](https://git-scm.com/)

---
## 🚀 Step-by-Step Setup
### 1. Clone the Repository

## Bash Script
git clone <your-repo-url>
cd project-root


### 2. Setup and Run the .NET Core Backend
cd server
dotnet restore
dotnet build
dotnet run


### 3. Setup the React Client App
cd ../client
npm install


### 4. setup the env file
VITE_API_BASE_URL={API_URL}
VITE_CLIENT_ID={AZURE_MAP__CLIENT_ID}
VITE_SUBSCRIPTION_KEY={AZURE_MAP_SUBSCRIPTION_KEY}
