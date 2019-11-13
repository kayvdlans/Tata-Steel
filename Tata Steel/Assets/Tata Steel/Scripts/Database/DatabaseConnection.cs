using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using System.Data;
using MySql.Data.MySqlClient;

public class DatabaseConnection : MonoBehaviour
{
    [SerializeField] private uint userID;
    [SerializeField] private int trainingLevelID;

    private const string CONN_STRING = @"Server=127.0.0.1;Database=tatasteeldb;User=root;Password=WeLoveTataSteel;CharSet=utf8";

    private DataTable userTable = new DataTable();
    private DataTable sessionTable = new DataTable();

    public struct UserInfo
    {
        public uint ID { get; set; }
        public bool TrainingFinished { get; set; }
        public uint TotalTime { get; set; }
        public uint TotalPoints { get; set; }
        public uint TotalMistakes { get; set; }
        public uint TotalAttempts { get; set; } //== amount of sessions
    }

    public struct SessionInfo
    {
        public uint SessionID { get; set; }
        public uint LevelID { get; set; }
        public uint Time { get; set; }
        public uint Points { get; set; }
        public uint Mistakes { get; set; }
        public uint UserID { get; set; }
    }

    public struct LevelInfo
    {
        public uint UserID { get; set; }
        public uint LevelID { get; set; }
        public uint BestTime { get; set; }
        public uint HighestPoints { get; set; }
        public uint LowestMistakeCount { get; set; }
        public uint Attempts { get; set; }
    }

    private UserInfo user = new UserInfo(); 
    private List<SessionInfo> sessions = new List<SessionInfo>();

    private void Start()
    {
        InitializeUser(userID);
    }

    public async void InitializeUser(uint id)
    {
        await DBReadQuery("user", id, userTable);
        user = new UserInfo()
        {
            ID                  = (uint)userTable.Rows[0]["id"],
            TrainingFinished    = (byte)userTable.Rows[0]["training_finished"] > 0,
            TotalTime           = (uint)userTable.Rows[0]["time_spent_total"],
            TotalPoints         = (uint)userTable.Rows[0]["points_total"],
            TotalMistakes       = (uint)userTable.Rows[0]["mistakes_total"],
            TotalAttempts       = (uint)userTable.Rows[0]["attempts_total"]
        };

        await DBReadQuery("session", id, sessionTable);
        foreach (DataRow row in sessionTable.Rows)
        {
            sessions.Add(new SessionInfo()
            {
                SessionID   = (uint)row["session_id"],
                LevelID     = (uint)row["level_id"],
                Time        = (uint)row["time_spent"],
                Points      = (uint)row["points"],
                Mistakes    = (uint)row["mistakes"],
                UserID      = (uint)row["user_id"]
            });
        }
    }

    private async Task ExecuteCommand(string command, MySqlConnection connection)
    {
        MySqlCommand cmd = new MySqlCommand(command, connection);
        await cmd.ExecuteReaderAsync();
        cmd.Dispose();
    }

    private async Task DBReadQuery(string dbName, uint userID, DataTable table = null)
    {
        MySqlConnection conn = new MySqlConnection(CONN_STRING);

        await conn.OpenAsync();

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

    public async Task DBUserUpdateQuery(UserInfo user)
    {
        MySqlConnection conn = new MySqlConnection(CONN_STRING);
        await conn.OpenAsync();

        UserInfo updatedInfo = new UserInfo()
        {
            ID                  = user.ID,
            TrainingFinished    = false,
            TotalTime           = 0,
            TotalPoints         = 0,
            TotalMistakes       = 0
        };

        for (int j = 0; j < sessions.Count; j++)
        {
            if (sessions[j].LevelID == trainingLevelID)
                updatedInfo.TrainingFinished = true;
            else
            {
                updatedInfo.TotalTime       += sessions[j].Time;
                updatedInfo.TotalPoints     += sessions[j].Points;
                updatedInfo.TotalMistakes   += sessions[j].Mistakes;
                updatedInfo.TotalAttempts   ++;
            }
        }

        string command = 
            "UPDATE user SET "
           + " id = "                   + updatedInfo.ID
           + ", training_finished = "   + (updatedInfo.TrainingFinished ? "1" : "0")
           + ", time_spent_total = "    + updatedInfo.TotalTime
           + ", points_total = "        + updatedInfo.TotalPoints
           + ", mistakes_total = "      + updatedInfo.TotalMistakes
           + ", attempts_total = "      + updatedInfo.TotalAttempts
           + " WHERE id = "             + updatedInfo.ID;

        await ExecuteCommand(command, conn);
        conn.Close();
    }

    public async Task DBUserInsertQuery(UserInfo user)
    {
        string command =
              "INSERT INTO user(id, training_finished, time_spent_total, points_total, mistakes_total, attempts_total) "
              + "VALUES("   + user.ID
              + ", "        + user.TrainingFinished
              + ", "        + user.TotalTime
              + ", "        + user.TotalPoints
              + ", "        + user.TotalMistakes
              + ", "        + user.TotalAttempts;

        MySqlConnection conn = new MySqlConnection(CONN_STRING);
        await conn.OpenAsync();
        await ExecuteCommand(command, conn);
        conn.Close();
    }

    public async Task DBSessionInsertQuery(SessionInfo session)
    {
        string command =
               "INSERT INTO session(session_id, level_id, time_spent, points, mistakes, user_id) "
               + "VALUES("  + session.SessionID
               + ", "       + session.LevelID
               + ", "       + session.Time
               + ", "       + session.Points
               + ", "       + session.Mistakes
               + ", "       + session.UserID 
               + ");";

        MySqlConnection conn = new MySqlConnection(CONN_STRING);
        await conn.OpenAsync();      
        await ExecuteCommand(command, conn);
        conn.Close();
    }
}
