using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class GridMeshGenerator : MonoBehaviour
{

    [field: SerializeField] public LayerMask gridLayer { get; private set; }
    [field: SerializeField] public HexGrid hexGrid { get; private set; }
    [field: SerializeField] public SquareGrid squareGrid { get; private set; }
    [SerializeField] private float amplitude = 0.05f;
    private Dictionary<string, GameObject> cubeInstanceDic = new Dictionary<string, GameObject>();
    private bool isMovingSquares = false;
    private int createdWidth = 0;
    private int createdHeight = 0;
    private float createdSize = 0f;
    private float startTime = 0f;

    private void Awake()
    {
        if (hexGrid == null) hexGrid = GetComponentInParent<HexGrid>();
        if (hexGrid == null) Debug.LogError("HexGrid not found in the scene.");
        if (squareGrid == null) squareGrid = GetComponentInParent<SquareGrid>();
        if (squareGrid == null) Debug.LogError("SquareGrid not found in the scene.");
    }

    public void Update()
    {
        if (isMovingSquares) MoveSquaresSinCos();
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

    public void ClearSquareGridMesh()
    {
        if (GetComponent<MeshFilter>().sharedMesh == null)
            return;
        GetComponent<MeshFilter>().sharedMesh.Clear();
        GetComponent<MeshCollider>().sharedMesh.Clear();
    }

    public void CreateSquarePrefabGrid()
    {
        CreateSquarePrefabGrid(squareGrid.Width, squareGrid.Height, squareGrid.SquareSize, squareGrid.SquarePrefab);
    }

    public void CreateSquarePrefabGrid(int width, int height, float cellSize, GameObject squarePrefab)
    {
        if (squarePrefab == null)
        {
            Debug.LogError("Square Prefab is not assigned.");
            return;
        }

        ClearExistingSquareChildren();
        
        createdHeight = height;
        createdWidth = width;
        createdSize = squareGrid.SquareSize;

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                float cubeToGroundLevel = cellSize / 2;

                Vector3 squareCenter = GridHelper.SquareCenter(cellSize, x, z);
                squareCenter.y += cubeToGroundLevel; // Adjust y to place the cube on the ground
                Quaternion rotation = Quaternion.Euler(-90, 0, 0); // The cube prefab is rotated to lie flat on the ground
                GameObject squareInstance = Instantiate(squarePrefab, squareCenter, rotation, squareGrid.transform);
                squareInstance.name = $"Square_{x}-{z}";
                squareInstance.layer = GetLayerIndex(gridLayer);
                float calculatedScale = cellSize;
                squareInstance.transform.localScale = new Vector3(calculatedScale, calculatedScale, calculatedScale);

                string key = $"{x}-{z}";
                if (!cubeInstanceDic.ContainsKey(key)) cubeInstanceDic.Add(key, squareInstance);
                else Debug.LogWarning($"Square instance at {key} already exists in the dictionary.");
            }
        }
    }

    public void ClearExistingSquareChildren()
    {
        for (int i = squareGrid.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        cubeInstanceDic.Clear();

        createdHeight = 0;
        createdWidth = 0;

        isMovingSquares = false;
    }

    public void ToggleMoveSquaresSin()
    {
        isMovingSquares = !isMovingSquares;
        if (isMovingSquares) startTime = Time.time;
        Debug.Log($"Toggling MoveSquaresSin. Currently moving: {isMovingSquares}");
    }

    public void MoveSquaresSinCos()
    {
        if (cubeInstanceDic.Count == 0)
        {
            Debug.LogWarning("No square instances to move. Creating new ones");
            ClearExistingSquareChildren();
            CreateSquarePrefabGrid();
            isMovingSquares = true;
            startTime = Time.time;
        }
        
        float elapsedTime = Time.time - startTime;

        foreach (var cube in cubeInstanceDic)
        {
            GameObject square = cube.Value;
            Vector3 newPosition = square.transform.position;
            float posToSin = Mathf.Sin((elapsedTime * 2f) + (newPosition.x / createdWidth * 2f * Mathf.PI));
            float posToCos = Mathf.Cos((elapsedTime * 2f) + (newPosition.z / createdHeight * 2f * Mathf.PI));
            float dislocation = amplitude * (posToSin + posToCos);
            newPosition.y = createdSize + dislocation;
            square.transform.position = newPosition;
        }
    }

    public void CreateSquareMesh()
    {
        CreateSquareMesh(squareGrid.Width, squareGrid.Height, squareGrid.SquareSize, gridLayer);
    }

    public void CreateSquareMesh(SquareGrid squareGrid, LayerMask layerMask)
    {
        this.squareGrid = squareGrid;
        this.gridLayer = layerMask;
        CreateSquareMesh(squareGrid.Width, squareGrid.Height, squareGrid.SquareSize, layerMask);
    }

    public void CreateSquareMesh(int width, int height, float cellSize, LayerMask layerMask)
    {
        ClearSquareGridMesh();
        Vector3[] vertices = new Vector3[(width + 1) * (height + 1)];

        for (int z = 0; z <= height; z++)
        {
            for (int x = 0; x <= width; x++)
            {
                vertices[x + (z * (width + 1))] = new Vector3(x * cellSize, 0, z * cellSize);
            }
        }

        int[] triangles = new int[width * height * 6];
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                int squareIndex = x + z * width;
                triangles[squareIndex * 6 + 0] = x + z * (width + 1);
                triangles[squareIndex * 6 + 1] = x + (z + 1) * (width + 1);
                triangles[squareIndex * 6 + 2] = (x + 1) + z * (width + 1);
                triangles[squareIndex * 6 + 3] = (x + 1) + z * (width + 1);
                triangles[squareIndex * 6 + 4] = x + (z + 1) * (width + 1);
                triangles[squareIndex * 6 + 5] = (x + 1) + (z + 1) * (width + 1);
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "SquareMesh";
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();
        mesh.RecalculateUVDistributionMetrics();

        GetComponent<MeshFilter>().sharedMesh = mesh;
        GetComponent<MeshCollider>().sharedMesh = mesh;

        int gridLayerIndex = GetLayerIndex(layerMask);
        gameObject.layer = gridLayerIndex;
    }
}
