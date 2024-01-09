using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public delegate void GolfBallScoredAction(Score lastHitter);
    public static event GolfBallScoredAction OnGolfBallScored;
    private Score _lastHitter;

    public void UpdateLastHitter(Score hitter)
    {
        _lastHitter = hitter;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            OnGolfBallScored?.Invoke(_lastHitter);
        }
    }
}
