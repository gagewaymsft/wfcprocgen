using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Diagnostics;

public class TileMapGenerator
{

    public GameObject GenerateTileMap()
    {
        GameObject tileMap = Utils.GenerateGameObject("GeneratedTileMap");
        tileMap.AddComponent<TileMap>();
        tileMap.transform.localScale = new Vector3(2, 2, 1);
        tileMap.tag = "GeneratedTileMap";
        return tileMap;
    }
}