using System.Collections.Generic;
using UnityEditor;
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

public class TileMapWindowSampler : MonoBehaviour
{
    public Tilemap tilemap; // Your original tilemap
    public Tilemap outlineTilemap; // The tilemap for the outline
    public TileBase redOutlineTile; // The tile for the red outline
    public TileBase blueOutlineTile; // The tile for the blue outline
    public Vector2Int windowSize; // The size of your window
    public Vector2Int windowPosition; // The position of your window
    public int windowStepSize; // The step size of your window
    public TileSample tileSample; // The sample of tiles in the window
    public List<Vector3Int> emptyTilesPositions = new List<Vector3Int>(); // A list of empty tiles created by this window
    public List<Vector3Int> blueTilePositions = new List<Vector3Int>(); // A list of blue tiles created by this window

    private void Start()
    {
        /* This is cute, but it's not solving any problems.
        // if i have a 5x5 window, i need 4 child samplers, each being 4x4, 3x3, 2x2, and 1x1 consecutively
        // this may be drastically overkill.
        // childSamplerCount = windowSize.x - 1;
        // Initialize the child samplers
        // childSamplers = new List<TileMapWindowSampler>();
        // for (int i = childSamplerCount; i > 0; i--)
        // {
        //    // create a new gameobject using Utils.GenerateGameObject, attach it to this gameobject, and add a TileMapWindowSampler component to it
        //    GameObject childSamplerGameObject = Utils.GenerateGameObject("ChildSampler");
        //    childSamplerGameObject.transform.parent = transform;
        //    TileMapWindowSampler childSampler = childSamplerGameObject.AddComponent<TileMapWindowSampler>();
        //    childSampler.isParentSampler = false;
        //    childSampler.windowSize = new Vector2Int(i, i);
        //    childSampler.windowPosition = new Vector2Int(windowSize.x + , windowPosition.y + i);
        //    childSamplers.Add(childSampler);
        // }
         */

        UpdateOutline();
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

    public void MoveWindow(int direction)
    {
        FillWithBlueTile();
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

        UpdateOutline();
        FillEmptyWithRedTile();
        CreateTileSample();
    }

    public void MoveWindowUp()
    {
        windowPosition.y += windowStepSize;
    }

    public void MoveWindowDown()
    {
        windowPosition.y -= windowStepSize;
    }

    public void MoveWindowLeft()
    {
        windowPosition.x -= windowStepSize;
    }

    public void MoveWindowRight()
    {
        windowPosition.x += windowStepSize;
    }

    // Move window to the given position
    public void MoveWindowToSpecificLocation(Vector2Int position)
    {
        windowPosition = position;
        UpdateOutline();
        FillEmptyWithRedTile();
    }

    // This method returns a list of tiles inside the given window
    public TileBase[,] GetTilesInWindow()
    {
        TileBase[,] tiles = new TileBase[windowSize.x, windowSize.y];
        for (int x = 0; x < windowSize.x; x++)
        {
            for (int y = 0; y < windowSize.y; y++)
            {
                Vector3Int gridPosition = new Vector3Int(windowPosition.x + x, windowPosition.y + y, 0);
                TileBase tileCandidate = tilemap.GetTile(gridPosition);
                tiles[x, y] = tileCandidate;
            }
        }

        return tiles;
    }

    public TileSample CreateTileSample()
    {
        TileBase[,] tilesInWindow = GetTilesInWindow();
        List<Vector3> tilePositions = new List<Vector3>();
        // where tilesInWindow is not null, get the real position of the tile and add it to tilePositions
        for (int x = 0; x < windowSize.x; x++)
        {
            for (int y = 0; y < windowSize.y; y++)
            {
                if (tilesInWindow[x, y] != null)
                {
                    Vector3Int gridPosition = new Vector3Int(windowPosition.x + x, windowPosition.y + y, 0);
                    Vector3 tilePosition = tilemap.CellToWorld(gridPosition);
                    tilePositions.Add(tilePosition);
                }
            }
        }

        return new TileSample(tilesInWindow, tilePositions, windowPosition, windowSize);
    }


    // This method updates the outline of the window
    public void UpdateOutline()
    {
        // Clear previous outline
        outlineTilemap.ClearAllTiles();

        // Create new outline
        for (int x = windowPosition.x - 1; x <= windowPosition.x + windowSize.x; x++)
        {
            for (int y = windowPosition.y - 1; y <= windowPosition.y + windowSize.y; y++)
            {
                if (x < windowPosition.x || y < windowPosition.y || x >= windowPosition.x + windowSize.x || y >= windowPosition.y + windowSize.y)
                {
                    Vector3Int gridPosition = new Vector3Int(x, y, 0);
                    outlineTilemap.SetTile(gridPosition, redOutlineTile);
                }
            }
        }
    }

    public void FillEmptyWithRedTile()
    {
        for (int x = windowPosition.x; x < windowPosition.x + windowSize.x; x++)
        {
            for (int y = windowPosition.y; y < windowPosition.y + windowSize.y; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                if (tilemap.GetTile(gridPosition) == null)
                {
                    tilemap.SetTile(gridPosition, redOutlineTile);
                    emptyTilesPositions.Add(gridPosition);
                }
            }
        }
    }

    public void FillWithBlueTile()
    {
        for (int x = windowPosition.x; x < windowPosition.x + windowSize.x; x++)
        {
            for (int y = windowPosition.y; y < windowPosition.y + windowSize.y; y++)
            {
                Vector3Int gridPosition = new Vector3Int(x, y, 0);
                // check if tile is red tile
                if (tilemap.GetTile(gridPosition) == redOutlineTile)
                {
                    tilemap.SetTile(gridPosition, blueOutlineTile);
                    blueTilePositions.Add(gridPosition);
                }
            }
        }

    }

    public void ClearEmptyTiles()
    {
        foreach (Vector3Int position in emptyTilesPositions)
        {
            tilemap.SetTile(position, null);
        }
    }
}


[CustomEditor(typeof(TileMapWindowSampler))]
public class WindowSamplerGui : Editor
{
    public override void OnInspectorGUI()
    {
        TileMapWindowSampler sampler = (TileMapWindowSampler)target;

        if (GUILayout.Button("Create Sample"))
        {
            TileSample sample = sampler.CreateTileSample();
            Debug.Log(sample);
        }

        if (GUILayout.Button("Reset Empty"))
        {
            sampler.ClearEmptyTiles();
        }

        if (GUILayout.Button("Move Up"))
        {
            sampler.MoveWindow(0);
        }

        if (GUILayout.Button("Move Down"))
        {
            sampler.MoveWindow(1);
        }

        if (GUILayout.Button("Move Left"))
        {
            sampler.MoveWindow(2);
        }

        if (GUILayout.Button("Move Right"))
        {
            sampler.MoveWindow(3);
        }

        DrawDefaultInspector();
    }
}