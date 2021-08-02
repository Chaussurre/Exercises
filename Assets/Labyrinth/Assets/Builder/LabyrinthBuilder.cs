using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LabyrinthBuilder : MonoBehaviour
{
    readonly List<int> Labyrinth = new List<int>();
    
    [Range(1, 20)]
    public int Width = 10;
    [Range(1, 20)]
    public int Height = 10;

    public readonly int OpenTop = 1 << 0;
    public readonly int OpenLeft = 1 << 1;
    public readonly int OpenBot = 1 << 2;
    public readonly int OpenRight = 1 << 3;


    readonly List<int> ColorMap = new List<int>();

    public int GetCell(Vector2Int coordinates)
    {
        int index = GetCellIndex(coordinates);
        return Labyrinth[index];
    }
    public int GetCell(int index)
    {
        return Labyrinth[Mathf.Clamp(index, 0, Labyrinth.Count - 1)];
    }

    public void InitializeLabyrinth()
    {
        Labyrinth.Clear();
        for (int i = 0; i < Height * Width; i++)
            Labyrinth.Add(0);

        InitializeColorMap();

        Labyrinth[0] = OpenBot;
        Labyrinth[Labyrinth.Count - 1] = OpenTop;
    }
    public bool GenStepLabyrinth()
    {
        if (IsColorMapUnicolor())
            return true;

        List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };
       
        int index1;
        int index2;

        Vector2Int coordinates;
        Vector2Int direction;

        do
        {
            int x = Random.Range(0, Width);
            int y = Random.Range(0, Height);

            coordinates = new Vector2Int(x, y);
            direction = directions[Random.Range(0, 4)];

            index1 = GetCellIndex(coordinates);
            index2 = GetCellIndex(coordinates + direction);

        }
        while (ColorMap[index1] == ColorMap[index2]);
         

        FillColor(coordinates, ColorMap[index2]);
        LinkCells(coordinates, coordinates + direction);

        return false;
    }
    void SetInBounds(ref Vector2Int coordinates)
    {
        int x = Mathf.Clamp(coordinates.x, 0, Width - 1);
        int y = Mathf.Clamp(coordinates.y, 0, Height - 1);

        coordinates.Set(x, y);
    }

    public int GetCellIndex(Vector2Int coordinates)
    {
        SetInBounds(ref coordinates);
        
        return coordinates.x + coordinates.y * Width;
    }

    public Vector2Int GetCellCoordinates(int index)
    {
        Vector2Int coordinates = new Vector2Int(index % Width, index / Width);
        SetInBounds(ref coordinates);

        return coordinates;
    }


    void LinkCells(Vector2Int cell1, Vector2Int cell2)
    {
        Vector2Int relative = cell2 - cell1;
        int index1 = GetCellIndex(cell1);
        int index2 = GetCellIndex(cell2);

        if (relative == Vector2Int.right)
        {
            Labyrinth[index1] |= OpenRight;
            Labyrinth[index2] |= OpenLeft;
            return;
        }
        if (relative == Vector2Int.left)
        {
            Labyrinth[index1] |= OpenLeft;
            Labyrinth[index2] |= OpenRight;
            return;
        }
        if (relative == Vector2Int.down)
        {
            Labyrinth[index1] |= OpenBot;
            Labyrinth[index2] |= OpenTop;
            return;
        }
        if (relative == Vector2Int.up)
        {
            Labyrinth[index1] |= OpenTop;
            Labyrinth[index2] |= OpenBot;
            return;
        }

        Debug.LogError("" + cell1 + " and " + cell2 + " are not adjacent cells");
    }

    void FillColor(Vector2Int cell, int color)
    {
        List<Vector2Int> SeenCells = new List<Vector2Int>();
        SeenCells.Add(cell);

        while(SeenCells.Count > 0) //Breadth-first exploration of the graph
        {
            Vector2Int coordinates = SeenCells[0];
            int index = GetCellIndex(coordinates);
            SeenCells.RemoveAt(0);

            if (ColorMap[index] == color)
                continue;

            ColorMap[index] = color;
            int currentCell = Labyrinth[index];

            if ((currentCell & OpenTop) != 0)
                SeenCells.Add(coordinates + Vector2Int.up);
            if ((currentCell & OpenBot) != 0)
                SeenCells.Add(coordinates + Vector2Int.down);
            if ((currentCell & OpenLeft) != 0)
                SeenCells.Add(coordinates + Vector2Int.left);
            if ((currentCell & OpenRight) != 0)
                SeenCells.Add(coordinates + Vector2Int.right);
        }
    }

    bool IsColorMapUnicolor()
    {
        int color = ColorMap[0];
        foreach (int cell in ColorMap)
            if (cell != color)
                return false;
        return true;
    }

    void InitializeColorMap()
    {
        ColorMap.Clear();
        for (int i = 0; i < Height * Width; i++)
            ColorMap.Add(i);
    }

}
