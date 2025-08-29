using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMeshGenerator))]
public class GridMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridMeshGenerator gridMeshGenerator = (GridMeshGenerator)target;
        if (GUILayout.Button("Create Hex Mesh"))
        {
            gridMeshGenerator.CreateHexMesh();
        }

        if (GUILayout.Button("Clear Hex Mesh"))
        {
            gridMeshGenerator.ClearHexGridMesh();
        }
    }
}
