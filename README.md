# IdentityJWT
.NET Core 3.1 using JWT and RefreshTokens for authentication

## Notes
- Used separate contexts so that I can apply EF Migrations to the IdentityContext while using Database Project for the AppDataContext
- initial Identity user/role data seed in a call to `UserAndRoleDataInitializer.SeedData()` from `Program.cs` along with related roles for testing role based authorizations in various WebAPI calls
- Added Swagger API documentation

## Known Issues
- Work-in-progress...  roles don't seem to work correctly.  I was trying to use Sqlite for ease of demonstration, but there seems to currently be several things broken.  
- The Client application is still logging out after a period of time.  The RefreshToken appears to be working, but there's a bug that is causing a logoff that I haven't looked into yet.
    - Was mostly working correctly on my personal application that I refactored to build this demo, but I need to clean up the bugs... may be related to trying to use Sqlite DB...
- The "Remember Me" on login is not currently wired up

## Setup
### These setup instructions are written based on Visual Studio 2019 (Windows 10)
1. Clone/download project
1. Run Create Datebase Migration for IdentityContext to build the identity database tables
    - `add-migration initial`
    - `update-database`
1. Run project... you should see the JWT/Identity API swagger API documentation screen pop up

## Auto-seeded login user accounts
| Username          | Password   |
|-------------------|------------|
| appuser@test.com  | P@ssw0rd1! |
| appadmin@test.com | P@ssw0rd1! |

