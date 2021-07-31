using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierCurve : MonoBehaviour
{
    [SerializeField, Range(0.01f, 1)]
    float distance;
    [SerializeField]
    GameObject prefabSegment;

    readonly List<Transform> Nodes = new List<Transform>();
    readonly List<Transform> Segments = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
            Nodes.Add(transform.GetChild(i));
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> positions = new List<Vector3>();
        foreach (Transform transform in Nodes)
            positions.Add(transform.position);


        int index = 0;
        for(float range = distance; range < 1; range += distance)
        {
            if (index == Segments.Count)
                Segments.Add(Instantiate(prefabSegment, transform).transform);

            Segments[index].position = GetCurvePosition(positions, range);
            index++;
        }

        while(index < Segments.Count)
        {
            Destroy(Segments[Segments.Count - 1].gameObject);
            Segments.RemoveAt(Segments.Count - 1);
        }
    }

    Vector3 GetCurvePosition(List<Vector3> positions, float range)
    {
        if (positions.Count < 2)
        {
            Debug.LogError("Not enough points for a curve");
            return Vector3.zero;
        }

        if (positions.Count == 2)
            return Vector3.Lerp(positions[0], positions[1], range);

        List<Vector3> newPos = new List<Vector3>();

        for (int i = 0; i < positions.Count - 1; i++)
            newPos.Add(Vector3.Lerp(positions[i], positions[i + 1], range));

        return GetCurvePosition(newPos, range);
    }
}
