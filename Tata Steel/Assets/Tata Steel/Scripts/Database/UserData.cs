using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "ScriptableObjects/UserData")]
public class UserData : ScriptableObject
{
    public uint ID { get; set; }

    [SerializeField] private UserInfo user = new UserInfo();
    [SerializeField] private List<SessionInfo> sessions = new List<SessionInfo>();
    
    private DataTable userTable = new DataTable();
    private DataTable sessionTable = new DataTable();

    public async void InitializeUser(uint id)
    {
        ID = id;

        await DatabaseConnection.DBReadQuery("user", ID, userTable);

        if (userTable.Rows.Count == 0)
            await DatabaseConnection.DBUserInsertQuery(user = new UserInfo()
            {
                ID = ID,
                TrainingFinished = false,
                TotalTime = 0,
                TotalPoints = 0,
                TotalMistakes = 0,
                TotalAttempts = 0
            });
        else
            user = new UserInfo()
            {
                ID = (uint)userTable.Rows[0]["id"],
                TrainingFinished = (byte)userTable.Rows[0]["training_finished"] > 0,
                TotalTime = (uint)userTable.Rows[0]["time_spent_total"],
                TotalPoints = (uint)userTable.Rows[0]["points_total"],
                TotalMistakes = (uint)userTable.Rows[0]["mistakes_total"],
                TotalAttempts = (uint)userTable.Rows[0]["attempts_total"]
            };

        await DatabaseConnection.DBReadQuery("session", ID, sessionTable);
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
}
