# BiteDance

## Setup

- Set server timezone to GMT+7
- Set super admin email in code `BiteDanceAPI/src/Domain/Constants/AuthorizationConst.cs`
- Seed department/department charge code in `BiteDanceAPI/src/Infrastructure/Data/ApplicationDbContextInitialiser.cs`
- App config
  - `BiteDanceAPI/src/Domain/Constants`
  - Id, **Email**, Name claims config for SSO `BiteDanceAPI/src/Web/Services/CurrentUser.cs`
  - Database, SSO, Azure Communication Service, CORS: `BiteDanceAPI/src/Web/appsettings.json`
- To run backend, check `BiteDanceAPI/README.md`
- To run frontend, check `BiteDanceWeb/README.md`
  - `npm install`
  - `npm run dev` for development
  - `npm run build` for production. Build folder will be in `BiteDanceWeb/dist`. Use a static file server (IIS, nginx, etc.) to host the frontend.

## Dev note:

- Instructions to generate typescript client, database migration in `BiteDanceAPI/README.md`
