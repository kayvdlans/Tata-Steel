using UnityEngine;
using System;

[Serializable]
public struct UserInfo
{
    [SerializeField] private uint id;
    [SerializeField] private bool trainingFinished;
    [SerializeField] private uint totalTime;
    [SerializeField] private uint totalPoints;
    [SerializeField] private uint totalMistakes;
    [SerializeField] private uint totalAttempts;

    public uint ID
    {
        get => id;
        set => id = value;
    }
    public bool TrainingFinished
    {
        get => trainingFinished;
        set => trainingFinished = value;
    }
    public uint TotalTime
    {
        get => totalTime;
        set => totalTime = value;
    }
    public uint TotalPoints
    {
        get => totalPoints;
        set => totalPoints = value;
    }

    public uint TotalMistakes
    {
        get => totalMistakes;
        set => totalMistakes = value;
    }
    public uint TotalAttempts
    {
        get => totalAttempts;
        set => totalAttempts = value;
    }
}

[Serializable]
public struct SessionInfo
{
    [SerializeField] private uint sessionID;
    [SerializeField] private uint userID;
    [SerializeField] private uint levelID;
    [SerializeField] private uint time;
    [SerializeField] private uint points;
    [SerializeField] private uint mistakes;

    public uint SessionID 
    {
        get => sessionID;
        set => sessionID = value;
    }
    public uint LevelID 
    {
        get => levelID;
        set => levelID = value;
    }
    public uint Time
    {
        get => time;
        set => time = value;
    }
    public uint Points
    {
        get => points;
        set => points = value;
    }
    public uint Mistakes
    {
        get => mistakes;
        set => mistakes = value;
    }
    public uint UserID
    {
        get => userID;
        set => userID = value;
    }
}

[Serializable]
public struct LevelInfo
{
    [SerializeField] private uint userID;
    [SerializeField] private uint levelID;
    [SerializeField] private uint bestTime;
    [SerializeField] private uint highestPoints;
    [SerializeField] private uint lowestMistakes;
    [SerializeField] private uint totalAttempts;

    public uint UserID
    {
        get => userID;
        set => userID = value;
    }
    public uint LevelID
    {
        get => levelID;
        set => levelID = value;
    }
    public uint BestTime
    {
        get => bestTime;
        set => bestTime = value;
    }
    public uint HighestPoints
    {
        get => highestPoints;
        set => highestPoints = value;
    }
    public uint LowestMistakes
    {
        get => lowestMistakes;
        set => lowestMistakes = value;
    }
    public uint TotalAttempts
    {
        get => totalAttempts;
        set => totalAttempts = value;
    }
}

