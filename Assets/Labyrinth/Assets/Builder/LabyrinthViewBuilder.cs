using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LabyrinthViewBuilder : MonoBehaviour
{
    LabyrinthBuilder Builder;
    LabyrinthSolverView Solver;

    [SerializeField]
    Tile wall;

    [Range(0, 2)]
    public float TimePerStep = 1f;
    float timer = 0;

    Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        Builder = GetComponent<LabyrinthBuilder>();
        Solver = GetComponent<LabyrinthSolverView>();
        Builder.InitializeLabyrinth();
        BuildAll();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        while(timer > TimePerStep)
        {
            timer -= TimePerStep;
            if (Builder.GenStepLabyrinth())
            {
                End();
                break;
            }
            BuildAll();
        }
    }

    void BuildAll()
    {
        transform.position = new Vector3(-Builder.Width, -Builder.Height);
        for (int x = 0; x < Builder.Width; x++)
            for (int y = 0; y < Builder.Height; y++)
                buildCell(new Vector3Int(x, y), Builder.GetCell(new Vector2Int(x, y)));
    }

    void buildCell(Vector3Int coordinates, int cell)
    {
        coordinates *= 2;

        foreach (Vector3Int Vertical in new List<Vector3Int> { Vector3Int.up, Vector3Int.down })
            foreach (Vector3Int Horizontal in new List<Vector3Int> { Vector3Int.left, Vector3Int.right })
                tilemap.SetTile(coordinates + Vertical + Horizontal, wall);


        BuildWall(cell, Builder.OpenTop, coordinates + Vector3Int.up);
        BuildWall(cell, Builder.OpenBot, coordinates + Vector3Int.down);
        BuildWall(cell, Builder.OpenLeft, coordinates + Vector3Int.left);
        BuildWall(cell, Builder.OpenRight, coordinates + Vector3Int.right);
    }

    void BuildWall(int cell, int direction, Vector3Int coordinates)
    {
        if ((cell & direction) == 0)
            tilemap.SetTile(coordinates, wall);
        else
            tilemap.SetTile(coordinates, null);
    }

    void End()
    {
        BuildAll();
        Solver.StartSolving();
        Destroy(this);
    }
}
