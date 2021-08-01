using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LabyrinthViewBuilder : MonoBehaviour
{
    LabyrinthBuilder Builder;

    [SerializeField]
    BlockCreator CreatorPrefab;
    [SerializeField]
    Tile wall;

    Tilemap tilemap;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        Builder = GetComponent<LabyrinthBuilder>();
        Builder.GenNewLabyrinth();
        BuildAll();
        CreateBlockCreators();
    }

    void BuildAll()
    {
        transform.position = new Vector3(-Builder.Width, -Builder.Height);
        for (int x = 0; x < Builder.Width; x++)
            for (int y = 0; y < Builder.Height; y++)
                buildCell(new Vector3Int(x, y), Builder.GetCell(new Vector2Int(x, y)));
    }

    void buildCell(Vector3Int coorindates, int cell)
    {
        coorindates *= 2;

        foreach (Vector3Int Vertical in new List<Vector3Int> { Vector3Int.up, Vector3Int.down })
            foreach (Vector3Int Horizontal in new List<Vector3Int> { Vector3Int.left, Vector3Int.right })
                tilemap.SetTile(coorindates + Vertical + Horizontal, wall);

        if ((cell & Builder.OpenTop) == 0)
            tilemap.SetTile(coorindates + Vector3Int.up, wall);
        if ((cell & Builder.OpenBot) == 0)
            tilemap.SetTile(coorindates + Vector3Int.down, wall);
        if ((cell & Builder.OpenLeft) == 0)
            tilemap.SetTile(coorindates + Vector3Int.left, wall);
        if ((cell & Builder.OpenRight) == 0)
            tilemap.SetTile(coorindates + Vector3Int.right, wall);
    }

    void CreateBlockCreators()
    {
        for (int x = -1; x < Builder.Width * 2; x++)
            for (int y = -1; y < Builder.Width * 2; y++)
            {
                Vector3Int coordinates = new Vector3Int(x, y);
                if (tilemap.GetTile(coordinates) != null)
                {
                    BlockCreator block = Instantiate(CreatorPrefab, transform);
                    block.Intialize(coordinates.magnitude / 20f, 1f, coordinates, tilemap);
                }
            }
    }
}
