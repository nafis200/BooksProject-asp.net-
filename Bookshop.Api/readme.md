cd backend

dotnet tool update --global dotnet-ef --version 8.0.*

dotnet ef migrations add InitialCreate --project Bookshop.Infrastructure --startup-project Bookshop.Api