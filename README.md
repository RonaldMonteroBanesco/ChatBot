# Let's create the README.md file for the ChatBotJob project

chatbotjob_readme = """# ChatBotJob

ChatBotJob is a real-time multi-room chat application built with ASP.NET Core 7, Blazor Server, and RabbitMQ.  
It allows multiple users to join chat rooms, send messages, and interact with a bot that can respond to commands like stock quotes.

## Features

- Multi-room chat (e.g., general, tech, finance, or custom rooms).
- Real-time message updates using periodic polling.
- Integration with RabbitMQ for message queuing between bot and chat.
- Stock command (`/stock=CODE`) to request stock quotes from StockBotWorker.
- Simple authentication using ASP.NET Core Identity.
- Bot messages handled in the background with `BackgroundService`.
- Graceful handling of unknown commands and exceptions.

## Technologies Used

- **.NET 7**
- **Blazor Server**
- **Entity Framework Core (SQLite)**
- **ASP.NET Core Identity**
- **RabbitMQ**
- **C# BackgroundService**

## Prerequisites

- .NET 7 SDK
- RabbitMQ installed and running locally
- SQLite (built-in support with EF Core)

## Getting Started

### 1. Clone the repository
```bash
git clone 
cd ChatBotJob
```

### 2. Install dependencies
```bash
dotnet restore
```

### 3. Apply migrations and create database
```bash
dotnet ef database update
```

### 4. Run the application
```bash
dotnet run
```

### 5. Start the StockBotWorker (optional, for stock quotes)
```bash
cd ../StockBotWorker
dotnet run
```

## Usage

1. Open **two different browser windows**.
2. Log in with **two different users**.
3. Join a room (or type a custom room name).
4. Send messages in real-time.
5. Type `/stock=aapl.us` to get the current stock quote (will not be saved in DB).

## Security Notes

- Do **not** commit secrets or sensitive information to the repository.
- Use environment variables for RabbitMQ credentials and connection strings.

## License
