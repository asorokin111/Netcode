using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolfClub : MonoBehaviour
{
    private bool isClubHeld;

    private void OnEnable()
    {
        ItemPickup.OnClubPickedUp += () => isClubHeld = true;
        ItemPickup.OnClubDropped += () => isClubHeld = false;
    }

    private void OnDisable()
    {
        ItemPickup.OnClubPickedUp -= () => isClubHeld = true;
        ItemPickup.OnClubDropped -= () => isClubHeld = false;
    }
}
