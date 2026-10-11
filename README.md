# HoopLog

HoopLog is a web app for logging basketball training. Players record their training sessions and drill results. Coaches create teams, build a drill library for each team, and follow how their players are doing.

## Features

- **Accounts:** register with a name, email and password, then log in and out. Your name is shown in the menu and on team rosters instead of your email. After 5 wrong passwords the account is locked for 5 minutes.
- **Teams with join codes:** create a team and share its 8-character code. Others join with the code. A coach can generate a new code with **Rotate code** (the old code stops working) and remove members.
- **Per-team roles:** the user who creates a team is its Coach, and everyone who joins is a Player. One person can be a Coach in one team and a Player in another.
- **Team drill library:** coaches add and edit drills for the teams they coach. Each drill has a category and a metric type. The metric type cannot be changed once results exist. Drills are archived instead of deleted, so old results stay visible.
- **Sessions and drill results:** log a session (date, duration, location, notes), then add results for your teams' drills. Results are recorded as makes/attempts, a count (reps) or a duration (seconds). Sessions and results can be edited and deleted.
- **Coach progress view:** a coach sees the drill results of the team's current players: drill, date, result and the player's name. Notes and locations stay private.

## Technologies

- ASP.NET Core 8 MVC (.NET 8)
- Entity Framework Core 8.0.31 with SQL Server / LocalDB
- ASP.NET Core Identity 8.0.31 (custom `AccountController`, no Identity UI)
- Bootstrap 5.1
- jQuery 3.6 and jQuery Validation 1.19.5 with the unobtrusive adapter 4.0
- DotNetEnv 3.2.0 (optional `.env` file for configuration)

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server LocalDB, which is installed with Visual Studio's "ASP.NET and web development" workload, or any SQL Server instance
- The EF Core command-line tool:

  ```
  dotnet tool install --global dotnet-ef
  ```

## Setup

No configuration is needed. The default connection string in `appsettings.json` uses LocalDB (`(localdb)\MSSQLLocalDB`, database `HoopLog`).

1. Clone the repository:

   ```
   git clone https://github.com/Ra4i/HoopLog.git
   ```

2. Go into the project folder (it contains `HoopLog.csproj`):

   ```
   cd HoopLog
   ```

3. Build the project. This also restores the NuGet packages, which `dotnet ef` needs on a fresh clone:

   ```
   dotnet build
   ```

4. Create the database:

   ```
   dotnet ef database update
   ```

5. Start the app:

   ```
   dotnet run
   ```

   `dotnet run` uses the `http` launch profile. Open **http://localhost:5161** in your browser.

   In Visual Studio, open `HoopLog.csproj` and press **F5**. To use HTTPS instead, run `dotnet run --launch-profile https` and open **https://localhost:7190**. If the browser warns about the certificate, run `dotnet dev-certs https --trust` once.

## Configuration (optional)

To use a different SQL Server, copy `.env.example` to `.env` in the project folder and set `ConnectionStrings__DefaultConnection`. `.env.example` starts with the same LocalDB value as `appsettings.json`; replace it with your own, for example:

```
ConnectionStrings__DefaultConnection=Server=YOUR_SERVER;Database=HoopLog;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True
```

The app loads `.env` at startup as environment variables, and environment variables override `appsettings.json`. `.env` is listed in `.gitignore`, so it is never committed. Run `dotnet ef database update` again after changing the connection string.

## Getting started

1. Click **Register** and create an account.
2. Go to **Teams**, then **Create team**. You become the team's coach, and the team page shows its join code.
3. A **Drills** link now appears in the menu. Click **Add Team Drill**, choose your team, and pick a metric type.
4. In a private window or another browser, register a second account. Go to **Teams**, then **Join with code**, and enter the code.
5. As the second account, go to **Sessions**, then **Add New Session**, and fill in the date and **Duration (minutes)**. Open the session with its 👁️ (**View Details**) button and use **Add Drill Result** to log a result for your team's drill.
6. Back as the coach, go to **Teams**, click **Open** next to the team, then click **Player progress** to see the result.

## Project structure

| Folder | Contents |
|---|---|
| `Controllers` | MVC controllers: Account, Home, Teams, Drills, Sessions |
| `Services` | Business logic and data access behind interfaces (`ITeamService`, `IDrillService`, `ISessionService`) |
| `Data/Models` | EF Core entities (User, Team, TeamMembership, Drill, DrillResult, TrainingSession) and enums |
| `ViewModels` | Input models with validation attributes, view models for pages, and small display helpers (`DrillResultFormatter`, `EnumDisplayExtensions`) |
| `Views` | Razor views, the shared layout and partial views |
| `Authorization` | The "Coach" policy requirement and its handler |
| `Migrations` | EF Core migrations for the database schema |
| `wwwroot` | Static files: CSS, JavaScript, and the Bootstrap and jQuery libraries |

## Authorization model

- **Roles belong to a team, not to a user.** `TeamMembership.Role` is `Coach` or `Player` for one specific team. There are no global roles, and no roles are stored in the login cookie. Permissions are read from the database on every request, so removing a member takes effect right away.
- **All data access is scoped inside the service queries.** Sessions and results can only be read or changed by their owner. Drills can be logged only by members of the drill's team and managed only by its coaches. The progress page shows only the team's current players.
- **Missing and forbidden look the same.** If a record does not exist or belongs to someone else, the app returns 404, so it never confirms that another user's data exists.
- The **Drills** page needs the "Coach" policy (you coach at least one team). The **Drills** link is shown only to coaches, and other users who open the page are sent to an Access denied page.

## Known limitations

- No email confirmation, password reset or account settings page.
- A team has exactly one coach (its creator). There is no way to add a second coach or transfer the team.
- Players cannot leave a team themselves; the coach removes them. Teams cannot be deleted.
- Archived drills cannot be restored.
- Login shows the same message for an unknown email and a wrong password, but the response time can differ slightly. This is accepted because registration already shows whether an email is taken.
