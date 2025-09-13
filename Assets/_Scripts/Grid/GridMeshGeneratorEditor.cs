using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridMeshGenerator))]
public class GridMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUILayout.Space(10);

        GridMeshGenerator gridMeshGenerator = (GridMeshGenerator)target;
        if (GUILayout.Button("Create Hex Mesh"))
        {
            gridMeshGenerator.CreateHexMesh();
        }

        if (GUILayout.Button("Clear Hex Mesh"))
        {
            gridMeshGenerator.ClearHexGridMesh();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Square Mesh"))
        {
            gridMeshGenerator.CreateSquareMesh();
        }

        if (GUILayout.Button("Clear Square Mesh"))
        {
            gridMeshGenerator.ClearSquareGridMesh();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Create Cube Grid"))
        {
            gridMeshGenerator.CreateSquarePrefabGrid();
        }

        if (GUILayout.Button("Clear Existing Cube Grid"))
        {
            gridMeshGenerator.ClearExistingSquareChildren();
        }
    }
}
