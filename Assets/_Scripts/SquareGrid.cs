using UnityEngine;
using UnityEngine.InputSystem;

public class SquareGrid : MonoBehaviour
{
    [field: SerializeField] public int Width { get; private set; } = 10;
    [field: SerializeField] public int Height { get; private set; } = 10;
    [field: SerializeField] public float SquareSize { get; private set; } = 10;
    [field: SerializeField] public GameObject SquarePrefab { get; private set; }


    private void OnDrawGizmos()
    {
        Vector3[] corners = GridHelper.SquareCorners(SquareSize);
        for (int iZ = 0; iZ < Height; iZ++)
        {
            for (int iX = 0; iX < Width; iX++)
            {
                Vector3 center = GridHelper.SquareCenter(SquareSize, iX, iZ) + transform.position;
                Gizmos.color = Color.softRed;

                for (int i = 0; i < corners.Length; i++)
                {
                    Gizmos.DrawLine(
                        center + corners[i % 4],
                        center + corners[(i + 1) % 4]
                        );
                }
            }
        }
    }
}
