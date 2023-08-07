
# Leave A Note App - Backend

Welcome to the backend repository of the Leave A Note app.

 This repository contains the ASP.NET Core project with Entity Framework that serves as the backend for [Leave a Note App](https://github.com/korenkaplan/Leave-A-Note). This README will provide you with essential information to understand and work with the backend portion of the application.

 


## Table of Content
 - [Getting Started](#getting-started)
 - [Project Structure](#project-structure)
 - [Database](#database)
 - [Authentication and Authorization](#authentication-and-authorization)
 - [Environments and Data Security](#environments-and-data-security)
 - [API Reference](#api-reference)
 - [Author and Feedback](#author-and-feedback)
## Getting Started
### Prerequisites
Before you begin, make sure you have the following prerequisites installed:
- Visual Studio Minimum version -> 17.4
- .NET SDK 7.0
- SQL Express
### Installation
1. Clone this repository to your local machine.
2. Create a new  SQL database and copy the connection string.
3. Open the solution in visual studio and run the following command in the  Package Manager Console:
```bash
dotnet restore
```


### Initializing User Secrets
1. **Open the Package Manager Console an run this command:**
```bash
cd ./LeaveANoteServerProject
```
2. **Add the following secrets to the user's secrets by typing this command into the Package Manager Console:**
```bash
dotnet user-secrets set "Your example Key" "Your example value".
```
- **JWT Key:**
```bash
dotnet user-secrets set "JWTKEY" "Your secret key"
```
- **Connection string:**
```bash
dotnet user-secrets set "ConnectionsStrings:defaultConnection" "Your sql connection string"
```

3. **You can view your new secrets using this command:**
```bash
dotnet user-secrets list
```
### Migrations and Local Run
1. Migrate with entity framework cli to create the database with the following command in the console:
```bash
dotnet ef database update
```
2. Now You can go to the database and see your tables.
3. Now you can run the project and test the endpoints start with register a new user and then try to login.


## Project Structure
- **Controllers:** This folder contains the API controllers that handle incoming HTTP requests and produce appropriate responses. 
- **Models:** The models folder holds the data models that represent the entities and objects used within the application.
- **Data:** This folder holds the data context class.
- **Services:** In the services folder, you'll find the business logic and services that encapsulate various operations within the application. 
- **Utils:** This folder contains the Token class.
- **DTO's:** DTOs are used to define the shape of data that is transferred between different parts of the application, you'll find a subfolder of dto's for each entity. 
- **Migrations:** he migrations folder is often associated with database management. In the context of Entity Framework, migrations are scripts that represent changes to the database schema over time. They are used to apply changes to the database structure, such as creating or altering tables, columns, and relationships
## Database

### Users Table
- Each row represents a user of the application.
- Users can have multiple accidents associated with them.
- The relationship is one-to-many from Users to Accidents.
- '*' = Uniqe Index

| Column           | Type       | Description                                 |
|------------------|------------|---------------------------------------------|
| Id               | int        | Primary key                                 |
| Name             | nvarchar   | User's name                                 |
| Email            | nvarchar   | User's email*                               |
| Password         | nvarchar   | User's password                             |
| CarNumber        | nvarchar   | User's car number*                          |
| PhoneNumber      | nvarchar   | User's phone number*                        |
| Role             | nvarchar   | User's role                                 |
| DeviceToken      | nvarchar   | User's device token                         |
| CreatedAt        | datetime   | Timestamp when user was created             |

### Accidents Table
- Each row represents an accident report or note.
- An accident is associated with the damaged user.
- Each accidentis can be associated with zero or one unmatched report.
- The relationship is many-to-one from Accidents to Users.
- The relationship is one-to-one from Accidents to UnmatchedReports.

| Column                 | Type       | Description                                 |
|------------------------|------------|---------------------------------------------|
| Id                     | int        | Primary key                                 |
| HittingDriverName      | nvarchar   | Name of the hitting driver in the accident  |
| HittingCarNumber       | nvarchar   | Car number of the hitting vehicle           |
| HittingDriverPhoneNumber | nvarchar  | Phone number of the hitting driver          |
| ReporterName           | nvarchar   | Name of the reporter                        |
| ReporterPhoneNumber    | nvarchar   | Phone number of the reporter                |
| ImageSource           | nvarchar   | Image source of the accident                |
| Type                   | nvarchar   | Type of accident                            |
| IsAnonymous            | bit        | Flag indicating if the report is anonymous |
| IsIdentify             | bit        | Flag indicating if the hitting driver is identified |
| IsDeleted              | bit        | Flag indicating if the accident is deleted  |
| IsRead                 | bit        | Flag indicating if the accident is read     |
| Date                   | nvarchar   | Date of the accident                        |
| createdAt              | datetime  | Timestamp when accident was created         |
| UserId                 | int (FK)   | Foreign key referencing Users table         |

### UnmatchedReports Table
- Each row represents an unmatched report related to an accident.
- Each unmatched report is associated with one accident.
- The relationship is one-to-one from UnmatchedReports to Accidents.

| Column                 | Type       | Description                                 |
|------------------------|------------|---------------------------------------------|
| Id                     | int        | Primary key                                 |
| DamagedCarNumber       | nvarchar   | Car number of the damaged vehicle           |
| AccidentId             | int (FK)   | Foreign key referencing Accidents table     |
| CreatedAt              | datetime  | Timestamp when unmatched report was created |





## Authentication and Authorization

### JWT Bearer Token
I've implemented authentication using JWT (JSON Web Tokens) Bearer tokens. When a user successfully logs in, they receive a JWT token that is included in the header of subsequent API requests. This token is used to verify the user's identity and permissions for each request.

### Roles
Upon registration, each user is automatically assigned the "User" role. Roles provide a way to group users with similar permissions. In the application, there are two primary roles: "User" and "Admin".

### Role-Based Authorization
Role-based authorization is enforced to control access to specific parts of the application. Here's how it works:

- **User Role:**
  - Users with the "User" role can access most of the application's endpoints.
  - They can create accident reports, view their own reports, and perform other user-specific actions.

- **Admin Role:**
  - Users with the "Admin" role have elevated privileges.
  - The "stats" controller, which contains administrative statistics , is accessible only to users with the "Admin" role.
  - This is achieved using `[Authorize(Roles = "Admin")]` on the relevant controller or action methods.

### Restricting Access to the "stats" Controller
To restrict access to the "stats" controller for administrators only, Iv'e used the `[Authorize(Roles = "Admin")]` attribute on the relevant controller or specific action methods. This ensures that only users with the "Admin" role can access these endpoints.

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")] // Restrict access to users with the "Admin" role
public class StatsController : ControllerBase
{
    // stats controller endpoints..
}
```
## Environments and Data Security

### Managed Identity and Azure Key Vault
To enhance security and manage sensitive configuration information such as connection strings and JWT keys, we leverage Azure Managed Identity and Azure Key Vault.

- **Managed Identity:** The application, when running in an Azure App Service, uses Managed Identity to authenticate itself to Azure services. This means that the application's code doesn't need to manage or store credentials. Instead, it can use the identity to securely access Azure Key Vault.

- **Azure Key Vault:** We store secrets like the connection string and JWT key securely in Azure Key Vault. Azure Key Vault acts as a secure store for these secrets, and our application can access them only with proper authorization.

### Different Configuration in Production and Development
Our application's configuration differs between production and development environments to accommodate security and operational considerations.

#### Production Environment
In the production environment:

- The Azure Key Vault URL is retrieved from an environment variable set in the Azure App Service configuration.
- We use the `ManagedIdentityCredential` to authenticate with Azure Key Vault and retrieve secrets like the connection string and JWT key.
- These secrets are then used to configure the application's services and authentication.

#### Development Environment
In the development environment:

- Configuration values are retrieved from the local `user-secrets` we have set up [before](#initializing-user-secrets).
- This includes connection strings and JWT keys for local development purposes.
- The `builder.Configuration["ConnectionsStrings:defaultConnection"]` and `builder.Configuration["JWTKEY"]` methods are used to access these values.

### Conclusion
By utilizing Managed Identity, Azure Key Vault, and environment-specific configuration, we ensure that sensitive information remains secure and that our application is properly configured for both production and development environments. This approach enhances security, simplifies configuration management, and follows best practices for Azure-based applications.

Make sure to consistently maintain and monitor the configuration to ensure the highest level of security and operational efficiency.


## API Reference

Base Url: "api/[controller]"
### User Controller 

#### Register 

Registers a new user.

- **URL:** `/api/user/register`
- **Method:** `POST`
- **Headers:** None (AllowAnonymous)
- **Request Body:**
  - `User` object: User registration details

#### Login

Logs in a user and returns a JWT token.

- **URL:** `/api/user/login`
- **Method:** `POST`
- **Headers:** None (AllowAnonymous)
- **Request Body:**
  - `LoginDto` object: User login credentials

#### Get All Users

Retrieves a list of all users.

- **URL:** `/api/user/allUsers`
- **Method:** `GET`
- **Headers:** None (AllowAnonymous)

#### Update Device Token

Updates the device token of a user.

- **URL:** `/api/user/updateDeviceToken`
- **Method:** `PUT`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Request Body:**
  - `DeviceTokenUpdateDto` object: User ID and updated device token

#### Update User Information

Updates the information of a user.

- **URL:** `/api/user/informationUpdate`
- **Method:** `PUT`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Request Body:**
  - `UpdateInformationDto` object: User ID and updated information

#### Update User Password

Updates the password of a user.

- **URL:** `/api/user/passwordUpdate`
- **Method:** `PUT`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Request Body:**
  - `UpdateUserPasswordDto` object: User ID, old password, and new password

#### Get User by Car Number

Retrieves a user by their car number.

- **URL:** `/api/user/searchCarNumber/`
- **Method:** `GET`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Query Parameter:**
  - `carNumber` (string): Car number to search for

#### Get User by ID

Retrieves a user by their ID.

- **URL:** `/api/user/getById/`
- **Method:** `GET`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Query Parameters:**
  - `id` (int): User ID
  - `minimal` (bool): Whether to retrieve minimal user information

#### Read Message Inbox

Marks an accident message as read in the user's inbox.

- **URL:** `/api/user/readMessageInbox`
- **Method:** `PUT`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Request Body:**
  - `AccidentDeleteDto` object: User ID and accident ID

#### Delete Message

Deletes an accident message from the user's history.

- **URL:** `/api/user/deleteMessage`
- **Method:** `PUT`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Request Body:**
  - `AccidentDeleteDto` object: User ID and accident ID

#### Delete User

Deletes a user by their ID.

- **URL:** `/api/user/deleteUser/{id}`
- **Method:** `DELETE`
- **Headers:** Authorization Bearer Token (User must be authenticated)


### Accident Controller

Base Url: "api/[controller]"

#### Create Accident Note

Creates a new accident note.

- **URL:** `/api/accident/createNote`
- **Method:** `POST`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Request Body:**
  - `CreateNoteReqDto` object: Details for creating an accident note

#### Create Accident Report

Creates a new accident report.

- **URL:** `/api/accident/createReport`
- **Method:** `POST`
- **Headers:** Authorization Bearer Token (User must be authenticated)
- **Request Body:**
  - `CreateReportReqDto` object: Details for creating an accident report

### Stats Controller

Base Url: "api/[controller]"

#### Get Registered Users Data

Retrieves data about registered users for a specific year.

- **URL:** `/api/stats/registeredUsersData`
- **Method:** `GET`
- **Headers:** Authorization Bearer Token with Admin Role (User must be authenticated and have Admin role)
- **Query Parameter:**
  - `year` (int): The year for which to retrieve registered users data

#### Get Reports Distribution Data

Retrieves data about the distribution of accident reports.

- **URL:** `/api/stats/reportsDistribution/`
- **Method:** `GET`
- **Headers:** Authorization Bearer Token with Admin Role (User must be authenticated and have Admin role)


## Author and Feedback
If you have any feedback, please reach out to me at korenkaplan96@gmail.com.

Developed by [@korenkaplan](https://github.com/korenkaplan)

