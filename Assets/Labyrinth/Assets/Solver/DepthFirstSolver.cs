using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DepthFirstSolver : LabyrinthSolver
{
    protected override Vector2Int PopNext()
    {
        Vector2Int coordinates = ExploreNext[ExploreNext.Count - 1];
        ExploreNext.RemoveAt(ExploreNext.Count - 1);

        return coordinates;
    }
}
