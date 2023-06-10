using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileMapWindowSampler))]
public class WindowSamplerGui : Editor
{
    public override void OnInspectorGUI()
    {
        TileMapWindowSampler sampler = (TileMapWindowSampler)target;

        if (GUILayout.Button("Create Sample"))
        {
            sampler.AnalyzeAndSampleTilesForGeneration();
        }

        if (GUILayout.Button("Reset Empty"))
        {
            sampler.Reset();
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