using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TileSample
{
    public TileBase[,] tiles;
    public List<Vector3> tilePositions;
    public Vector2Int windowPosition; // The position of your window
    public Vector2Int windowSize; // The size of your window
    public TileSample(TileBase[,] tiles, List<Vector3> tilePositions, Vector2Int windowPosition, Vector2Int windowSize)
    {
        this.tiles = tiles;
        this.tilePositions = tilePositions;
        this.windowPosition = windowPosition;
        this.windowSize = windowSize;
    }
}
