using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int checkpointNumber;

    void OnTriggerEnter(Collider other)
    {
        if (RaceManager.Instance != null)
        {
            // Check if the collider belongs to a registered race participant
            if (RaceManager.Instance.participantsData.ContainsKey(other.gameObject))
            {
                RaceManager.Instance.ParticipantHitCheckpoint(other.gameObject, this);
            }
        }
    }
}
