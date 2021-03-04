using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GridDisplay : MonoBehaviour
{
    public int width;
    public int height;
    public int cellSize;
    public bool showGrid;

    GridSystem<bool> grid;

    private void OnDrawGizmos()
    {        
        
    }

    private void Update()
    {
        if (!showGrid) return;

        grid = new GridSystem<bool>(width, height, cellSize, new Vector3(-width * cellSize / 2f, 0, -height * cellSize / 2f));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x + 1, y));
                Debug.DrawLine(grid.GetWorldPosition(x, y), grid.GetWorldPosition(x, y + 1));
            }
        }
    }


}
