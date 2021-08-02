using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSolver : LabyrinthSolver
{
    protected override Vector2Int PopNext()
    {
        Vector2Int coordinates = ExploreNext[0];
        ExploreNext.RemoveAt(0);

        return coordinates;
    }
}
