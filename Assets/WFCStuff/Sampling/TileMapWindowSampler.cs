using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
// organize tilesampler, make a couple data structures for it in case i need them (stfu yagni you pussy bitch) 

public class TileMapWindowSampler : MonoBehaviour
{
    public Tilemap tilemap; // Your seed tilemap
    public Tilemap outlineTilemap; // The tilemap for the outline, which is needed as a separate tilemap
    public TileBase redOutlineTile;
    public TileBase blueOutlineTile;
    public Vector2Int windowSize;
    public Vector2Int windowPosition;
    public int samplerStepSize;
    public List<Vector3Int> redTilePositions = new();
    public List<Vector3Int> blueTilePositions = new();
    public SampleSetQueue sampleSetQueue = new(25);


    public void AnalyzeAndSampleTilesForGeneration()
    {
        TileSampleSet tileSampleSet = CreateTileSampleSet();
        sampleSetQueue.AddSample(tileSampleSet);
        Debug.Log("Analyzing and sampling tiles for generation...");
        // do something with the tile sample set
    }

    // This method returns a list of tiles inside the given window
    public TileBase[,] GetTilesInWindow()
    {
        TileBase[,] tiles = new TileBase[windowSize.x, windowSize.y];
        for (int x = 0; x < windowSize.x; x++)
        {
            for (int y = 0; y < windowSize.y; y++)
            {
                Vector3Int gridPosition = new(windowPosition.x + x, windowPosition.y + y, 0);
                TileBase tileCandidate = tilemap.GetTile(gridPosition);
                tiles[x, y] = tileCandidate;
            }
        }

        return tiles;
    }

    public TileSampleSet CreateTileSampleSet()
    {
        TileBase[,] sampleTiles = new TileBase[windowSize.x, windowSize.y];
        List<Vector3> sampleTilePositions = new();
        TileBase[,] emptyTiles = new TileBase[windowSize.x, windowSize.y];
        List<Vector3> emptyTilePositions = new();
        for (int x = windowPosition.x; x < windowPosition.x + windowSize.x; x++)
        {
            for (int y = windowPosition.y; y < windowPosition.y + windowSize.y; y++)
            {
                Vector3Int gridPosition = new(x, y, 0);
                if (tilemap.GetTile(gridPosition) == null)
                {
                    // set red temporary tile
                    redTilePositions.Add(gridPosition);
                    emptyTilePositions.Add(tilemap.CellToWorld(gridPosition));
                    // set emptyTiles to current tile
                    emptyTiles[x - windowPosition.x, y - windowPosition.y] = tilemap.GetTile(gridPosition);
                    tilemap.SetTile(gridPosition, redOutlineTile);
                }
                else
                {
                    // set blue temporary tile, this implies that the blue tile is a tile that will be sampled
                    sampleTilePositions.Add(gridPosition);
                    blueTilePositions.Add(gridPosition);
                    // set sampleTiles to current tile
                    sampleTiles[x - windowPosition.x, y - windowPosition.y] = tilemap.GetTile(gridPosition);
                }
            }
        }

        return TileSampleSet.CreateSampleSet(sampleTiles, emptyTiles, sampleTilePositions, emptyTilePositions, windowPosition, windowSize);
    }

    // For now, creates a box of blue tiles
    public void ApplyGeneratedTiles()
    {
        // temporary until we have tiles from the generator
        for (int x = windowPosition.x; x < windowPosition.x + windowSize.x; x++)
        {
            for (int y = windowPosition.y; y < windowPosition.y + windowSize.y; y++)
            {
                Vector3Int gridPosition = new(x, y, 0);
                // check if tile is red tile
                if (tilemap.GetTile(gridPosition) == redOutlineTile)
                {
                    tilemap.SetTile(gridPosition, blueOutlineTile);
                    blueTilePositions.Add(gridPosition);
                }
            }
        }
    }

    public void UpdateOutline()
    {
        // Clear previous outline
        outlineTilemap.ClearAllTiles();

        // Create new outline
        for (int x = windowPosition.x - 1; x <= windowPosition.x + windowSize.x; x++)
        {
            for (int y = windowPosition.y - 1; y <= windowPosition.y + windowSize.y; y++)
            {
                bool isBorderTile = x < windowPosition.x || y < windowPosition.y || x >= windowPosition.x + windowSize.x || y >= windowPosition.y + windowSize.y;
                if (isBorderTile)
                {
                    Vector3Int gridPosition = new(x, y, 0);
                    outlineTilemap.SetTile(gridPosition, redOutlineTile);
                }
            }
        }
    }

    private void Start()
    {
        UpdateWindowAndSample();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveWindow(0);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveWindow(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveWindow(2);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveWindow(3);
        }
    }

    public void MoveWindowUp()
    {
        windowPosition.y += samplerStepSize;
    }

    public void MoveWindowDown()
    {
        windowPosition.y -= samplerStepSize;
    }

    public void MoveWindowLeft()
    {
        windowPosition.x -= samplerStepSize;
    }

    public void MoveWindowRight()
    {
        windowPosition.x += samplerStepSize;
    }

    // Move window to the given position
    public void MoveWindowToSpecificLocation(Vector2Int position)
    {
        windowPosition = position;
        UpdateWindowAndSample();
    }

    public void MoveWindow(int direction)
    {
        ApplyGeneratedTiles();
        switch (direction)
        {
            case 0:
                MoveWindowUp();
                break;
            case 1:
                MoveWindowDown();
                break;
            case 2:
                MoveWindowLeft();
                break;
            case 3:
                MoveWindowRight();
                break;
        }

        UpdateWindowAndSample();
    }

    public void UpdateWindowAndSample()
    {
        UpdateOutline();
        AnalyzeAndSampleTilesForGeneration();
    }

    public void StartExternal()
    {
        Start();
    }

    public void Reset()
    {

        windowPosition = new Vector2Int(0, 0);

        foreach (Vector3Int position in redTilePositions)
        {
            // check if tile is red tile
            if (tilemap.GetTile(position) == redOutlineTile)
            {
                tilemap.SetTile(position, null);
            }
        }

        foreach (Vector3Int position in blueTilePositions)
        {
            // check if tile is blue tile
            if (tilemap.GetTile(position) == blueOutlineTile)
            {
                tilemap.SetTile(position, null);
            }
        }
        redTilePositions.Clear();
        blueTilePositions.Clear();
        sampleSetQueue.Clear();
        StartExternal();
    }

    #region Code Graveyard

    //(TileBase[,], List<Vector3>) CleanReorderTileSample(TileBase[,] tilesToReoder, List<Vector3> tilePositionListToReorder)
    //{
    //    // reorder tilePositions to match order of tilesInWindow
    //    tilePositionListToReorder = tilePositionListToReorder.OrderBy(tilePosition => tilePosition.x).ThenBy(tilePosition => tilePosition.y).ToList();
    //    // reorder tilesInWindow to match order of tilePositions
    //    TileBase[,] reorderedTilesInWindow = new TileBase[windowPosition.x, windowPosition.y];
    //    for (int x = 0; x < windowSize.x; x++)
    //    {
    //        for (int y = 0; y < windowSize.y; y++)
    //        {
    //            Vector3Int gridPosition = new Vector3Int(windowPosition.x + x, windowPosition.y + y, 0);
    //            Vector3 tilePosition = tilemap.CellToWorld(gridPosition);
    //            int index = tilePositionListToReorder.IndexOf(tilePosition);
    //            reorderedTilesInWindow[x, y] = tilesToReoder[index % windowSize.x, index / windowSize.x];
    //        }
    //    }

    //    return (reorderedTilesInWindow, tilePositionListToReorder);
    //}


    // This method updates the outline of the window

    // For now, creates a box of red tiles
    #endregion
}
