using UnityEngine;

public class Score : MonoBehaviour
{
    public int score = 0;

    private void OnEnable()
    {
        GolfBall.OnGolfBallScored += (Score hitter) => {
            if (hitter == this) ++score;
        };
    }

    private void OnDisable()
    {
        GolfBall.OnGolfBallScored -= (Score hitter) => {
            if (hitter == this) ++score; 
        };
    }
}
