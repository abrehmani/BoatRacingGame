using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI lapText;
    public TextMeshProUGUI positionText;
    public TextMeshProUGUI bestLapText;

    [Header("References")]
    public GameObject playerBoat;

    void Update()
    {
        if (RaceManager.Instance != null && RaceManager.Instance.IsRaceStarted())
        {
            UpdateHUD();
        }
    }

    void UpdateHUD()
    {
        var data = RaceManager.Instance.GetParticipantData(playerBoat);
        if (data != null)
        {
            // Format time as MM:SS.ms
            timerText.text = $"Time: {FormatTime(data.raceTime)}";
            lapText.text = $"Lap: {data.currentLap} / {RaceManager.Instance.totalLaps}";
            positionText.text = $"Pos: {data.currentPosition} / {RaceManager.Instance.participantsData.Count}";
            
            if (data.bestLapTime != Mathf.Infinity)
            {
                bestLapText.text = $"Best: {FormatTime(data.bestLapTime)}";
            }
            else
            {
                bestLapText.text = "Best: --:--.--";
            }
        }
    }

    string FormatTime(float time)
    {
        int minutes = (int)time / 60;
        int seconds = (int)time % 60;
        int fraction = (int)((time * 100) % 100);
        return string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }
}
