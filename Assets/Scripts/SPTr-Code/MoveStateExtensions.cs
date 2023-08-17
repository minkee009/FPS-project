using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MoveStateExtensions
{
    public static Vector3 ComputeMoveVector(Vector2 input, LookController lookCon, bool useAltCamHolder = false)
    {
        return (useAltCamHolder ? lookCon.AltCamHolder.rotation : lookCon.CamHolder.rotation) * new Vector3(input.x, 0f, input.y);
    }

    public static Vector3 Calculate3PCurve(Vector3[] pos, float timing)
    {
        var lerpP1 = Vector3.Lerp(pos[0], pos[1], timing);
        var lerpP2 = Vector3.Lerp(pos[1], pos[2], timing);

        return Vector3.Lerp(lerpP1, lerpP2, timing);
    }

    public static bool isMoveForward(Vector2 input)
    {
        return Vector2.Dot(Vector2.up, input.normalized) > 0.7f && input.magnitude > 0.98f;
    }
}
