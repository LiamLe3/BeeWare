using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DifficultyApplier : MonoBehaviour
{
    const int EASY = 0;
    const int MEDIUM = 1;
    const int HARD = 2;

    const int UNREVEALED = -3;

    [SerializeField] Tilemap tileMap;
    
    Camera mainCamera;
    Painter painter;

    private readonly Dictionary<int, GameSettings> difficultySettings = new Dictionary<int, GameSettings>
    {
        { EASY, new GameSettings(7, 11, 10, new Vector3(3.702355f, 3.365f, -1), 3.846154f) },
        { MEDIUM, new GameSettings(9, 15, 20, new Vector3(5.231393f, 4.355f, -1), 4.83871f) },
        { HARD, new GameSettings(11, 19, 30, new Vector3(6.770431f, 5.385f, -1), 5.851064f) }
    };

    void Awake()
    {
        mainCamera = Camera.main;
        painter = FindObjectOfType<Painter>();
    }

    public GameState ApplyDifficulty(int difficulty, GameState gameState)
    {
        if (difficultySettings.TryGetValue(difficulty, out var settings))
        {
            gameState = new GameState(settings.rows, settings.cols, settings.beesTotal);
            RepositionCamera(settings.cameraPosition, settings.orthographicSize);
            InitialiseBoard(gameState);
        }
        else
        {
            Debug.LogError("Invalid difficulty level");
        }

        return gameState;
    }

    void RepositionCamera(Vector3 position, float orthographicSize)
    {
        mainCamera.transform.position = position;
        mainCamera.orthographicSize = orthographicSize;
    }

    void InitialiseBoard(GameState gameState)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        Hex[ , ] board = new Hex[gameState.rows, gameState.cols];

        tileMap.ClearAllTiles();

        for(int row = 0; row < gameState.rows; row++)
            for(int col = 0; col < gameState.cols; col++)
            {
                board[row, col] = new Hex();  //Initialise Hex
                painter.PaintTile(row, col, UNREVEALED); // Set tile as unrevealed
                positions.Add(new Vector2Int(row, col)); // Add to list  of positions to generate bees later
            }

        gameState.positions = positions;
        gameState.board = board;
    }

    class GameSettings
    {
        public int rows { get; }
        public int cols { get; }
        public int beesTotal { get; }
        public Vector3 cameraPosition { get; }
        public float orthographicSize { get; }

        public GameSettings(int rows, int cols, int beesTotal, Vector3 cameraPosition, float orthographicSize)
        {
            this.rows = rows;
            this.cols = cols;
            this.beesTotal = beesTotal;
            this.cameraPosition = cameraPosition;
            this.orthographicSize = orthographicSize;
        }
    }
}
