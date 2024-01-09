using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public delegate void GolfBallScoredAction(GameObject lastHitter);
    public static event GolfBallScoredAction OnGolfBallScored;
    private GameObject _lastHitter;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goal"))
        {
            OnGolfBallScored?.Invoke(_lastHitter);
        }
    }
}
