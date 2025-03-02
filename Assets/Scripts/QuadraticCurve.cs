using UnityEngine;

public class QuadraticCurve
{
    public static Vector3 Evaluate(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        // Quadratic Bezier formula
        Vector3 ac = Vector3.Lerp(start, control, t);
        Vector3 cb = Vector3.Lerp(control, end, t);
        return Vector3.Lerp(ac, cb, t);
    }
}