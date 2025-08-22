using UnityEngine;
using UnityEngine.InputSystem;

public class HexGrid : MonoBehaviour
{
    [field: SerializeField] public int Width { get; private set; } = 10;
    [field: SerializeField] public int Height { get; private set; } = 10;
    [field: SerializeField] public float HexSize { get; private set; } = 10;
    [field: SerializeField] public GameObject HexPrefab { get; private set; }
    [field: SerializeField] public HexDirection Direction { get; private set; } = HexDirection.FlatTop;


    private void OnDrawGizmos()
    {
        for (int z = 0; z < Height; z++)
        {
            for (int x = 0; x < Width; x++)
            {
                Vector3 center = HexGridHelper.Center(HexSize, x, z, Direction) + transform.position;
                Gizmos.color = Color.green;
                
                Vector3[] corners = HexGridHelper.Corners(HexSize, Direction);
                for (int i = 0; i < corners.Length; i++)
                {
                    Gizmos.DrawLine(
                        center + corners[i % 6],
                        center + corners[(i+1) % 6]
                        );
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