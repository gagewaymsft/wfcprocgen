using UnityEngine;
using UnityEngine.Tilemaps;

[ExecuteInEditMode]
public class TileMapGenerator : MonoBehaviour
{
    public int MaxIterations = 1000;

    GameObject grid;
    Grid gridComponent;

    GameObject groundMap;
    TilemapRenderer groundMapTileRenderer;
    TileMap groundMapTileMapComponent;

    private void Start()
    {
        // grid object is the first child of the tilemap generator
        grid = transform.GetChild(0).gameObject;
        gridComponent = grid.GetComponent<Grid>();

        // tilemap object is the first child of the grid
        groundMap = grid.transform.GetChild(0).gameObject;
        groundMapTileMapComponent = groundMap.GetComponent<TileMap>();
        groundMapTileRenderer = groundMap.GetComponent<TilemapRenderer>();

    }

    private void Update()
    {

    }

    public GameObject GenerateTile()
    {
        GameObject tileMap = Utils.GenerateGameObject("GeneratedTile");
        tileMap.AddComponent<TileMap>();
        tileMap.tag = "GeneratedTileMap";
        return tileMap;
    }
}