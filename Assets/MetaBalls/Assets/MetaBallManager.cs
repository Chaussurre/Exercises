using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetaBallManager : MonoBehaviour
{
    readonly public HashSet<MetaBall> Balls = new HashSet<MetaBall>();

    [SerializeField, Range(0.0001f, 10f)]
    float Threshold;

    public float GlobalMetaBallFunction(Vector3 position)
    {
        float result = 0f;
        foreach (MetaBall ball in Balls)
            result += ball.MetaBallFunction(position);

        return result;
    }

    private void Update()
    {
        foreach (MetaBall ball in Balls)
            ball.UpdateMetaBall(Threshold, this);
    }
}
