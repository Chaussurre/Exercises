using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum SolverType
{
    BreadthFirst, DepthFirst
}

public class LabyrinthSolverView : MonoBehaviour
{
    LabyrinthBuilder Builder;

    [SerializeField]
    SolverType SolverType;

    LabyrinthSolver BreadthFirstSolver;
    LabyrinthSolver DepthFirstSolver;


    LabyrinthSolver Solver;

    [SerializeField]
    Tile tileExplore;
    [SerializeField]
    Tile tileSolution;

    [Range(0, 2)]
    public float TimePerStep = 1f;
    float timer = 0;

    Tilemap tilemap;

    bool running = false;

    // Start is called before the first frame update
    void Start()
    {
        BreadthFirstSolver = GetComponent<BreadthFirstSolver>();
        DepthFirstSolver = GetComponent<DepthFirstSolver>();
        tilemap = GetComponentInChildren<Tilemap>();
        Builder = GetComponent<LabyrinthBuilder>();
        
        switch(SolverType)
        {
            case SolverType.BreadthFirst:
                Solver = BreadthFirstSolver;
                break;
            case SolverType.DepthFirst:
                Solver = DepthFirstSolver;
                break;
        }

    }

    public void StartSolving()
    {
        Vector2Int end = new Vector2Int(Builder.Width - 1, Builder.Height - 1);

        Solver.Initialize(Vector2Int.zero, end, Builder);
        running = true;
    }

    private void Update()
    {
        if (!running)
            return;

        timer += Time.deltaTime;
        while (timer > TimePerStep)
        {
            timer -= TimePerStep;
            if (Solver.ExploreNextStep())
            {
                End();
                break;
            }
            ViewSolutionBuild();
        }
    }

    void ViewSolutionBuild()
    {
        foreach(int index in Solver.ExploredCells())
        {
            Vector2Int CellCoordinates = Builder.GetCellCoordinates(index) * 2;
            Vector3Int MapCoordinates = new Vector3Int(CellCoordinates.x, CellCoordinates.y);
            int cell = Builder.GetCell(index);
            
            tilemap.SetTile(MapCoordinates, tileExplore);

            if ((cell & Builder.OpenTop) != 0) tilemap.SetTile(MapCoordinates + Vector3Int.up, tileExplore);
            if ((cell & Builder.OpenBot) != 0) tilemap.SetTile(MapCoordinates + Vector3Int.down, tileExplore);
            if ((cell & Builder.OpenLeft) != 0) tilemap.SetTile(MapCoordinates + Vector3Int.left, tileExplore);
            if ((cell & Builder.OpenRight) != 0) tilemap.SetTile(MapCoordinates + Vector3Int.right, tileExplore);
        }
    }


    void End()
    {
        ViewSolutionBuild();

        List<Vector2Int> Solution = Solver.GetSolution();

        Vector2Int previous = (Solution[0] + Vector2Int.down) * 2;

        foreach (Vector2Int coordinates in Solution)
        {
            DrawCellSolution(coordinates * 2, previous);
            previous = coordinates * 2;
        }

        DrawCellSolution(previous + Vector2Int.up, previous); //Drawing the exit

        Destroy(this);
    }

    void DrawCellSolution(Vector2Int coordinates, Vector2Int previous)
    {
        Vector3Int c = new Vector3Int(coordinates.x, coordinates.y);
        Vector3Int p = new Vector3Int(previous.x, previous.y);

        Vector3Int middle = ((p - c) / 2) + c;

        tilemap.SetTile(c, tileSolution);
        tilemap.SetTile(middle, tileSolution);
    }
}
