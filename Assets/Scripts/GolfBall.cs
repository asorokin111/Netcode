using UnityEngine;

public class GolfBall : MonoBehaviour
{
    public GameObject lastHitter;
    private Vector3 _respawnPosition;

    private void Awake()
    {
        _respawnPosition = transform.position;
    }

    public void Respawn()
    {
        lastHitter = null;
        transform.position = _respawnPosition;
    }
}
