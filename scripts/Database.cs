using Godot;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Text.Json;

public static class Database
{
    private static string DbPath => ProjectSettings.GlobalizePath("user://savegame.db");
    private static string ConnectionString => $"Data Source={DbPath}";

    public static int Points { get; set; } = 0;
    public static List<string> CompletedActivities { get; set; } = new List<string>();
    public static Vector2 LastPosition { get; set; } = Vector2.Zero;
    public static bool PlayedOnce { get; set; } = false;
    public static List<int> TopScores { get; set; } = new List<int>();

    private static void InitializeDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS PlayerData (
                Id INTEGER PRIMARY KEY,
                Points INTEGER,
                Activities TEXT,
                PosX REAL,
                PosY REAL,
                PlayedOnce INTEGER,
                Scores TEXT
            );
            INSERT OR IGNORE INTO PlayerData (Id, Points, Activities, PosX, PosY, PlayedOnce, Scores)
            VALUES (1, 0, '[]', 0, 0, 0, '[]');
        ";
        command.ExecuteNonQuery();
    }

    public static void Save()
    {
        InitializeDatabase();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE PlayerData SET
                Points = $Points,
                Activities = $Activities,
                PosX = $PosX,
                PosY = $PosY,
                PlayedOnce = $PlayedOnce,
                Scores = $Scores
            WHERE Id = 1;
        ";

        command.Parameters.AddWithValue("$Points", Points);
        command.Parameters.AddWithValue("$Activities", JsonSerializer.Serialize(CompletedActivities));
        command.Parameters.AddWithValue("$PosX", LastPosition.X);
        command.Parameters.AddWithValue("$PosY", LastPosition.Y);
        command.Parameters.AddWithValue("$PlayedOnce", PlayedOnce ? 1 : 0);
        command.Parameters.AddWithValue("$Scores", JsonSerializer.Serialize(TopScores));

        command.ExecuteNonQuery();
        GD.Print("Saved to SQLite DB successfully at: ", DbPath);
    }

    public static void Load()
    {
        try
        {
            InitializeDatabase();

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Points, Activities, PosX, PosY, PlayedOnce, Scores FROM PlayerData WHERE Id = 1;";

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                Points = reader.GetInt32(0);
                GD.Print($"Loaded Points: {Points}");

                string activitiesStr = reader.GetString(1);
                var loadedActivities = JsonSerializer.Deserialize<List<string>>(activitiesStr);
                if (loadedActivities != null) CompletedActivities = loadedActivities;
                GD.Print($"Loaded Completed Activities: {CompletedActivities.Count} stored.");

                float px = reader.GetFloat(2);
                float py = reader.GetFloat(3);
                LastPosition = new Vector2(px, py);
                GD.Print($"Loaded LastPosition: {LastPosition}");

                PlayedOnce = reader.GetInt32(4) == 1;
                GD.Print($"Loaded PlayedOnce: {PlayedOnce}");

                string scoresStr = reader.GetString(5);
                var loadedScores = JsonSerializer.Deserialize<List<int>>(scoresStr);
                if (loadedScores != null) TopScores = loadedScores;
                GD.Print($"Loaded TopScores: [{string.Join(", ", TopScores)}]");

                GD.Print("Database successfully loaded!");
            }
        }
        catch (System.Exception e)
        {
            GD.PrintErr("Failed to load SQLite DB: ", e.Message);
        }
    }
}
