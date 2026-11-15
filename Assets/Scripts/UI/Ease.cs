using UnityEngine;

// easing curves, t goes from 0 to 1 (the same maths as the easing lab)
public static class Ease
{
    public static float Linear(float t) => t;

    // starts fast, slows into place: for things arriving
    public static float OutQuad(float t) => 1f - (1f - t) * (1f - t);
    public static float OutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);

    // starts slow, leaves fast: for things going away
    public static float InQuad(float t) => t * t;
    public static float InCubic(float t) => t * t * t;

    // slow at both ends: for things moving between two resting places
    public static float InOutQuad(float t) => t < 0.5f ? 2f * t * t : 1f - Mathf.Pow(-2f * t + 2f, 2f) / 2f;
    public static float InOutSine(float t) => -(Mathf.Cos(Mathf.PI * t) - 1f) / 2f;

    // overshoots to about 1.1, then settles on 1: for things that pop in and want attention
    public static float OutBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    // pulls back a little before leaving
    public static float InBack(float t)
    {
        const float c1 = 1.70158f;
        const float c3 = c1 + 1f;
        return c3 * t * t * t - c1 * t * t;
    }

    // lands and bounces: for the logo dropping in
    public static float OutBounce(float t)
    {
        const float n1 = 7.5625f, d1 = 2.75f;
        if (t < 1f / d1) return n1 * t * t;
        if (t < 2f / d1) { t -= 1.5f / d1; return n1 * t * t + 0.75f; }
        if (t < 2.5f / d1) { t -= 2.25f / d1; return n1 * t * t + 0.9375f; }
        t -= 2.625f / d1; return n1 * t * t + 0.984375f;
    }

    // a wobble that dies away: 0 at both ends, for shakes and punches
    public static float Shake(float t) => Mathf.Sin(t * Mathf.PI * 6f) * (1f - t);
    public static float Punch(float t) => Mathf.Sin(t * Mathf.PI) * (1f - t * 0.5f);
}
