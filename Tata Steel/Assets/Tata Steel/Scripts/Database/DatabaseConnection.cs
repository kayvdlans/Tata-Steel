using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data;
using MySql.Data.MySqlClient;

public static class DatabaseConnection 
{
    //TODO: UPDATE THIS STRING ONCE THE DATABASE GOES ONLINE. 
    private const string CONN_STRING = @"Server=127.0.0.1;Database=tatasteeldb;User=root;Password=WeLoveTataSteel;CharSet=utf8";

    private static async Task ExecuteCommand(string command, MySqlConnection connection)
    {
        MySqlCommand cmd = new MySqlCommand(command, connection);
        await cmd.ExecuteReaderAsync();
        cmd.Dispose();
    }

    public static async Task DBReadQuery(string dbName, uint userID, DataTable table = null)
    {
        MySqlConnection conn = new MySqlConnection(CONN_STRING);

        await conn.OpenAsync();

        //since the tables have different names for the user id, check which table is being accessed.
        string idName = dbName == "user" ? "id" : "user_id";
        string cmdString = "SELECT * FROM " + dbName + " WHERE " + idName + " = " + userID;

        MySqlCommand cmd = new MySqlCommand(cmdString, conn);
        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
        
        if (table != null)
            adapter.Fill(table);

        cmd.Dispose();
        adapter.Dispose();

        conn.Close();
    }

    public static async Task DBUserUpdateQuery(UserInfo user)
    {
        string command =
            "UPDATE user SET "
           + " id = " + user.ID
           + ", training_finished = " + (user.TrainingFinished ? "1" : "0")
           + ", time_spent_total = " + user.TotalTime
           + ", points_total = " + user.TotalPoints
           + ", mistakes_total = " + user.TotalMistakes
           + ", attempts_total = " + user.TotalAttempts
           + " WHERE id = " + user.ID;

        MySqlConnection conn = new MySqlConnection(CONN_STRING);
        await conn.OpenAsync();
        await ExecuteCommand(command, conn);
        conn.Close();
    }

    public static async Task DBUserInsertQuery(UserInfo user)
    {
        string command =
              "INSERT INTO user(id, training_finished, time_spent_total, points_total, mistakes_total, attempts_total) "
              + "VALUES("   + user.ID
              + ", "        + (user.TrainingFinished ? "1" : "0")
              + ", "        + user.TotalTime
              + ", "        + user.TotalPoints
              + ", "        + user.TotalMistakes
              + ", "        + user.TotalAttempts
              + ");";

        MySqlConnection conn = new MySqlConnection(CONN_STRING);
        await conn.OpenAsync();
        await ExecuteCommand(command, conn);
        conn.Close();
    }

    public static async Task DBSessionInsertQuery(SessionInfo session)
    {
        string command =
               "INSERT INTO session(session_id, level_id, time_spent, points, mistakes, passed, user_id) "
               + "VALUES("  + session.SessionID
               + ", "       + session.LevelID
               + ", "       + session.Time
               + ", "       + session.Points
               + ", "       + session.Mistakes
               + ", "       + (session.Passed ? "1" : "0")
               + ", "       + session.UserID 
               + ");";

        MySqlConnection conn = new MySqlConnection(CONN_STRING);
        await conn.OpenAsync();      
        await ExecuteCommand(command, conn);
        conn.Close();
    }
}
