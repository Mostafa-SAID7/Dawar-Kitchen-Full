# Contributing to Naar & Noor

First off, thank you for considering contributing to Naar & Noor! It's people like you that make this project great.

## Development Setup

1. **Clone the repository:**
   ```bash
   git clone https://github.com/Mostafa-SAID7/Naar-Noor-Full.git
   cd Naar-Noor-Full
   ```

2. **Frontend Setup (Angular):**
   ```bash
   cd naar-noor
   npm install
   npm run start
   ```

3. **Backend Setup (.NET):**
   ```bash
   cd api-server
   dotnet restore NaarNoor.sln
   dotnet run --project src/NaarNoor.API/NaarNoor.API.csproj
   ```

## Pull Request Process

1. Ensure any install or build dependencies are removed before the end of the layer when doing a build.
2. Update the README.md with details of changes to the interface, this includes new environment variables, exposed ports, useful file locations and container parameters.
3. You may merge the Pull Request in once you have the sign-off of two other developers, or if you do not have permission to do that, you may request the second reviewer to merge it for you.

## Code Style

- Frontend: Follow standard Angular style guides and Prettier configurations.
- Backend: Follow standard C# and .NET coding conventions.

## Running Tests

- Frontend tests: `npm run test` inside `naar-noor` directory.
- Backend tests: `dotnet test` inside `api-server` directory.

We look forward to your contributions!
