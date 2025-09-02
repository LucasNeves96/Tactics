using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SquareGrid : MonoBehaviour
{
    [field: SerializeField] public int Width { get; private set; } = 10;
    [field: SerializeField] public int Height { get; private set; } = 10;
    [field: SerializeField] public float SquareSize { get; private set; } = 10;
    [field: SerializeField] public GameObject SquarePrefab { get; private set; }
    [SerializeField] private bool drawGizmos = true;
    [field: SerializeField] public bool drawPositions { get; private set; } = true;
    private int lastSquareWidth, lastSquareHeight;
    private float lastSquareSize;
    private Vector3[] squareCorners;
    private const int SQUARE_SIDES = 4;


    private void OnDrawGizmos()
    {
        if (!drawGizmos) return;

        if (CheckLastValuesChange() || squareCorners == null)
        {
            UpdateLastValuesWithNewValues();
            UpdateSquareGrid();
        }
        DrawGizmosFromCorners(squareCorners);
    }

    private void UpdateSquareGrid()
    {
        Vector3[] corners = GridHelper.SquareCorners(SquareSize);
        squareCorners = new Vector3[(SQUARE_SIDES * Width * Height) + 1];
        for (int iZ = 0; iZ < Height; iZ++)
        {
            for (int iX = 0; iX < Width; iX++)
            {
                for (int i = 0; i < corners.Length; i++)
                {
                    Vector3 center = GridHelper.SquareCenter(SquareSize, iX, iZ) + transform.position;
                    squareCorners[(iZ * Width * SQUARE_SIDES) + (SQUARE_SIDES * iX) + i] = center + corners[i];
                }
            }
        }
    }

    private bool CheckLastValuesChange()
    {
        if (lastSquareWidth != Width ||
            lastSquareHeight != Height ||
            lastSquareSize != SquareSize)
        {
            return true;
        }
        return false;
    }

    private void UpdateLastValuesWithNewValues()
    {
        lastSquareWidth = Width;
        lastSquareHeight = Height;
        lastSquareSize = SquareSize;
    }

    private void DrawGizmosFromCorners(Vector3[] corners)
    {
        Gizmos.color = Color.softRed;
        for (int i = 0; i < corners.Length - 1; i++)
        {
            if ((i + 1) % SQUARE_SIDES == 0)
            {
                Gizmos.DrawLine(corners[i], corners[i - (SQUARE_SIDES - 1)]); // close the square
            }
            else
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }
        }
    }
}
