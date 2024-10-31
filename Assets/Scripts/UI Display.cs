using UnityEngine;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    [SerializeField]  TextMeshProUGUI flagsText;
    [SerializeField]  TextMeshProUGUI timeText;

    void Update()
    {
        //Check if there's a gameState and check if game is running or is a new game
        if(GameManager.gameState != null && (GameManager.gameState.isRunning || GameManager.gameState.isFirstClick))
        {
            UpdateTimeText();
            UpdateFlagsText();
        }
    }

    void UpdateTimeText()
    {
        if(GameManager.gameState.isFirstClick && !GameManager.gameState.isRunning) //New game
            GameManager.gameState.elapsedTime = 0f;
        else
            GameManager.gameState.elapsedTime = Time.time - GameManager.gameState.startTime;

        timeText.text = GameManager.gameState.elapsedTime.ToString("000");
    }

    void UpdateFlagsText()
    {
        int flagsRemaining = GameManager.gameState.beesTotal - GameManager.gameState.flagsCount;
        flagsText.text = flagsRemaining.ToString("00");
    }
}