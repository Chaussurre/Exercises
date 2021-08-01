using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BlockCreator : MonoBehaviour
{
    float WaitTime;
    float CreationTime;

    SpriteRenderer renderer;
    Tilemap tilemap;
    Tile tile;
    Vector3Int coordinates;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }

    public void Intialize(float waitTime, float creationTime, Vector3Int coordinates, Tilemap tilemap)
    {
        WaitTime = waitTime;
        CreationTime = creationTime;
        this.coordinates = coordinates;
        this.tilemap = tilemap;
        tile = tilemap.GetTile(coordinates) as Tile;
        renderer.sprite = tile.sprite;
        renderer.color = tile.color;

        transform.position = tilemap.GetCellCenterWorld(coordinates);
        tilemap.SetTile(coordinates, null);
        transform.localScale = Vector3.forward;

        StartCoroutine("CreateRoutine");
    }

    IEnumerator CreateRoutine()
    {
        yield return new WaitForSeconds(WaitTime);

        for (float timer = 0f; timer < CreationTime; timer += 0.1f)
        {
            transform.localScale = new Vector3(1, 1) * (timer / CreationTime) + Vector3.forward;
            yield return new WaitForSeconds(0.1f);
        }

        transform.localScale = new Vector3(1, 1, 1);

        Destroy(gameObject);
        tilemap.SetTile(coordinates, tile);
    }
}
