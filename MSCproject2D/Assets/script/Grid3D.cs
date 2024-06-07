//using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid3D
{
    private int[,,] grid;
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int Depth { get; private set; }

    public Grid3D(int width, int height, int depth)
    {
        Width = width;
        Height = height;
        Depth = depth;
        grid = new int[width, height, depth];
    }

    // Indexer to access the grid using Vector3
    public int this[Vector3 index]
    {
        get
        {
            int x = Mathf.FloorToInt(index.x);
            int y = Mathf.FloorToInt(index.y);
            int z = Mathf.FloorToInt(index.z);
            if (x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth)
            {
                return grid[x, y, z];
            }
            throw new System.IndexOutOfRangeException("Index was outside the bounds of the array.");
        }
        set
        {
            int x = Mathf.FloorToInt(index.x);
            int y = Mathf.FloorToInt(index.y);
            int z = Mathf.FloorToInt(index.z);
            if (x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth)
            {
                grid[x, y, z] = value;
            }
            else
            {
                throw new System.IndexOutOfRangeException("Index was outside the bounds of the array.");
            }
        }
    }

    // Indexer to access the grid using individual x, y, z coordinates
    public int this[int x, int y, int z]
    {
        get
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth)
            {
                return grid[x, y, z];
            }
            throw new System.IndexOutOfRangeException("Index was outside the bounds of the array.");
        }
        set
        {
            if (x >= 0 && x < Width && y >= 0 && y < Height && z >= 0 && z < Depth)
            {
                grid[x, y, z] = value;
            }
            else
            {
                throw new System.IndexOutOfRangeException("Index was outside the bounds of the array.");
            }
        }
    }

    // Method to initialize the grid with some values
    public void InitializeGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    grid[x, y, z] = 0;
                }
            }
        }
    }

    // Method to print the grid values
    public void PrintGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    Debug.Log($"grid[{x}, {y}, {z}] = {grid[x, y, z]}");
                }
            }
        }
    }

    public List<Vector3> FindCellsWithValue(int value)
    {
        List<Vector3> cells = new List<Vector3>();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Depth; z++)
                {
                    if (grid[x, y, z] == value)
                    {
                        cells.Add(new Vector3(x, y, z));
                    }
                }
            }
        }

        return cells;
    }

    public List<Vector3> PickRandomCells(List<Vector3> cells, int count)
    {
        List<Vector3> pickedCells = new List<Vector3>();

        if (cells.Count < count)
        {
            Debug.LogWarning("Not enough cells to pick the requested number of random cells.");
            return cells;
        }

        while (pickedCells.Count < count)
        {
            int index = Random.Range(0, cells.Count);
            if (!pickedCells.Contains(cells[index]))
            {
                pickedCells.Add(cells[index]);
            }
        }

        return pickedCells;
    }

}
