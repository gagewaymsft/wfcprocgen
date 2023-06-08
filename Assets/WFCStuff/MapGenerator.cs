using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;
using Diag = System.Diagnostics;

// this is probably not a good idea, but it's the only way I can get the tilemaps to generate in the editor
[ExecuteInEditMode]
public class MapGenerator : MonoBehaviour
{
    public bool stopGenerating = false;
    public bool hasStarted = false;
    private TileMapGenerator tileMapGenerator;
    public List<GameObject> tileMaps;
    public bool hasCreatedAnyTileMaps;

    public GameObject referenceTileMap;
    public GameObject lastTileMapCreated;

    public List<GameObject> tilesToTrainFrom;
    public int referenceTileMapGridSize;

    public int referenceTileMapWidth;
    public int referenceTileMapDepth;

    public float referenceTileMapPositionX;
    public float referenceTileMapPositionY;

    public int chunkWidth;
    public int gridSize;

    public float referenceTileMapTileScale;
    public bool hasDestroyed;

    private Diag.Stopwatch stopwatch;
    public double timespan;
    public int maxTileMapsCount;

    public int x, y = -10;
    public int step = 1;
    public int stepSize = 0;
    public int numSteps = 1;
    public int state = 0;
    public int turnCounter = 1;

    void Start()
    {
        tileMapGenerator = new TileMapGenerator();

        stopwatch.Start();
        GenerateTilemaps();
        Initialize();
    }

    public void Initialize()
    {
        referenceTileMap = GameObject.FindGameObjectWithTag("ReferenceTileMap");
        referenceTileMap.GetComponent<TilePainter>().palette = null;
        referenceTileMapWidth = referenceTileMap.GetComponent<TilePainter>().width;
        referenceTileMapDepth = referenceTileMap.GetComponent<TilePainter>().height;
        referenceTileMapGridSize = referenceTileMap.GetComponent<TilePainter>().gridsize;
        referenceTileMapPositionX = referenceTileMap.GetComponent<TilePainter>().transform.position.x;
        referenceTileMapPositionY = referenceTileMap.GetComponent<TilePainter>().transform.position.y;
        GameObject tileField = referenceTileMap.transform.Find("tiles").gameObject;

        foreach (Transform child in tileField.transform)
        {
            tilesToTrainFrom.Add(child.gameObject);
        }
    }

    public void ResetEverything()
    {
        Time.timeScale = 0.02f;
        tileMaps = new List<GameObject>();
        tileMapGenerator = new TileMapGenerator();
        hasCreatedAnyTileMaps = false;
        lastTileMapCreated = null;
        tilesToTrainFrom = new List<GameObject>();
        referenceTileMapGridSize = 1;
        referenceTileMapWidth = 20;
        referenceTileMapDepth = 20;
        referenceTileMapPositionX = -10;
        referenceTileMapPositionY = -10;
        referenceTileMapTileScale = 0;
        hasDestroyed = false;
        stopwatch = new Diag.Stopwatch();
        timespan = 0;
        x = -10;
        y = -10;
        step = 0;
        stepSize = 0;
        numSteps = 1;
        state = 0;
        turnCounter = 1;
        step = 1;

        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }
        DestroyTaggedObject("GeneratedTileMap");
    }



    internal void GenerateTilemaps()
    {
        List<GameObject> tileMaps = GetOrInitTilemaps();

        if (tileMaps.Count < maxTileMapsCount)
        {
            if (tileMaps.Count == 0)
            {
                lastTileMapCreated = null;
            }


            referenceTileMap = lastTileMapCreated != null ? lastTileMapCreated : referenceTileMap;

            if (lastTileMapCreated == null)
            {
                lastTileMapCreated = referenceTileMap;
            }
            hasStarted = true;
            stepSize = referenceTileMapWidth;
            CreateSpiralTileMapStartingPlot();
        }
        else
        {
            // just for now, eat it
        }
    }


    private void Update()
    {
    }

    private List<GameObject> GetOrInitTilemaps()
    {
        return tileMaps ?? new List<GameObject>();
    }

    private void CreateSpiralTileMapStartingPlot()
    {
        if (tileMaps.Count == maxTileMapsCount)
        {
            return;
        }
        var tileMapToCreate = tileMapGenerator.GenerateTileMap();
        tileMapToCreate.transform.parent = transform;
        Vector3 nextLocation = GetNextTileMapPosition();
        tileMapToCreate.transform.position = nextLocation;

        tileMapToCreate = DoTileShit(tileMapToCreate);


        tileMaps.Add(tileMapToCreate);
        lastTileMapCreated = tileMapToCreate;
        hasCreatedAnyTileMaps = true;

        CreateSpiralTileMapStartingPlot();
    }

    // awesome name i know, will abstract this later
    public GameObject DoTileShit(GameObject tileMapToCreate)
    {
        GameObject tileField = new("tiles");
        tileField.transform.parent = tileMapToCreate.transform;
        tileField.transform.position = tileMapToCreate.transform.position;

        foreach (GameObject tile in tilesToTrainFrom)
        {
            GameObject newTile = Instantiate(tile);
            newTile.transform.parent = tileField.transform;
            newTile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + chunkWidth, 0);
        }

        tileField.AddComponent<Training>();
        Training tileFieldTraining = tileField.GetComponent<Training>();
        tileFieldTraining.gridsize = referenceTileMapGridSize;
        tileFieldTraining.width = referenceTileMapWidth;
        tileFieldTraining.depth = referenceTileMapDepth;

        var referenceTiles = lastTileMapCreated.transform.Find("tiles").gameObject;
        var referenceTraining = referenceTiles.GetComponent<Training>();

        tileFieldTraining.tiles = referenceTraining.tiles;
        tileFieldTraining.RS = referenceTraining.RS;

        tileFieldTraining.Compile();


        GameObject output = tileMapToCreate.transform.Find("output").gameObject ?? Utils.GenerateGameObject("output");

        // stupid hack
        output.tag = "TILESTOCOPY";
        output.transform.parent = tileMapToCreate.transform;

        output.transform.position = tileMapToCreate.transform.position;
        output.AddComponent<OverlapWFC>();
        var overlap = output.GetComponent<OverlapWFC>();
        overlap.gridsize = referenceTileMapGridSize;
        overlap.width = referenceTileMapWidth;
        overlap.depth = referenceTileMapDepth;
        overlap.training = tileFieldTraining;
        overlap.N = 3; // pulled out of my ass
        overlap.incremental = true;
        overlap.Generate(); // this will be wrong probably but oh well fuck it

        return tileMapToCreate;
    }

    public Vector3 GetNextTileMapPosition()
    {
        // this does it in a spiral starting from center
        // this is no longer how i think this should be done
        // but oh well
        switch (state)
        {
            case 0:
                x += stepSize;
                break;
            case 1:
                y -= stepSize;
                break;
            case 2:
                x -= stepSize;
                break;
            case 3:
                y += stepSize;
                break;
        }

        if (step % numSteps == 0)
        {
            state = (state + 1) % 4;
            turnCounter++;
            if (turnCounter % 2 == 0)
            {
                numSteps++;
            }
        }
        step++;
        return new Vector3(x, y, 0);
    }

    private void DestroyTaggedObject(string tag)
    {
        GameObject objectToDestroy = GameObject.FindGameObjectWithTag(tag);
        DestroyImmediate(objectToDestroy);
    }

}

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorGui : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator generator = (MapGenerator)target;

        if (GUILayout.Button("Generate Tilemaps"))
        {
            generator.GenerateTilemaps();
        }

        if (GUILayout.Button("Reset"))
        {
            for (int i = 0; i < 10; i++)
            {
                generator.ResetEverything();

            }
        }

        DrawDefaultInspector();
    }
}