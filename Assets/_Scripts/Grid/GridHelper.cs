using System.IO.Compression;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class GridHelper
{
    #region Hex Helpers
    public static float OuterRadius(float hexSize)
    {
        return hexSize;
    }

    public static float InnerRadius(float hexSize)
    {
        return hexSize * Mathf.Sqrt(3) / 2;
    }

    public static Vector3 HexToWorldPosition(int q, int r, float hexSize, HexDirection direction)
    {
        float x = hexSize * (3f / 2f * q);
        float y = hexSize * (Mathf.Sqrt(3) * (r + q / 2f));

        if (direction == HexDirection.PointyTop)
        {
            return new Vector3(x, 0, y);
        }
        else // FlatTop
        {
            return new Vector3(x, y, 0);
        }
    }

    public static Vector3 HexCorner(float hexSize, HexDirection direction, int index)
    {
        float angle = 60f * index;
        if (direction == HexDirection.PointyTop)
        {
            angle += 30f; // Offset for pointy top hexes
        }
        Vector3 corner = new Vector3(
            hexSize * Mathf.Cos(angle * Mathf.Deg2Rad),
            0,
            hexSize * Mathf.Sin(angle * Mathf.Deg2Rad)
        );
        return corner;
    }

    public static Vector3[] HexCorners(float hexSize, HexDirection direction)
    {
        Vector3[] corners = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            corners[i] = HexCorner(hexSize, direction, i);
        }
        return corners;
    }

    public static Vector3 HexCenter(float hexSize, int x, int z, HexDirection direction)
    {
        Vector3 centerPosition;
        if (direction == HexDirection.PointyTop)
        {
            centerPosition.x = (x + (z * 0.5f) - (z / 2)) * InnerRadius(hexSize) * 2f;
            centerPosition.y = 0;
            centerPosition.z = z * (OuterRadius(hexSize) * 1.5f);
        }
        else
        {
            centerPosition.x = x * OuterRadius(hexSize) * 1.5f;
            centerPosition.y = 0;
            centerPosition.z = (z + (x * 0.5f) - (x / 2)) * InnerRadius(hexSize) * 2f;
        }
        return centerPosition;
    }

    public static Vector3 HexOffsetToCube(int col, int row, HexDirection orientation)
    {
        if (orientation == HexDirection.PointyTop)
        {
            return HexAxialToCube(OffsetToAxialPointy(col, row));
        }
        else
        {
            return HexAxialToCube(OffsetToAxialFlat(col, row));
        }
    }

    public static Vector3 HexAxialToCube(Vector2Int axial)
    {
        float x = axial.x;
        float z = axial.y;
        float y = -x - z;

        return new Vector3(x, z, y);
    }

    public static Vector2Int OffsetToAxialFlat(int col, int row)
    {
        int q = col;
        int r = row - (col + (col & 1)) / 2;

        return new Vector2Int(q, r);
    }

    public static Vector2Int OffsetToAxialPointy(int col, int row)
    {
        int q = col - (row + (row & 1)) / 2;
        int r = row;
        return new Vector2Int(q, r);
    }

    #endregion

    #region Square Helpers

    public static Vector3 SquareCenter(float squareSize, int iX, int iZ)
    {
        float centerX = (iX + 0.5f) * squareSize;
        float centerZ = (iZ + 0.5f) * squareSize;
        return new Vector3(centerX, 0, centerZ);
    }

    public static Vector3[] SquareCorners(float squareSize)
    {
        float halfSize = squareSize / 2;
        return new Vector3[]
        {
            new Vector3(-halfSize, 0, -halfSize),
            new Vector3(halfSize, 0, -halfSize),
            new Vector3(halfSize, 0, halfSize),
            new Vector3(-halfSize, 0, halfSize)
        };
    }
    public static Vector3 SquareOffsetToCube(int col, int row)
    {
        return SquareAxialToCube(new Vector2Int(col, row));
    }

    public static Vector3 SquareAxialToCube(Vector2Int axial)
    {
        float x = axial.x;
        float z = axial.y;
        float y = - x - z;

        return new Vector3(x, z, y);
    }

    #endregion
}
