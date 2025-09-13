using UnityEngine;
using UnityEngine.InputSystem;

public class HexGrid : MonoBehaviour
{
    [SerializeField][Range(1, 15)] private int width = 10;
    public int Width
    {
        get => width;
        private set => width = value;
    }
    [SerializeField][Range(1, 15)] private int height = 10;
    public int Height
    {
        get => height;
        private set => height = value;
    }
    [field: SerializeField] public float HexSize { get; private set; } = 10;
    [field: SerializeField] public GameObject HexPrefab { get; private set; }
    [field: SerializeField] public HexDirection Direction { get; private set; } = HexDirection.FlatTop;
    [SerializeField] private bool drawGizmos = true;
    [field: SerializeField] public bool drawPositions { get; private set; } = true;
    private int lastHexWidth, lastHexHeight;
    private float lastHexSize;
    private Vector3[] hexCorners;
    private HexDirection lastHexDirection;
    private const int HEX_SIDES = 6;

    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        if (lastHexDirection != Direction ||
            lastHexWidth != Width ||
            lastHexHeight != Height ||
            lastHexSize != HexSize ||
            hexCorners == null)
        {
            lastHexDirection = Direction;
            lastHexWidth = Width;
            lastHexHeight = Height;
            lastHexSize = HexSize;
            UpdateHexGrid();
        }

        for (int i = 0; i < hexCorners.Length - 1; i++)
        {
            if ((i + 1) % HEX_SIDES == 0)
            {
                Gizmos.DrawLine(hexCorners[i], hexCorners[i - (HEX_SIDES-1)]); // close the hexagon
            }
            else
            {
                Gizmos.DrawLine(hexCorners[i], hexCorners[i + 1]);
            }
        }
    }

    private void UpdateHexGrid()
    {
        hexCorners = new Vector3[HEX_SIDES * Width * Height + 1];
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector3 center = GridHelper.HexCenter(HexSize, x, z, Direction) + transform.position;
                Gizmos.color = Color.green;

                Vector3[] corners = GridHelper.HexCorners(HexSize, Direction);
                for (int i = 0; i < corners.Length; i++)
                {
                    hexCorners[(z * Width * HEX_SIDES) + (x * HEX_SIDES) + i] = center + corners[i];
                    
                }
            }
        }
    }
}

public enum HexDirection
{
    FlatTop,
    PointyTop
}