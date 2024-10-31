using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public int rows { get; set; }
    public int cols { get; set; }
    public int beesTotal { get; set; }
    public int flagsCount { get; set; }
    public float startTime { get; set; }
    public float elapsedTime { get; set; }
    public bool isFirstClick { get; set; }
    public bool isRunning { get; set; }
    public Hex[ , ] board { get; set; }
    public List<Vector2Int> positions { get; set; }
    public HashSet<Vector2Int> safeZone { get; set;}

    public GameState(int rows, int cols, int beesTotal)
    {
        this.rows = rows;
        this.cols = cols;
        this.beesTotal = beesTotal;
        flagsCount = 0;
        isFirstClick = true;
    }

    public bool IsValidHex(int row, int col)
    {
        return row >= 0 && row < rows && col >= 0 && col < cols;
    }

    public IEnumerable<(int rowOffset, int colOffset)> GetDirections(int col)
    {
        bool isEvenColumn = col % 2 == 0;

        return isEvenColumn
            ? new (int, int)[] { //Even column directions (0, 2, 4, ...)
                                        (1, 0),
                                (0, -1),        (0, 1),
                                (-1, -1),       (-1, 1),
                                        (-1, 0),
                                }
            : new (int, int)[]  { //Odd column directions(1, 3, 5, ...)
                                        (1, 0),
                                (1, -1),        (1, 1),
                                (0, -1),        (0, 1),
                                        (-1, 0),
                                };

    }
}

public class Hex
{
    public int adjacentBees { get; set; }
    public bool isRevealed { get; set; }
    public bool isFlagged { get; set; }
}
