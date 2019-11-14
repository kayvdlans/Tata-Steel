using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "ScriptableObjects/UserData")]
public class UserData : ScriptableObject
{
    [SerializeField] private RoomSettings lastRoom;
    [Space]
    [SerializeField] private UserInfo user = new UserInfo();
    [SerializeField] private List<SessionInfo> sessions = new List<SessionInfo>();
    [SerializeField] private List<LevelInfo> highscores = new List<LevelInfo>();

    public uint ID { get; private set; }
    public int SessionsAmount { get => sessions.Count; }

    private DataTable userTable = new DataTable();
    private DataTable sessionTable = new DataTable();

    public void UpdateHighscores()
    {
        if (sessions.Count == 0)
            return;

        //0 is training room and we dont want to add that to the highscores.
        for (uint i = 1; i <= lastRoom.RoomID; i++)
        {
            List<SessionInfo> relevantSessions = new List<SessionInfo>();
            relevantSessions.AddRange(sessions.Where(s => s.LevelID == i));

            if (relevantSessions != null && relevantSessions.Count > 0)
            { 
                LevelInfo highscore = new LevelInfo()
                {
                    UserID          = ID,
                    LevelID         = i,
                    BestTime        = uint.MaxValue,
                    HighestPoints   = 0,
                    LowestMistakes  = uint.MaxValue,
                    TotalAttempts   = (uint)relevantSessions.Count             
                };

                for (int j = 0; j < relevantSessions.Count; j++)
                {
                    if (relevantSessions[j].Time < highscore.BestTime)
                        highscore.BestTime = relevantSessions[j].Time;

                    if (relevantSessions[j].Points > highscore.HighestPoints)
                        highscore.HighestPoints = relevantSessions[j].Points;

                    if (relevantSessions[j].Mistakes < highscore.LowestMistakes)
                        highscore.LowestMistakes = relevantSessions[j].Mistakes;
                }

                bool highscoreExists = false;

                for (int j = 0; j < highscores.Count; j++)
                {
                    if (highscores[j].LevelID == i)
                    {
                        highscores[j] = highscore;
                        highscoreExists = true;
                        break;
                    }
                }

                if (!highscoreExists)
                {
                    highscores.Add(highscore);
                }
            }
        }
    }

    public async void AddSession(SessionInfo session)
    {
        if (sessions.Contains(session))
            return;

        sessions.Add(session);
        await DatabaseConnection.DBSessionInsertQuery(session);
    }

    public async void InitializeUser(uint id)
    {
        ID = id;

        await DatabaseConnection.DBReadQuery("user", ID, userTable);

        if (userTable.Rows.Count == 0)
            await DatabaseConnection.DBUserInsertQuery(user = new UserInfo()
            {
                ID                  = ID,
                TrainingFinished    = false,
                TotalTime           = 0,
                TotalPoints         = 0,
                TotalMistakes       = 0,
                TotalAttempts       = 0
            });
        else
            user = new UserInfo()
            {
                ID                  = (uint)userTable.Rows[0]["id"],
                TrainingFinished    = (byte)userTable.Rows[0]["training_finished"] > 0,
                TotalTime           = (uint)userTable.Rows[0]["time_spent_total"],
                TotalPoints         = (uint)userTable.Rows[0]["points_total"],
                TotalMistakes       = (uint)userTable.Rows[0]["mistakes_total"],
                TotalAttempts       = (uint)userTable.Rows[0]["attempts_total"]
            };

        sessions.Clear();
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

        highscores.Clear();
        UpdateHighscores();
    }
}
