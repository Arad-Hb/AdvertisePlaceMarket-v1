# Migrations

The downloadable project defaults to `Database:InitializeWithEnsureCreated = true` so it can create a development database immediately on first run.

For the normal migration workflow:

1. Stop the API and delete the development database that was created with EnsureCreated.
2. Install the matching EF tool if needed:
   `dotnet tool install --global dotnet-ef --version 10.0.10`
3. Create the first migration from the solution root:
   `dotnet ef migrations add InitialCreate --project DomainModel --startup-project Api`
4. Change `Database:InitializeWithEnsureCreated` to `false`.
5. Run:
   `dotnet ef database update --project DomainModel --startup-project Api`
