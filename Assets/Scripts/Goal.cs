using UnityEngine;

public class Goal : MonoBehaviour
{
    public delegate void PlayerScoredAction(string player);
    public static event PlayerScoredAction OnPlayerScored;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("GolfBall"))
        {
            if (!other.TryGetComponent(out GolfBall golfScript)) return;
            OnPlayerScored?.Invoke(golfScript.lastHitter.ClientId.ToString());
            golfScript.RespawnServer();
        }
    }
}
