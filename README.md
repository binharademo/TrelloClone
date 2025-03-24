 # Trello Clone

A full-stack Trello clone application built with ASP.NET Core and Blazor WebAssembly, featuring real-time updates with SignalR.

## Features

- User authentication with JWT
- Create, view, update, and delete boards
- Create, view, update, and delete lists within boards
- Create, view, update, and delete cards within lists
- Drag and drop cards between lists
- Real-time updates using SignalR
- Responsive design for mobile and desktop

## Project Structure

- **TrelloClone** - ASP.NET Core API backend
- **TrelloClone.Client** - Blazor WebAssembly frontend

## Prerequisites

- .NET 9.0 SDK or later
- SQLite (included in the project)

## Test User Credentials

The application comes with a pre-configured test user:
- **Email**: test@example.com
- **Password**: password123

You can use these credentials to log in and test the application without having to register a new account.

## Error Handling

The application includes comprehensive error handling for authentication:
- Visible error messages on the login and registration pages
- Detailed logging in both frontend and backend
- API health check endpoint for connectivity testing
- Browser console logging for debugging
- Diagnostic information display directly on the login page

If you encounter login issues:
1. Check the displayed error message on the login page
2. Use the "Test API Connection" button to verify backend connectivity
3. Use the "Show Login Details" button to view your login attempt information
4. Use the "Show Password Hash" button to verify password hashing
5. Review the diagnostic information panel for detailed error context

### Backend Logging

The backend implements detailed logging for troubleshooting:
- SQL command logging to track database operations
- API request/response logging for monitoring HTTP interactions
- Authentication service logging for tracking login attempts and failures

### Frontend Diagnostics

The frontend provides several diagnostic tools:
- Detailed error messages displayed directly in the UI
- Password hash verification tool to check credential encoding
- API connectivity testing with response display
- Comprehensive diagnostic information panel

## Getting Started

### Running the API

1. Navigate to the API project directory:
   ```
   cd TrelloClone
   ```

2. Run the API:
   ```
   dotnet run
   ```

3. The API will be available at: http://localhost:5022

### Running the Client

1. Navigate to the Client project directory:
   ```
   cd TrelloClone.Client
   ```

2. Run the client:
   ```
   dotnet run
   ```

3. The client will be available at: http://localhost:5173

## Running Both Projects

You can run both projects simultaneously using the solution file:

```
dotnet run --project TrelloClone/TrelloClone.csproj
```

In another terminal:

```
dotnet run --project TrelloClone.Client/TrelloClone.Client.csproj
```

## API Endpoints

### Authentication
- POST `/api/auth/register` - Register a new user
- POST `/api/auth/login` - Login and get JWT token

### Boards
- GET `/api/boards` - Get all boards for the authenticated user
- GET `/api/boards/{id}` - Get a specific board
- POST `/api/boards` - Create a new board
- DELETE `/api/boards/{id}` - Delete a board

### Cards
- GET `/api/cards/{id}` - Get a specific card
- GET `/api/cards/{id}/history` - Get card history
- POST `/api/cards` - Create a new card
- PUT `/api/cards/{id}` - Update a card
- PUT `/api/cards/{id}/move` - Move a card to a different list
- DELETE `/api/cards/{id}` - Delete a card

## SignalR Hub

The application uses SignalR for real-time updates. The hub is available at:

```
/hubs/board
```

Events:
- `ReceiveCardAdded` - When a card is added
- `ReceiveCardUpdated` - When a card is updated
- `ReceiveCardMoved` - When a card is moved
- `ReceiveCardDeleted` - When a card is deleted

## License

MIT
