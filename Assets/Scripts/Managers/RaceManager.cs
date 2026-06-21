using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RaceManager : MonoBehaviour
{
    public static RaceManager Instance;

    [Header("Race Settings")]
    public int totalLaps = 3;
    public List<Checkpoint> checkpoints;
    public List<GameObject> raceParticipants; // Player and AI boats

    private float raceStartTime;
    private bool raceStarted = false;
    private bool raceFinished = false;

    // Store race data for each participant
    public class ParticipantRaceData
    {
        public GameObject participantObject;
        public int currentLap = 0;
        public int lastCheckpointIndex = -1;
        public float currentLapTime;
        public float bestLapTime = Mathf.Infinity;
        public float raceTime;
        public bool finishedRace = false;
        public int currentPosition = 0; // For real-time position tracking

        public ParticipantRaceData(GameObject obj) { participantObject = obj; }
    }

    public Dictionary<GameObject, ParticipantRaceData> participantsData = new Dictionary<GameObject, ParticipantRaceData>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeParticipants();
        // For now, start race immediately. Later, this will be triggered by UI.
        StartRace();
    }

    void Update()
    {
        if (raceStarted && !raceFinished)
        {
            foreach (var data in participantsData.Values)
            {
                if (!data.finishedRace)
                {
                    data.raceTime = Time.time - raceStartTime;
                    data.currentLapTime += Time.deltaTime;
                }
            }
            UpdateRacePositions();
            // TODO: Update UI with race data
        }
    }

    void InitializeParticipants()
    {
        foreach (GameObject participant in raceParticipants)
        {
            participantsData.Add(participant, new ParticipantRaceData(participant));
        }
    }

    public void StartRace()
    {
        raceStarted = true;
        raceStartTime = Time.time;
        foreach (var data in participantsData.Values)
        {
            data.currentLap = 1;
            data.lastCheckpointIndex = -1;
            data.currentLapTime = 0f;
            data.raceTime = 0f;
            data.finishedRace = false;
        }
        Debug.Log("Race Started!");
    }

    public void ParticipantHitCheckpoint(GameObject participant, Checkpoint checkpoint)
    {
        if (!participantsData.ContainsKey(participant))
        {
            Debug.LogWarning($"Participant {participant.name} not registered in RaceManager.");
            return;
        }

        ParticipantRaceData data = participantsData[participant];
        if (data.finishedRace) return; // Ignore if already finished

        int hitCheckpointIndex = checkpoints.IndexOf(checkpoint);

        // Ensure checkpoints are hit in order
        if (hitCheckpointIndex == data.lastCheckpointIndex + 1)
        {
            data.lastCheckpointIndex = hitCheckpointIndex;
            Debug.Log($"{participant.name} hit Checkpoint {checkpoint.checkpointNumber}");

            // If participant hits the last checkpoint of a lap
            if (data.lastCheckpointIndex == checkpoints.Count - 1)
            {
                data.currentLap++;
                data.lastCheckpointIndex = -1; // Reset for next lap

                if (data.currentLap <= totalLaps)
                {
                    Debug.Log($"{participant.name} completed Lap {data.currentLap - 1}. Current Lap: {data.currentLap}");
                    if (data.currentLapTime < data.bestLapTime)
                    {
                        data.bestLapTime = data.currentLapTime;
                    }
                    data.currentLapTime = 0f; // Reset lap time for new lap
                }
                else
                {
                    data.finishedRace = true;
                    Debug.Log($"{participant.name} finished race!");
                    CheckRaceEnd();
                }
            }
        }
        else
        {
            Debug.LogWarning($"{participant.name} hit checkpoint {checkpoint.checkpointNumber} out of order or already hit.");
        }
    }

    void UpdateRacePositions()
    {
        // Sort participants by lap, then checkpoint, then race time
        var sortedParticipants = participantsData.Values
            .OrderByDescending(p => p.currentLap)
            .ThenByDescending(p => p.lastCheckpointIndex)
            .ThenBy(p => p.raceTime)
            .ToList();

        for (int i = 0; i < sortedParticipants.Count; i++)
        {
            sortedParticipants[i].currentPosition = i + 1;
            // Debug.Log($"{sortedParticipants[i].participantObject.name} is in position {sortedParticipants[i].currentPosition}");
        }
    }

    void CheckRaceEnd()
    {
        if (participantsData.Values.All(p => p.finishedRace))
        {
            raceFinished = true;
            Debug.Log("All participants finished the race!");
            DisplayRaceResults();
        }
    }

    void DisplayRaceResults()
    {
        Debug.Log("--- Race Results ---");
        var finalResults = participantsData.Values
            .OrderBy(p => p.raceTime)
            .ToList();

        for (int i = 0; i < finalResults.Count; i++)
        {
            Debug.Log($"Position {i + 1}: {finalResults[i].participantObject.name} - Time: {finalResults[i].raceTime:F2}s, Best Lap: {finalResults[i].bestLapTime:F2}s");
        }
        // TODO: Trigger UI for race results screen
    }

    // Public getters for UI to display race data
    public bool IsRaceStarted() { return raceStarted; }
    public bool IsRaceFinished() { return raceFinished; }

    public ParticipantRaceData GetParticipantData(GameObject participant) {
        if (participantsData.ContainsKey(participant)) return participantsData[participant];
        return null;
    }
}
