using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SquareGrid))]
public class SquareGridEditor : Editor
{
    private SquareGrid squareGrid => target as SquareGrid;
    void OnSceneGUI()
    {
        if (!squareGrid || !squareGrid.drawPositions) return;
        for (int z = 0; z < squareGrid.Height; z++)
        {
            for (int x = 0; x < squareGrid.Width; x++)
            {
                Vector3 centrePosition = GridHelper.SquareCenter(squareGrid.SquareSize, x, z) + squareGrid.transform.position;

                int centerX = x;
                int centerZ = z;

                Vector3 cubeCoord = GridHelper.SquareOffsetToCube(centerX, centerZ);
                Handles.color = Color.red;
                Handles.Label(centrePosition + Vector3.forward * squareGrid.SquareSize/4, $"[{centerX}, {centerZ}]", new GUIStyle() { alignment = TextAnchor.MiddleCenter });
                Handles.Label(centrePosition, $"({cubeCoord.x},{cubeCoord.y},{cubeCoord.z})", new GUIStyle() { alignment = TextAnchor.MiddleCenter });
            }
        }
    }
}