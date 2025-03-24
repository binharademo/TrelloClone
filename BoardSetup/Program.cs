// See https://aka.ms/new-console-template for more information
using System;
using Microsoft.Data.Sqlite;

// Código para adicionar as listas solicitadas (Todo, Priority, Doing e Done) ao quadro do usuário teste
Console.WriteLine("Adicionando listas ao quadro do usuário teste...");

string dbPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "TrelloClone", "trelloclone.db");
string connectionString = $"Data Source={dbPath}";

Console.WriteLine($"Conectando ao banco de dados: {dbPath}");

using (var connection = new SqliteConnection(connectionString))
{
    connection.Open();
    
    // 1. Encontrar o ID do usuário teste
    int testUserId = 0;
    using (var userCommand = connection.CreateCommand())
    {
        userCommand.CommandText = "SELECT Id FROM Users WHERE Email = 'test@example.com'";
        using (var reader = userCommand.ExecuteReader())
        {
            if (reader.Read())
            {
                testUserId = reader.GetInt32(0);
                Console.WriteLine($"Usuário teste encontrado com ID: {testUserId}");
            }
            else
            {
                Console.WriteLine("Usuário teste não encontrado!");
                return;
            }
        }
    }
    
    // 2. Verificar se o usuário já tem um quadro
    int boardId = 0;
    bool hasBoardAlready = false;
    
    using (var boardCheckCommand = connection.CreateCommand())
    {
        boardCheckCommand.CommandText = "SELECT Id FROM Boards WHERE UserId = @UserId LIMIT 1";
        boardCheckCommand.Parameters.AddWithValue("@UserId", testUserId);
        
        using (var reader = boardCheckCommand.ExecuteReader())
        {
            if (reader.Read())
            {
                boardId = reader.GetInt32(0);
                hasBoardAlready = true;
                Console.WriteLine($"Usuário já possui um quadro com ID: {boardId}");
            }
        }
    }
    
    // 3. Se não tiver um quadro, criar um
    if (!hasBoardAlready)
    {
        using (var createBoardCommand = connection.CreateCommand())
        {
            createBoardCommand.CommandText = @"
                INSERT INTO Boards (Name, Description, UserId) 
                VALUES (@Name, @Description, @UserId);
                SELECT last_insert_rowid();";
            
            createBoardCommand.Parameters.AddWithValue("@Name", "Meu Quadro");
            createBoardCommand.Parameters.AddWithValue("@Description", "Quadro com listas personalizadas");
            createBoardCommand.Parameters.AddWithValue("@UserId", testUserId);
            
            boardId = Convert.ToInt32(createBoardCommand.ExecuteScalar());
            Console.WriteLine($"Novo quadro criado com ID: {boardId}");
        }
    }
    
    // 4. Limpar listas existentes (se houver)
    using (var clearListsCommand = connection.CreateCommand())
    {
        clearListsCommand.CommandText = "DELETE FROM Lists WHERE BoardId = @BoardId";
        clearListsCommand.Parameters.AddWithValue("@BoardId", boardId);
        
        int rowsAffected = clearListsCommand.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
            Console.WriteLine($"Removidas {rowsAffected} listas existentes do quadro.");
        }
    }
    
    // 5. Adicionar as novas listas solicitadas
    string[] listNames = new[] { "Todo", "Priority", "Doing", "Done" };
    
    foreach (var listName in listNames)
    {
        using (var addListCommand = connection.CreateCommand())
        {
            addListCommand.CommandText = @"
                INSERT INTO Lists (Name, BoardId) 
                VALUES (@Name, @BoardId);
                SELECT last_insert_rowid();";
            
            addListCommand.Parameters.AddWithValue("@Name", listName);
            addListCommand.Parameters.AddWithValue("@BoardId", boardId);
            
            long listId = (long)addListCommand.ExecuteScalar();
            Console.WriteLine($"Lista '{listName}' adicionada com ID: {listId}");
        }
    }
    
    Console.WriteLine("\nConcluído! As listas Todo, Priority, Doing e Done foram adicionadas ao quadro.");
    Console.WriteLine("Você pode fazer login com:");
    Console.WriteLine("Email: test@example.com");
    Console.WriteLine("Senha: password123");
}
