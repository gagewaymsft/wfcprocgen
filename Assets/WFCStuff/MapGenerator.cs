using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;
using Diag = System.Diagnostics;

[ExecuteAlways]
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

    // VARIABLES FOR INIT SPIRAL TILEMAP GENERATION
    public int x, y = -10;
    public int step = 1;
    public int stepSize = 0; // set to reference tilemap width
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
        List<GameObject> tileMaps = CheckTileMaps();

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
            CreateSpiralTileMapStartingPlot(lastTileMapCreated);
        }
        else
        {
            // just for now, eat it
        }
    }

    //private void FreshStart()
    //{
    //    referenceTileMap = GameObject.FindGameObjectWithTag("ReferenceTileMap");
    //    referenceTileMap.GetComponent<TilePainter>().palette = null;
    //    referenceTileMapWidth = referenceTileMap.GetComponent<TilePainter>().width;
    //    referenceTileMapDepth = referenceTileMap.GetComponent<TilePainter>().height;
    //    referenceTileMapGridSize = referenceTileMap.GetComponent<TilePainter>().gridsize;
    //    referenceTileMapPositionX = referenceTileMap.GetComponent<TilePainter>().transform.position.x;
    //    referenceTileMapPositionY = referenceTileMap.GetComponent<TilePainter>().transform.position.y;


    //    GameObject tileField = referenceTileMap.transform.Find("tiles").gameObject;

    //    foreach (Transform child in tileField.transform)
    //    {
    //        tilesToTrainFrom.Add(child.gameObject);
    //    }

    //    CreateFirstTileMap();
    //}

    private void Update()
    {

        if (stopGenerating is false)
        {
            if (!hasCreatedAnyTileMaps)
            {
                Initialize();
            }
            if (!hasStarted)
            {
                GenerateTilemaps();
            }

            timespan = stopwatch?.Elapsed.TotalMilliseconds ?? 0;
            //List<GameObject> tileMaps = CheckTileMaps();

            //if (ShouldDestroyTiles(tileMaps))
            //{
            //    DestroyTileField(tileMaps[0]);
            //    GameObject tileField = CreateNewTileField(tileMaps[0]);
            //    ReparentTiles(tileField);
            //    DestroyTaggedObject("TILESTOCOPY");
            //    hasDestroyed = true;
            //}
        }
    }

    private IEnumerator Wait(bool isPaused)
    {
        yield return new WaitForSeconds(0.5f);

    }

    private List<GameObject> CheckTileMaps()
    {
        return tileMaps ?? new List<GameObject>();
    }

    private void CreateSpiralTileMapStartingPlot(GameObject lastTileMap)
    {
        if (tileMaps.Count == maxTileMapsCount)
        {
            return;
        }
        var tileMapToCreate = tileMapGenerator.GenerateTileMap();
        tileMapToCreate.transform.parent = transform;
        Vector3 nextLocation = GetNextTileMapPosition(lastTileMap.transform);
        tileMapToCreate.transform.position = nextLocation;

        tileMapToCreate = DoTileShit(tileMapToCreate);


        tileMaps.Add(tileMapToCreate);
        lastTileMapCreated = tileMapToCreate;
        hasCreatedAnyTileMaps = true;
        // recursively call this method to create the next tilemap
        CreateSpiralTileMapStartingPlot(tileMapToCreate);
    }

    public GameObject DoTileShit(GameObject tileMapToCreate)
    {
        GameObject tileField = new GameObject("tiles");
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


        GameObject output = tileMapToCreate.transform.Find("output") == null
            ? Utils.GenerateGameObject("output")
            : tileMapToCreate.transform.Find("output").gameObject;

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

    public Vector3 GetNextTileMapPosition(Transform lastTilemapLocation)
    {
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

    private void CreateNorthTileMap()
    {
        var tileMapToCreate = tileMapGenerator.GenerateTileMap();
        tileMapToCreate.transform.parent = transform;

        Debug.Log("Creating North Tile Map");

        // get last created tilemap
        var lastTileMap = lastTileMapCreated;
        // get the last tilemap position
        var lastTileMapPosition = lastTileMap.transform.position;
        // get the reference tilemap width
        var tilemapwidth = referenceTileMapWidth;
        // get the reference tilemap depth
        var tilemapdepth = referenceTileMapDepth;
        // get the reference tilemap gridsize
        var tilemapgridsize = gridSize > 0 ? gridSize : referenceTileMapGridSize > 0 ? referenceTileMapGridSize : 1;

        // figure out how to position the new tilemap

        // going clockwise

        // get last tilemap direction

        //Transform nextLocation = GetNextTileMapPosition(lastTileMap.transform, 09);
        ;




        // get all the tiles on the last created tilemap



        // feed the tiles into the wfc algorithm



        // get the output tiles from the wfc algorithm



        // append the output tiles to the new tilemap



        // add tilemap to list
        tileMaps.Add(tileMapToCreate);
        //set last tilemap created to this one
        lastTileMapCreated = tileMapToCreate;
    }

    private void CreateEastTileMap()
    {
        var tileMapToCreate = tileMapGenerator.GenerateTileMap();
        // parent the new tilemap to self
        tileMapToCreate.transform.parent = transform;

        Debug.Log("Creating East Tile Map");

        tileMaps.Add(tileMapToCreate);
        lastTileMapCreated = tileMapToCreate;
    }

    private void CreateSouthTileMap()
    {
        var tileMapToCreate = tileMapGenerator.GenerateTileMap();
        tileMapToCreate.transform.parent = transform;

        Debug.Log("Creating South Tile Map");

        tileMaps.Add(tileMapToCreate);
        lastTileMapCreated = tileMapToCreate;
    }

    private void CreateWestTileMap()
    {
        var tileMapToCreate = tileMapGenerator.GenerateTileMap();
        tileMapToCreate.transform.parent = transform;

        Debug.Log("Creating West Tile Map");

        tileMaps.Add(tileMapToCreate);
        lastTileMapCreated = tileMapToCreate;
    }

    private bool ShouldDestroyTiles(List<GameObject> tileMaps)
    {
        return tileMaps.Count > 0 && GetGeneratedMapOutput().Count > 0 && !hasDestroyed;
    }

    private void DestroyTileField(GameObject tileMap)
    {
        GameObject tileField = tileMap.transform.Find("tiles").gameObject;
        DestroyImmediate(tileField);
    }

    private GameObject CreateNewTileField(GameObject parentTileMap)
    {
        GameObject tileField = new GameObject("tiles");
        tileField.transform.parent = parentTileMap.transform;
        return tileField;
    }

    private void ReparentTiles(GameObject newParent)
    {
        List<GameObject> tiles = GetGeneratedMapOutput();

        foreach (GameObject tile in tiles)
        {
            ReparentAndPositionTile(tile, newParent);
        }
    }

    private void ReparentAndPositionTile(GameObject tile, GameObject newParent)
    {
        tile.transform.parent = newParent.transform;
        tile.transform.position = new Vector3(tile.transform.position.x - chunkWidth, tile.transform.position.y, 0);
    }

    private void DestroyTaggedObject(string tag)
    {
        GameObject objectToDestroy = GameObject.FindGameObjectWithTag(tag);
        DestroyImmediate(objectToDestroy);
    }



    // this is a clusterfuck, but it works for at least the first tilemap
    // need to abstract this out into individual classes and stop relying on "GameObject" for everything
    private void CreateFirstTileMap()
    {
        GameObject tileMapToCreate = tileMapGenerator.GenerateTileMap();
        tileMapToCreate.transform.hierarchyCapacity = 10000;
        tileMapToCreate.transform.parent = transform;
        tileMapToCreate.transform.position = new Vector3(referenceTileMapPositionX, referenceTileMapPositionX + chunkWidth, 0);
        GameObject tileField = new GameObject("tiles");
        tileField.transform.parent = tileMapToCreate.transform;
        tileField.transform.position = new Vector3(referenceTileMapPositionX, referenceTileMapPositionX + chunkWidth, 0);

        foreach (GameObject tile in tilesToTrainFrom)
        {
            GameObject newTile = Instantiate(tile);
            newTile.transform.parent = tileField.transform;
            newTile.transform.position = new Vector3(tile.transform.position.x, tile.transform.position.y + chunkWidth, 0);
        }

        tileField.AddComponent<Training>();
        var tileFieldTraining = tileField.GetComponent<Training>();
        tileFieldTraining.gridsize = referenceTileMapGridSize;
        tileFieldTraining.width = referenceTileMapWidth;
        tileFieldTraining.depth = referenceTileMapDepth;

        var referenceTiles = referenceTileMap.transform.Find("tiles").gameObject;
        var referenceTraining = referenceTiles.GetComponent<Training>();

        tileFieldTraining.tiles = referenceTraining.tiles;
        tileFieldTraining.RS = referenceTraining.RS;

        tileFieldTraining.Compile();

        GameObject output = tileMapToCreate.transform.Find("output") == null
            ? Utils.GenerateGameObject("output")
            : tileMapToCreate.transform.Find("output").gameObject;

        // stupid hack
        output.tag = "TILESTOCOPY";

        output.transform.parent = tileMapToCreate.transform;
        output.transform.position = new Vector3(referenceTileMapPositionX + chunkWidth, referenceTileMapPositionX + chunkWidth, 0);
        output.transform.hierarchyCapacity = 10000;
        output.AddComponent<OverlapWFC>();
        var overlapWFC = output.GetComponent<OverlapWFC>();
        overlapWFC.gridsize = referenceTileMapGridSize;
        overlapWFC.width = referenceTileMapWidth + 2;
        overlapWFC.depth = referenceTileMapDepth + 2;
        overlapWFC.training = tileFieldTraining;
        overlapWFC.N = 3;
        overlapWFC.incremental = true;
        overlapWFC.Generate();

        if (!hasCreatedAnyTileMaps)
        {
            hasCreatedAnyTileMaps = true;
            lastTileMapCreated = tileMapToCreate;
        }

        tileMaps.Add(tileMapToCreate);
    }


    internal List<GameObject> GetGeneratedMapOutput()
    {
        // stupid hack 2.0
        List<GameObject> output = GameObject.FindGameObjectsWithTag("TILESTOCOPY").ToList();
        if (output.Count > 0)
        {
            var outputOverlap = output[0].GetComponent<OverlapWFC>();
            var outputGroup = outputOverlap.group.gameObject;
            List<GameObject> outputTiles = new List<GameObject>();
            foreach (Transform child in outputGroup.transform)
            {
                outputTiles.Add(child.gameObject);
            }

            return outputTiles;
        }

        return new List<GameObject>();
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