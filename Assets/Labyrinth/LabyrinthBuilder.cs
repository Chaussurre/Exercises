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
    
    public int GetCell(Vector2Int coordinates)
    {
        int delta = CoordinatesToDelta(coordinates);
        return Labyrinth[delta];
    }

    public void GenNewLabyrinth()
    {
        InitializeLabyrinth();
        List<int> ColorMap = CreateColorMap();

        List<Vector2Int> directions = new List<Vector2Int> { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

        while (!IsColorMapUnicolor(ColorMap))
        {
            int x = Random.Range(0, Width);
            int y = Random.Range(0, Height);

            Vector2Int coordinates = new Vector2Int(x, y);
            Vector2Int direction = directions[Random.Range(0, 4)];

            int delta1 = CoordinatesToDelta(coordinates);
            int delta2 = CoordinatesToDelta(coordinates + direction);
            
            if (ColorMap[delta1] == ColorMap[delta2])
                continue;

            FillColor(ColorMap, coordinates, ColorMap[delta2]);
            LinkCells(coordinates, coordinates + direction);
        }
    }
    void SetInBounds(ref Vector2Int coordinates)
    {
        int x = Mathf.Clamp(coordinates.x, 0, Width - 1);
        int y = Mathf.Clamp(coordinates.y, 0, Height - 1);

        coordinates.Set(x, y);
    }

    int CoordinatesToDelta(Vector2Int coordinates)
    {
        SetInBounds(ref coordinates);
        
        return coordinates.x + coordinates.y * Width;
    }

    Vector2Int DeltaToCoordinates(int delta)
    {
        int x = delta % Width;
        int y = delta / Width;

        Vector2Int coordinates =  new Vector2Int(x, y);
        SetInBounds(ref coordinates);
        return coordinates;
    }


    void LinkCells(Vector2Int cell1, Vector2Int cell2)
    {
        Vector2Int relative = cell2 - cell1;
        int delta1 = CoordinatesToDelta(cell1);
        int delta2 = CoordinatesToDelta(cell2);

        if (relative == Vector2Int.right)
        {
            Labyrinth[delta1] |= OpenRight;
            Labyrinth[delta2] |= OpenLeft;
            return;
        }
        if (relative == Vector2Int.left)
        {
            Labyrinth[delta1] |= OpenLeft;
            Labyrinth[delta2] |= OpenRight;
            return;
        }
        if (relative == Vector2Int.down)
        {
            Labyrinth[delta1] |= OpenBot;
            Labyrinth[delta2] |= OpenTop;
            return;
        }
        if (relative == Vector2Int.up)
        {
            Labyrinth[delta1] |= OpenTop;
            Labyrinth[delta2] |= OpenBot;
            return;
        }

        Debug.LogError("" + cell1 + " and " + cell2 + " are not adjacent cells");
    }

    void FillColor(List<int> colorMap, Vector2Int cell, int color)
    {
        List<Vector2Int> SeenCells = new List<Vector2Int>();
        SeenCells.Add(cell);

        while(SeenCells.Count > 0) //Breadth-first exploration of the graph
        {
            Vector2Int coordinates = SeenCells[0];
            int delta = CoordinatesToDelta(coordinates);
            SeenCells.RemoveAt(0);

            if (colorMap[delta] == color)
                continue;

            colorMap[delta] = color;
            int currentCell = Labyrinth[delta];

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

    bool IsColorMapUnicolor(List<int> colorMap)
    {
        int color = colorMap[0];
        foreach (int cell in colorMap)
            if (cell != color)
                return false;
        return true;
    }

    List<int> CreateColorMap()
    {
        List<int> ColorMap = new List<int>();
        for (int i = 0; i < Height * Width; i++)
            ColorMap.Add(i);

        return ColorMap;
    }

    void InitializeLabyrinth()
    {
        Labyrinth.Clear();
        for (int i = 0; i < Height * Width; i++)
            Labyrinth.Add(0);

        Labyrinth[0] = OpenBot;
        Labyrinth[Labyrinth.Count - 1] = OpenTop;
    }

}
