using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;


class GameManager : MonoBehaviour
{
    const int FLAGGED = -2;
    const int UNREVEALED = -3;
    const int BEE = -1;

    [SerializeField] Tilemap tileMap;
    [SerializeField] AudioClip flagSFX;
    [SerializeField] AudioClip unflagSFX;
    [SerializeField] AudioClip revealSFX;
    [SerializeField] AudioClip loseSFX;
    [SerializeField] AudioClip winSFX;
    [SerializeField] AudioSource audioSource;
   
    Camera mainCamera;
    GameGenerator gameGenerator;
    Painter painter;
    DifficultyApplier difficultyApplier;

    static public GameState gameState {get; set; }
    int difficulty = 0;

    void Awake()
    {
        mainCamera = Camera.main;
        gameGenerator = FindObjectOfType<GameGenerator>();
        painter = FindObjectOfType<Painter>();
        difficultyApplier = FindObjectOfType<DifficultyApplier>();
    }
    
    void Start()
    {
        gameState = difficultyApplier.ApplyDifficulty(0, gameState);
    }   

    void OnLeftClick()
    {
        StartCoroutine(HandleClick(MouseToTilePos(), isLeftClick: true));
    }

    void OnRightClick()
    {
        StartCoroutine(HandleClick(MouseToTilePos(), isLeftClick: false));
    }

    void OnMiddleClick()
    {
        gameState = difficultyApplier.ApplyDifficulty(difficulty, gameState);
    }

    public void RestartGame()
    {
        gameState = difficultyApplier.ApplyDifficulty(difficulty, gameState);
    }

    Vector3Int MouseToTilePos()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);

        return tileMap.WorldToCell(mouseWorldPos);
    }

    IEnumerator HandleClick(Vector3Int hexPos, bool isLeftClick)
    {
        yield return null;

        if (EventSystem.current.IsPointerOverGameObject())
        {
            yield break;
        }

        bool isWithinBoard = gameState.IsValidHex(hexPos.x, hexPos.y);

        if(isWithinBoard && (gameState.isRunning || gameState.isFirstClick))
        {
            Hex currentHex = gameState.board[hexPos.x, hexPos.y];

            if(isLeftClick)
            {
                HandleLeftClick(hexPos, currentHex);
            }
            else
            {
                HandleRightClick(hexPos, currentHex);
            }
        }
    }

    void HandleLeftClick(Vector3Int hexPos, Hex currentHex)
    {
        if(gameState.isFirstClick)
        {
            gameState = gameGenerator.CreateBoard(hexPos, gameState);
            RevealTiles(hexPos.x, hexPos.y, gameState);
            gameState.isFirstClick = false;
            gameState.isRunning = true;

            gameState.startTime = Time.time;
        }
        else if(!currentHex.isFlagged)
        {
            RevealTiles(hexPos.x, hexPos.y, gameState);
        }

        CheckWin(gameState);
    }

    void HandleRightClick(Vector3Int hexPos, Hex currentHex)
    {
        if(currentHex.isRevealed)
            return;

        if(!currentHex.isFlagged)
        {
            currentHex.isFlagged = true;
            painter.PaintTile(hexPos.x, hexPos.y, FLAGGED);
            audioSource.PlayOneShot(flagSFX);
            gameState.flagsCount++;
        }
        else
        {
            currentHex.isFlagged = false;
            painter.PaintTile(hexPos.x, hexPos.y, UNREVEALED);
            audioSource.PlayOneShot(unflagSFX);
            gameState.flagsCount--;
        }
    }

    void RevealTiles(int row, int col, GameState gameState)
    {
        if(!gameState.IsValidHex(row, col) || gameState.board[row, col].isRevealed) //Do nothing if invalid or tile is revealed
        { 
            return;
        }

        Hex hex = gameState.board[row, col];
        painter.PaintTile(row, col, hex.adjacentBees); //Reveal tile
        gameState.board[row, col].isRevealed = true;

        if(hex.adjacentBees == -1) // Games ends if it is a bee
        {
            gameState.isRunning = false;
            audioSource.PlayOneShot(loseSFX);
            RevealBees(gameState);
            return;
        }

        if(!audioSource.isPlaying) // Prevent same audio being played when recurring
            audioSource.PlayOneShot(revealSFX);
        
        if(hex.adjacentBees > 0) //Don't search for any more tiles if hex is a number
        {
            return;
        }

        var directions = gameState.GetDirections(col);
        foreach(var (rowOffset, colOffset) in directions) //Recursion to reveal more hexs
            {
                int newRow = row + rowOffset;
                int newCol = col + colOffset;
                RevealTiles(newRow, newCol, gameState);
            }
    }

    void RevealBees(GameState gameState)
    {
        for(int row = 0; row < gameState.rows; row++)
        {
            for(int col = 0; col < gameState.cols; col++)
            {
                var hex = gameState.board[row, col];
                
                if (hex.adjacentBees == BEE) // Reveal Bees
                {
                    painter.PaintTile(row, col, BEE);
                }
                else if(hex.isFlagged && hex.adjacentBees != BEE) //Remove flags
                {
                    painter.PaintTile(row, col, UNREVEALED);
                }

                //painter.PaintTile(row, col, hex.adjacentBees); // Reveal all tiles
            }
        }
    }

    void CheckWin(GameState gameState)
    {
        for(int row = 0; row < gameState.rows; row++)
        {
            for(int col = 0; col < gameState.cols; col++)
            {
                var hex = gameState.board[row, col];
                if(hex.adjacentBees != BEE && !hex.isRevealed) // Checks if all non-bees are revealed
                    return;
            }
        }
        gameState.isRunning = false;
        audioSource.PlayOneShot(winSFX);
    }

    public void UpdateDifficulty(int updatedDifficulty)
    {
        difficulty = updatedDifficulty;
    }
}