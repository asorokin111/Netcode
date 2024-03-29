using FishNet.Object;
using FishNet.Object.Synchronizing;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{
    public static ScoreManager instance;
    public delegate void ScoresUpdatedAction();
    public static event ScoresUpdatedAction OnScoresUpdated;
    public Dictionary<int, int> scores = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    private void OnEnable()
    {
        Goal.OnPlayerScored += UpdateScoresServer;
    }

    private void OnDisable()
    {
        Goal.OnPlayerScored -= UpdateScoresServer;
    }

    [ServerRpc (RequireOwnership = false)]
    private void UpdateScoresServer(int scoringPlayer)
    {
        UpdateScoresObserver(scoringPlayer);
    }

    [ObserversRpc]
    private void UpdateScoresObserver(int scoringPlayer)
    {
        if (scores.ContainsKey(scoringPlayer))
        {
            ++scores[scoringPlayer];
        }
        else
        {
            scores.Add(scoringPlayer, 1);
        }
        OnScoresUpdated?.Invoke();
    }
}
