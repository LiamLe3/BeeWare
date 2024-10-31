using TMPro;
using UnityEngine;

public class DropDown : MonoBehaviour
{
    [SerializeField] TMP_Dropdown dropDown;
    DifficultyApplier difficultyApplier;
    GameManager gameManager;

    void Awake()
    {
        difficultyApplier = FindObjectOfType<DifficultyApplier>();
        gameManager = FindObjectOfType<GameManager>();
    }
    
    public void ChangeDifficulty(int difficulty)
    {
        GameManager.gameState = difficultyApplier.ApplyDifficulty(difficulty, GameManager.gameState);
        gameManager.UpdateDifficulty(difficulty);
    }
}
