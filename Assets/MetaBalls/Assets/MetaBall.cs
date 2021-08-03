using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class MetaBall : MonoBehaviour
{
    SpriteShapeController SpriteShape;
    SpriteShapeRenderer Renderer;

    [SerializeField, Range(-10, 10)]
    float Mass = 1;
    [SerializeField, Range(3, 200)]
    int NbPoints;

    private void OnEnable()
    {
        FindObjectOfType<MetaBallManager>()?.Balls.Add(this);
        SpriteShape = GetComponent<SpriteShapeController>();
        Renderer = GetComponent<SpriteShapeRenderer>();
    }

    private void OnDisable() => FindObjectOfType<MetaBallManager>()?.Balls.Remove(this);

    public void UpdateMetaBall(float Threshold, MetaBallManager manager)
    {
        Renderer.color = Mass > 0 ? Color.white : Color.black;

        for (int i = SpriteShape.spline.GetPointCount() - 1; i >= NbPoints; i--)
            SpriteShape.spline.RemovePointAt(i);

        for (int i = 0; i < NbPoints; i++)
            SetPointAt(i, FindPosition(i, Threshold, manager));
    }

    void SetPointAt(int index, Vector3 pos)
    {
        if (index < SpriteShape.spline.GetPointCount())
            SpriteShape.spline.SetPosition(index, pos);
        else
            SpriteShape.spline.InsertPointAt(SpriteShape.spline.GetPointCount(), pos);

        SpriteShape.spline.SetTangentMode(index, ShapeTangentMode.Continuous);

        SpriteShape.spline.SetLeftTangent(index, Vector3.Cross(pos, Vector3.forward).normalized / NbPoints);
        SpriteShape.spline.SetRightTangent(index, Vector3.Cross(pos, Vector3.back).normalized / NbPoints);
    }

    Vector3 FindPosition(int index, float Threshold, MetaBallManager manager)
    {
        float Angle = 360f / NbPoints;
        float Step = 0.01f;

        Vector3 vector = Quaternion.Euler(0, 0, Angle * index) * Vector3.up;

        float k = Step * 20;
        if (Mass > 0)
        {
            if (Threshold > 0)
                while (manager.GlobalMetaBallFunction(vector * k + transform.position) > Threshold)
                    k += Step;
        }
        else
            k = -Mass;

        return vector * k;
    }

    public float MetaBallFunction(Vector3 position)
    {
        return Mass / Vector3.Distance(transform.position, position); ;
    }
}
