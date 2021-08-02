using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LabyrinthSolver : MonoBehaviour
{
    protected LabyrinthBuilder Builder;

    readonly protected List<Vector2Int> ExploreNext = new List<Vector2Int>();
    readonly protected HashSet<int> Explored = new HashSet<int>();

    readonly protected List<int> PreviousVector = new List<int>();

    protected Vector2Int target;

    public HashSet<int> ExploredCells()
    {
        return Explored;
    }

    public void Initialize(Vector2Int start, Vector2Int end, LabyrinthBuilder Labyrinth)
    {
        target = end;
        Builder = Labyrinth;
        ExploreNext.Clear();
        ExploreNext.Add(start);

        Explored.Clear();

        PreviousVector.Clear();
        for (int i = 0; i < Builder.Height * Builder.Width; i++)
            PreviousVector.Add(-1); //-1 is unreachable / Not reached yet
        PreviousVector[0] = -2; // -2 is start
    }

    public bool ExploreNextStep()
    {
        if (ExploreNext.Count == 0)
            return true;

        Vector2Int coordinates = PopNext();

        if (coordinates == target)
            return true;

        int index = Builder.GetCellIndex(coordinates);

        if (Explored.Contains(index))
            return false;

        Explored.Add(index);

        int cell = Builder.GetCell(coordinates);

        CheckNeighbourg(cell, Builder.OpenTop, coordinates, coordinates + Vector2Int.up);
        CheckNeighbourg(cell, Builder.OpenBot, coordinates, coordinates + Vector2Int.down);
        CheckNeighbourg(cell, Builder.OpenLeft, coordinates, coordinates + Vector2Int.left);
        CheckNeighbourg(cell, Builder.OpenRight, coordinates, coordinates + Vector2Int.right);

        return false;
    }

    protected abstract Vector2Int PopNext();

    protected void CheckNeighbourg(int cell, int direction, Vector2Int coordinates, Vector2Int Neighbourg)
    {
        if ((cell & direction) != 0)
        {
            ExploreNext.Add(Neighbourg);
            int indexNeighbourg = Builder.GetCellIndex(Neighbourg);
            int index = Builder.GetCellIndex(coordinates);
            if (PreviousVector[indexNeighbourg] == -1)
                PreviousVector[indexNeighbourg] = index;
        }
    }

    public List<Vector2Int> GetSolution()
    {
        List<Vector2Int> Solution = new List<Vector2Int> { target };
        int previous = PreviousVector[Builder.GetCellIndex(target)];

        while(previous > 0)
        {
            Solution.Insert(0, Builder.GetCellCoordinates(previous));
            previous = PreviousVector[previous];
        }

        Solution.Insert(0, Builder.GetCellCoordinates(previous));

        return Solution;
    }
}
