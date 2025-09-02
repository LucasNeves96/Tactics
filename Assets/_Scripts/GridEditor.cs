using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    private HexGrid hexGrid => target as HexGrid;
    void OnSceneGUI()
    {
        if (!hexGrid || !hexGrid.drawPositions) return;
        for (int z = 0; z < hexGrid.Height; z++)
        {
            for (int x = 0; x < hexGrid.Width; x++)
            {
                Vector3 centrePosition = GridHelper.HexCenter(hexGrid.HexSize, x, z, hexGrid.Direction) + hexGrid.transform.position;

                int centerX = x;
                int centerZ = z;

                Vector3 cubeCoord = GridHelper.HexOffsetToCube(centerX, centerZ, hexGrid.Direction);
                Handles.color = Color.red;
                Handles.Label(centrePosition + Vector3.forward * 0.5f, $"[{centerX}, {centerZ}]");
                Handles.Label(centrePosition, $"({cubeCoord.x},{cubeCoord.y},{cubeCoord.z})");
            }
        }
    }
}