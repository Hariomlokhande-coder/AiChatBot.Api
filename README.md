AiChatBot.Api
=============

Description
-----------
AiChatBot.Api is a simple ASP.NET Core Web API (targeting .NET 8) that provides a conversational AI backend. It stores conversation history in SQL Server via Entity Framework Core and forwards conversation history to a hosted LLM provider (configured via settings) to obtain assistant responses.

Key features
------------
- REST API for sending messages, retrieving conversation history (paginated and full), listing conversations, renaming and deleting conversations
- Persistence using Entity Framework Core with SQL Server
- Pluggable OpenAI-style service abstraction (IOpenAiService) with an implementation that calls a hosted LLM endpoint
- Lightweight Angular frontend included in the ai-chatbot-ui folder (separate project)
- Swagger UI available in development for interactive testing

Technical stack
---------------
- .NET 8 (ASP.NET Core Web API)
- Entity Framework Core 8 (SqlServer provider)
- Swashbuckle (Swagger)
- Angular (frontend located in ai-chatbot-ui)

Repository layout
-----------------
- Controllers/ChatController.cs        - API endpoints
- Data/ApplicationDbContext.cs         - EF Core DbContext with audit hooks
- Models/                              - Request/response and EF entities (ChatMessage, Conversation, DTOs)
- Services/OpenAiService.cs            - HTTP client integration with configured LLM endpoint
- Settings/OpenAiSettings.cs           - Configuration contract for the LLM provider
- ai-chatbot-ui/                       - Angular frontend (separate client app)
- Migrations/                          - EF Core migrations
- Program.cs                           - Application startup

Configuration
-------------
The app reads configuration from appsettings.json and environment-specific files. Important settings:

- ConnectionStrings:DefaultConnection
  - SQL Server connection string for the application's database.

- OpenAI (section name: "OpenAI")
  - ApiKey: the API key for your LLM provider (DO NOT commit real keys to source control)
  - Model: which model to request
  - MaxHistoryMessages: how many messages to include in the prompt

Recommended: store secrets using environment variables or the .NET user secrets store rather than committing API keys into source control.

Example appsettings snippet (safe placeholder):

{
  "ConnectionStrings": {
	"DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=AiChatBotDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "OpenAI": {
	"ApiKey": "<YOUR_API_KEY_HERE>",
	"Model": "llama-3.3-70b-versatile",
	"MaxHistoryMessages": 20
  }
}

Running locally
---------------
Prerequisites:
- .NET 8 SDK
- SQL Server or LocalDB
- (optional) Node.js and npm for the Angular frontend

1. Restore & build:
   dotnet restore
   dotnet build

2. Apply migrations and create the database (install dotnet-ef if needed):
   dotnet tool install --global dotnet-ef
   dotnet ef database update --project .

3. Run the API:
   dotnet run

4. Open Swagger (in Development environment) at https://localhost:{port}/swagger to explore the API.

Angular frontend (optional):
- cd ai-chatbot-ui
- npm install
- npm start

API Endpoints
-------------
Base route: /api/chat

1) POST /api/chat/send
- Request JSON:
  {
	"message": "Hello, assistant!",
	"conversationId": "<optional-guid>"
  }
- Behavior: saves the user message, forwards conversation history to the configured LLM service, saves the assistant response, and returns the assistant content with the conversationId.
- Success response (200):
  {
	"success": true,
	"conversationId": "...",
	"message": "Assistant response text",
	"timestampUtc": "..."
  }

2) GET /api/chat/{conversationId}
- Returns full message list for the conversation (mapped roles: "assistant" -> "ai" for frontend)

3) GET /api/chat/{conversationId}/history?before={isoDateTime}&pageSize={int}
- Paginated history (descending by time) with HasMore flag. Default pageSize = 10.

4) GET /api/chat/conversations
- Lists conversations (ConversationSummary) ordered by CreatedDateUtc desc

5) DELETE /api/chat/{conversationId}
- Deletes the conversation and its messages. Returns 204 No Content on success.

6) PATCH /api/chat/{conversationId}/rename
- Request JSON: { "title": "New Title" }
- Renames the conversation title. Returns 204 No Content on success.

Sample curl (send message):

curl -X POST "https://localhost:5001/api/chat/send" -H "Content-Type: application/json" -d "{\"message\":\"Hello world\"}"

Security
--------
- Do not expose your OpenAI/LLM API key in source control. Use environment variables or the ASP.NET Core secrets manager.
- The sample project does not include authentication; consider adding authentication and per-user scoping for production.

Extending the LLM provider
--------------------------
- The project uses an IOpenAiService interface. To swap providers implement IOpenAiService and register the implementation in Program.cs. The existing OpenAiService uses a Groq-compatible endpoint; adapt the URL and JSON parsing for other providers.

Database & Migrations
---------------------
- Migrations are located under Migrations/. To add a migration:
  dotnet ef migrations add <Name>
  dotnet ef database update

Contributing
------------
- Fork the repo, create a feature branch, add tests (if applicable), and submit a pull request.

License
-------
This project is provided as-is. Add an appropriate license file if you plan to open-source this repository.

Support
-------
For issues or questions, open an issue in the repository or contact the maintainer.
