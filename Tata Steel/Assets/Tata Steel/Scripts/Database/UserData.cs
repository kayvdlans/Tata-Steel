using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "UserData", menuName = "ScriptableObjects/UserData")]
public class UserData : ScriptableObject
{
    public uint ID { get; set; }

    [SerializeField] private RoomSettings lastRoom;
    [Space]
    [SerializeField] private UserInfo user = new UserInfo();
    [SerializeField] private List<SessionInfo> sessions = new List<SessionInfo>();
    [SerializeField] private List<LevelInfo> highscores = new List<LevelInfo>();

    private DataTable userTable = new DataTable();
    private DataTable sessionTable = new DataTable();

    public void UpdateHighscores()
    {
        //0 is training room and we dont want to add that to the highscores.
        for (uint i = 1; i < lastRoom.RoomID; i++)
        {
            List<SessionInfo> relevantSessions = sessions.Where(s => s.LevelID == i) as List<SessionInfo>;

            LevelInfo highscore = new LevelInfo()
            {
                UserID = ID,
                LevelID = i,
                BestTime = uint.MaxValue,
                HighestPoints = 0,
                LowestMistakes = uint.MaxValue,
                TotalAttempts = (uint)relevantSessions.Count             
            };

            if (relevantSessions.Count > 0)
            {
                for (int j = 0; j < relevantSessions.Count; j++)
                {
                    if (relevantSessions[j].Time < highscore.BestTime)
                        highscore.BestTime = relevantSessions[j].Time;

                    if (relevantSessions[j].Points > highscore.HighestPoints)
                        highscore.HighestPoints = relevantSessions[j].Points;

                    if (relevantSessions[j].Mistakes < highscore.LowestMistakes)
                        highscore.LowestMistakes = relevantSessions[j].Mistakes;

                    bool highscoreExists = false;

                    for (int k = 0; k < highscores.Count; k++)
                    {
                        if (highscores[k].LevelID == i)
                        {
                            highscores[k] = highscore;
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
