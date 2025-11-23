# API
cd C:\Source\atomicmedia
dotnet restore
dotnet run --project src/server/AtomicTasks.Api/AtomicTasks.Api.csproj

#API LITE 
cd C:\Source\atomicmedia
dotnet run --project src/server/AtomicTasks.Api/AtomicTasks.Api.csproj

# FRONTEND 
nvm use 20.19.0
cd C:\Source\atomicmedia\src\client
npm install
$env:VITE_API_BASE_URL = "http://localhost:5286/api"
npm run dev

# FRONTEND LITE
cd C:\Source\atomicmedia\src\client
$env:VITE_API_BASE_URL = "http://localhost:5286/api"
npm run dev
