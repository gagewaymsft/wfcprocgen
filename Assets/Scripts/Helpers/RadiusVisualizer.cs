using UnityEngine;

// ripped from https://www.loekvandenouweland.com/content/use-linerenderer-in-unity-to-draw-a-circle.html
public static class RadiusVisualizer
{
    public static void DrawCircle(this GameObject container, float radius, float lineWidth, Color color)
    {
        var segments = 360;
        var line = container.AddComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.startWidth = lineWidth;
        line.endWidth = lineWidth;
        line.positionCount = segments + 1;
        var pointCount = segments + 1; // add extra point to make startpoint and endpoint the same to close the circle
        var points = new Vector3[pointCount];

        for (int i = 0; i < pointCount; i++)
        {
            var rad = Mathf.Deg2Rad * (i * 360f / segments);
            points[i] = new Vector3(Mathf.Sin(rad) * radius, 0, Mathf.Cos(rad) * radius);
        }

        line.SetPositions(points);
        line.material.color = color;
        line.sortingLayerName = "UI";
        container.transform.Rotate(90f, 0, 0);

    }

    public static void RemoveCircle(this GameObject container)
    {
        Object.Destroy(container);
    }
}