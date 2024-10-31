using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Painter : MonoBehaviour
{
    [SerializeField] List<Tile> tiles = new List<Tile>();
    [SerializeField] Tilemap tileMap;

    public void PaintTile(int row, int col, int tileValue)
    {
        int index = Mathf.Clamp(tileValue, -3, 6);

        if(index < 0) //Can't use -1 as an index in C#, they use (^) hence below
            index = tiles.Count + index;

        tileMap.SetTile(new Vector3Int(row, col, 0), tiles[index]);
    }
}
