using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileSampleSet
{
    public TileSample sampleTileSample;
    public TileSample emptyTileSample;


    private TileSampleSet(
        TileBase[,] sampleTiles,
        TileBase[,] emptyTiles,
        List<Vector3> sampleTilePositions,
        List<Vector3> emptyTilePositions,
        Vector2Int windowPosition,
        Vector2Int windowSize)
    {
        sampleTileSample = new TileSample(sampleTiles, sampleTilePositions, windowPosition, windowSize);
        emptyTileSample = new TileSample(emptyTiles, emptyTilePositions, windowPosition, windowSize);
    }

    public static TileSampleSet CreateSampleSet(
        TileBase[,] sampleTiles,
        TileBase[,] emptyTiles,
        List<Vector3> sampleTilePositions,
        List<Vector3> emptyTilePositions,
        Vector2Int windowPosition,
        Vector2Int windowSize)
    {
        return new TileSampleSet(sampleTiles, emptyTiles, sampleTilePositions, emptyTilePositions, windowPosition, windowSize);
    }
}
