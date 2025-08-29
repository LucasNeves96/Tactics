
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class GridMeshGenerator : MonoBehaviour
{

    [field: SerializeField] public LayerMask gridLayer { get; private set; }
    [field: SerializeField] public HexGrid hexGrid { get; private set; }

    private void Awake()
    {
        if (hexGrid == null) hexGrid = GetComponentInParent<HexGrid>();
        if (hexGrid == null) Debug.LogError("HexGrid not found in the scene.");
    }

    public void ClearHexGridMesh()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
            return;
        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshCollider>().sharedMesh.Clear();
    }

    public void CreateHexMesh()
    {
        CreateHexMesh(hexGrid.Width, hexGrid.Height, hexGrid.HexSize, hexGrid.Direction, gridLayer);
    }

    public void CreateHexMesh(HexGrid hexGrid, LayerMask layerMask)
    {
        this.hexGrid = hexGrid;
        this.gridLayer = layerMask;
        CreateHexMesh(hexGrid.Width, hexGrid.Height, hexGrid.HexSize, hexGrid.Direction, layerMask);
    }

    public void CreateHexMesh(int width, int height, float hexSize, HexDirection direction, LayerMask layerMask)
    {
        ClearHexGridMesh();
        Vector3[] vertices = new Vector3[7 * width * height];

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 center = GridHelper.HexCenter(hexSize, x, z, direction);
                vertices[(x + (z * width)) * 7] = center;
                for (int s = 0; s < GridHelper.HexCorners(hexSize, direction).Length; s++)
                {
                    vertices[(x + (z * width)) * 7 + 1 + s] = center + GridHelper.HexCorners(hexSize, direction)[s % 6];
                }
            }
        }

        int[] triangles = new int[6 * 3 * width * height];
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                for (int s = 0; s < 6; s++)
                {
                    int cornerIndex = (s + 2) > 6 ? s + 2 - 6 : s + 2;
                    triangles[3 * 6 * (z * width + x) + 3 * s + 0] = (x + (z * width)) * 7;
                    triangles[3 * 6 * (z * width + x) + 3 * s + 1] = (x + (z * width)) * 7 + 1 + s;
                    triangles[3 * 6 * (z * width + x) + 3 * s + 2] = (x + (z * width)) * 7 + cornerIndex;
                }
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "HexMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        mesh.RecalculateUVDistributionMetrics();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        int gridLayerIndex = GetLayerIndex(layerMask);
        Debug.Log($"Grid Layer Index: {gridLayerIndex}");

        gameObject.layer = gridLayerIndex;
    }

    private int GetLayerIndex(LayerMask layerMask)
    {
        int layer = layerMask.value;
        int layerIndex = 0;
        while (layer > 1)
        {
            layer = layer >> 1;
            layerIndex++;
        }
        return layerIndex;
    }
}
