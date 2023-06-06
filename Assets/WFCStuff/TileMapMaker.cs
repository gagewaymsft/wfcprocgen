using System;
using System.Collections.Generic;
using Diag = System.Diagnostics;
using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class TileMapMaker : MonoBehaviour
{

    public List<GameObject> tileMaps = null;
    public bool hasCreatedAnyTileMaps = false;

    public List<TileChunk> tileChunks = new();

    public GameObject referenceTileMap;

    public GameObject lastTileMapCreated;
    public int lastCreatedTileMapDirection = 0;

    public List<GameObject> tilesToTrainFrom = new();
    public int referenceTileMapGridSize = 0;

    public int referenceTileMapWidth = 0;
    public int referenceTileMapDepth = 0;

    public float referenceTileMapPositionX = 0;
    public float referenceTileMapPositionY = 0;

    public int chunkWidth = 20;
    public int gridSize = 1;

    public float referenceTileMapTileScale = 0;
    public bool hasDestroyed = false;

    private Diag.Stopwatch stopwatch = new();
    public double timespan;
    public int maxTileMapsCount = 30;

    void Start()
    {
        // start stopwatch
        stopwatch.Start();
        GenerateClockwise();
    }



    internal void GenerateClockwise()
    {
        bool freshStart = tileMaps.Count == 0 && hasCreatedAnyTileMaps == false;

        if (freshStart)
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

            CreateFirstTileMap();
        }
        else
        {
            List<GameObject> tileMaps = CheckTileMaps();

            if (tileMaps.Count < maxTileMapsCount)
            {
                GameObject lastTileMap = tileMaps[tileMaps.Count - 1];

                hasCreatedAnyTileMaps = true;
                referenceTileMap = lastTileMap;

                // creating them clockwise will eventually cause issues, but for now i just want to generate a few maps to prove the concept
                switch (lastCreatedTileMapDirection)
                {
                    case 0:
                        CreateEastTileMap();
                        lastCreatedTileMapDirection = 1;
                        break;
                    case 1:
                        CreateSouthTileMap();
                        lastCreatedTileMapDirection = 2;
                        break;
                    case 2:
                        CreateWestTileMap();
                        lastCreatedTileMapDirection = 3;
                        break;
                    case 3:
                        CreateNorthTileMap();
                        lastCreatedTileMapDirection = 0;
                        break;
                }
            }
            else
            {
                // just for now
                Debug.Log("Max tile maps reached");
            }
        }
    }

    private void Update()
    {
        GenerateClockwise();
        timespan = stopwatch.Elapsed.TotalMilliseconds;
        List<GameObject> tileMaps = CheckTileMaps();

        if (ShouldDestroyTiles(tileMaps))
        {
            DestroyTileField(tileMaps[0]);
            GameObject tileField = CreateNewTileField(tileMaps[0]);
            ReparentTiles(tileField);
            DestroyTaggedObject("TILESTOCOPY");
            hasDestroyed = true;
        }
    }

    private List<GameObject> CheckTileMaps()
    {
        return tileMaps ?? new List<GameObject>();
    }

    private void CreateNorthTileMap()
    {
        var tileMapToCreate = GenerateGameObject("GeneratedTileMap");
        Debug.Log("Creating North Tile Map");

        // add tilemap to list
        tileMaps.Add(tileMapToCreate);
        //set last direction to north
        lastCreatedTileMapDirection = 0;
        //set last tilemap created to this one
        lastTileMapCreated = tileMapToCreate;
    }


    private void CreateEastTileMap()
    {
        var tileMapToCreate = GenerateGameObject("GeneratedTileMap");
        Debug.Log("Creating East Tile Map");

        tileMaps.Add(tileMapToCreate);
        lastCreatedTileMapDirection = 1;
        lastTileMapCreated = tileMapToCreate;
    }

    private void CreateSouthTileMap()
    {
        var tileMapToCreate = GenerateGameObject("GeneratedTileMap");
        Debug.Log("Creating South Tile Map");

        tileMaps.Add(tileMapToCreate);
        lastCreatedTileMapDirection = 2;
        lastTileMapCreated = tileMapToCreate;
    }

    private void CreateWestTileMap()
    {
        var tileMapToCreate = GenerateGameObject("GeneratedTileMap");
        Debug.Log("Creating West Tile Map");

        tileMaps.Add(tileMapToCreate);
        lastCreatedTileMapDirection = 3;
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

    private GameObject GenerateGameObject(string namePrefix)
    {
        GameObject toCreate = new();
        Guid guid = Guid.NewGuid();
        toCreate.name = $"{namePrefix}-" + guid.ToString();
        return toCreate;
    }


    // this is a clusterfuck, but it works for at least the first tilemap
    // need to abstract this out into individual classes and stop relying on "GameObject" for everything
    private void CreateFirstTileMap()
    {
        GameObject tileMapToCreate = GenerateGameObject("GeneratedTileMap");
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
            ? GenerateGameObject("output")
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
            lastCreatedTileMapDirection = 0;
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

[CustomEditor(typeof(TileMapMaker))]
public class TileMapMakerGenerator : Editor
{
    public override void OnInspectorGUI()
    {
        TileMapMaker generator = (TileMapMaker)target;

        if (GUILayout.Button("Generate Clockwise"))
        {
            generator.GenerateClockwise();
        }

        if (GUILayout.Button("Reset"))
        {
            generator.hasCreatedAnyTileMaps = false;
            generator.tileMaps = new List<GameObject>();
            generator.tilesToTrainFrom.Clear();
            generator.hasDestroyed = false;
            foreach (Transform child in generator.transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }

        if (GUILayout.Button("Get Output Tilemaps"))
        {
            generator.GetGeneratedMapOutput();
        }

        DrawDefaultInspector();
    }
}