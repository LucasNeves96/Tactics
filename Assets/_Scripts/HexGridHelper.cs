using System.IO.Compression;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public static class HexGridHelper
{
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

    public static Vector3 Corner(float hexSize, HexDirection direction, int index)
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

    public static Vector3[] Corners(float hexSize, HexDirection direction)
    {
        Vector3[] corners = new Vector3[6];
        for (int i = 0; i < 6; i++)
        {
            corners[i] = Corner(hexSize, direction, i);
        }
        return corners;
    }

    public static Vector3 Center(float hexSize, int x, int z, HexDirection direction)
    {
        Vector3 centerPosition;
        if (direction == HexDirection.PointyTop)
        {
            centerPosition.x = (x + z * 0.5f - z / 2) * InnerRadius(hexSize) * 2f;
            centerPosition.y = 0;
            centerPosition.z = z * (OuterRadius(hexSize) * 1.5f);
        }
        else
        {
            centerPosition.x = x * OuterRadius(hexSize) * 1.5f;
            centerPosition.y = 0;
            centerPosition.z = (z + x * 0.5f - x / 2) * InnerRadius(hexSize) * 2f;
        }
        return centerPosition;
    }
}
