using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystem<T>
{
    int width;
    int height;
    int cellSize;
    Vector3 origin;
    public T[,] gridArray;

    public GridSystem(int height, int width, int cellSize)
    {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = new Vector3(-width * cellSize / 2f, 0, -height * cellSize / 2f);

        gridArray = new T[width, height];
    }

    public GridSystem(int height, int width, int cellSize, Vector3 origin) {
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.origin = origin;

        gridArray = new T[width, height];
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, 0, y) * cellSize + origin;
    }

    public Vector2Int GetXY(Vector3 worldPosition)
    {
        return new Vector2Int(Mathf.FloorToInt((worldPosition-origin).x / cellSize), Mathf.FloorToInt((worldPosition - origin).z / cellSize));
    }

    public void SetValue(int x, int y, T value)
    {
        if (x>0 && y>0 && x<width && y<height)
        {
            gridArray[x, y] = value;
        }
    }
    public void SetValue(Vector3 worldPosition, T value)
    {
        Vector2Int gridXY = GetXY(worldPosition);
        if (gridXY.x > 0 && gridXY.y > 0 && gridXY.x < width && gridXY.y < height)
        {
            gridArray[gridXY.x, gridXY.y] = value;
        }
    }
}
