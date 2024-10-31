using System.Collections.Generic;
using UnityEngine;

public class GameGenerator : MonoBehaviour
{
    const int BEE = -1;

    public GameState CreateBoard(Vector3Int firstClickPos, GameState gameState)
    {      
        GetSafeZone(firstClickPos, gameState);
        Shuffle(gameState.positions);

        PlaceBees(gameState);
        CalculateNearbyBees(gameState);
        return gameState;
    }

    void GetSafeZone(Vector3Int firstClickPos, GameState gameState)
    {   
        var directions = gameState.GetDirections(firstClickPos.y);
        
        var safeZone = new HashSet<Vector2Int> {new Vector2Int(firstClickPos.x, firstClickPos.y)};

        foreach(var (rowOffset, colOffset) in directions)
        {
            int newRow = firstClickPos.x + rowOffset;
            int newCol = firstClickPos.y + colOffset;
            if(gameState.IsValidHex(newRow, newCol))
                safeZone.Add(new Vector2Int(newRow, newCol));
        }

        gameState.safeZone = safeZone;
    }

    void Shuffle<T>(List<T> list)
    {
        var rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    void PlaceBees(GameState gameState)
    {
        List<Vector2Int> positions = gameState.positions;

        int beesPlaced = 0;
        int offSet = 0;

        while(beesPlaced < gameState.beesTotal)
        {
            Vector2Int possibleBee = positions[beesPlaced + offSet];
            if(gameState.safeZone.Contains(possibleBee)) //Skip if position is a safezone
            {
                offSet++;
            }
            else
            {
                gameState.board[possibleBee.x, possibleBee.y].adjacentBees = BEE;
                beesPlaced++;
            }
        }
    }

    void CalculateNearbyBees(GameState gameState)
    {
        int rows = gameState.rows;
        int cols = gameState.cols;

        for(int row = 0; row < rows; row++)
        {
            for(int col = 0; col < cols; col++)
            {
                if (gameState.board[row, col].adjacentBees == BEE)
                    continue;

                int count = CountBeesAround(row, col, gameState);
                gameState.board[row, col].adjacentBees = count;
            }
        }
    }

    int CountBeesAround(int row, int col, GameState gameState)
    {
        int count = 0;
        var directions = gameState.GetDirections(col);

        foreach (var (rowOffset, colOffset) in directions)
        {
            int newRow = row + rowOffset;
            int newCol = col + colOffset;
            if (gameState.IsValidHex(newRow, newCol) &&
                gameState.board[newRow, newCol].adjacentBees == BEE)
            {
                count++;
            }
        }

        return count;
    }

    
}
