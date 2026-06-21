using UnityEngine;
using System.Collections.Generic;

public class AIBoatController : MonoBehaviour
{
    [Header("AI Boat Settings")]
    public float moveSpeed = 8f;
    public float turnSpeed = 40f;
    public float waypointTolerance = 5f;
    public float obstacleAvoidanceDistance = 10f;
    public float avoidanceForce = 50f;

    private Rigidbody rb;
    private List<Checkpoint> waypoints;
    private int currentWaypointIndex = 0;
    private bool raceStarted = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody not found on AIBoatController. Please add a Rigidbody component.");
            enabled = false;
        }
    }

    void Start()
    {
        // Get waypoints from RaceManager (assuming checkpoints double as AI waypoints)
        if (RaceManager.Instance != null)
        {
            waypoints = RaceManager.Instance.checkpoints;
        }
        else
        {
            Debug.LogError("RaceManager not found. AI will not have waypoints.");
            enabled = false;
            return;
        }

        // AI boats should be added to RaceManager.raceParticipants in the Unity Editor.
        // RaceManager will handle starting the race for all participants.
    }

    void FixedUpdate()
    {
        if (!RaceManager.Instance.IsRaceStarted() || RaceManager.Instance.IsRaceFinished() || waypoints == null || waypoints.Count == 0)
            return;

        NavigateToWaypoint();
        AvoidObstacles();
    }

    void NavigateToWaypoint()
    {
        // Ensure the AI has a valid target waypoint
        if (currentWaypointIndex >= waypoints.Count)
        {
            // This should ideally not happen if RaceManager correctly manages AI laps
            // For now, if it somehow goes out of bounds, reset to 0 or handle as error
            currentWaypointIndex = 0; 
            return;
        }

        Vector3 targetPosition = waypoints[currentWaypointIndex].transform.position;
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;

        // Apply movement force
        rb.AddForce(transform.forward * moveSpeed, ForceMode.Acceleration);

        // Calculate turn direction
        float angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        float turn = Mathf.Clamp(angle, -1f, 1f) * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Check if reached waypoint
        if (Vector3.Distance(transform.position, targetPosition) < waypointTolerance)
        {
            // AI has reached its current target waypoint, now check if it's a checkpoint
            Checkpoint hitCheckpoint = waypoints[currentWaypointIndex];
            if (hitCheckpoint != null)
            {
                RaceManager.Instance.ParticipantHitCheckpoint(this.gameObject, hitCheckpoint);
            }
            
            // Advance to the next waypoint in its internal sequence
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Count)
            {
                // If all checkpoints for the current lap are hit, RaceManager will handle lap completion.
                // The AI should then target the first checkpoint of the next lap.
                // For now, reset to 0 to simulate continuous looping for AI.
                currentWaypointIndex = 0; 
            }
        }
    }

    void AvoidObstacles()
    {
        RaycastHit hit;
        // Cast a ray forward to detect obstacles
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceDistance))
        {
            // Check if the hit object is an obstacle or another boat (excluding self)
            if ((hit.collider.CompareTag("Obstacle") || hit.collider.CompareTag("Boat")) && hit.collider.gameObject != this.gameObject)
            {
                // Calculate an avoidance direction (e.g., perpendicular to the hit normal)
                Vector3 avoidanceDirection = Vector3.Cross(Vector3.up, hit.normal);
                // Apply force to steer away from the obstacle
                rb.AddForce(avoidanceDirection * avoidanceForce, ForceMode.Acceleration);
            }
        }
    }
}
