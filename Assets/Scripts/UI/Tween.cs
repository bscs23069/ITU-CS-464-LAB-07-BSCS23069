using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A small tween helper: every tween is a coroutine on one runner object.
// Menu tweens use unscaled time, so they still play while the game is paused (timeScale 0).
public class Tween : MonoBehaviour
{
    static Tween runner;
    // one running tween per (object, channel), so a new move on the same thing replaces the old one
    static readonly Dictionary<(int, string), Coroutine> running = new Dictionary<(int, string), Coroutine>();

    // Settings → Reduce motion: shorter tweens, no overshoot
    public static bool ReduceMotion;
    // playback speed for every tween: 1 normally, lower for slow motion when checking an animation
    public static float Speed = 1f;

    static Tween Runner
    {
        get
        {
            if (runner == null)
            {
                var go = new GameObject("Tweens");
                DontDestroyOnLoad(go);
                runner = go.AddComponent<Tween>();
            }
            return runner;
        }
    }

    public static void Kill(UnityEngine.Object target, string channel)
    {
        var key = (target.GetInstanceID(), channel);
        if (running.TryGetValue(key, out var c) && c != null && runner != null) runner.StopCoroutine(c);
        running.Remove(key);
    }

    public static void KillAll(UnityEngine.Object target)
    {
        foreach (var ch in new[] { "pos", "scale", "alpha", "value", "rot", "color" }) Kill(target, ch);
    }

    // the core: call step(eased t) every frame for duration seconds
    public static Coroutine Value(UnityEngine.Object target, string channel, float duration, Func<float, float> ease,
        Action<float> step, Action done = null, float delay = 0f, bool unscaled = true)
    {
        Kill(target, channel);
        if (ReduceMotion && unscaled)
        {
            duration = Mathf.Min(duration, 0.12f);
            delay = Mathf.Min(delay, 0.05f);
            if (ease == Ease.OutBack || ease == Ease.OutBounce || ease == Ease.InBack) ease = Ease.OutQuad;
            if (ease == Ease.Shake || ease == Ease.Punch) { step(0f); done?.Invoke(); return null; }
        }
        var key = (target.GetInstanceID(), channel);
        var c = Runner.StartCoroutine(Run(target, key, duration, ease, step, done, delay, unscaled));
        running[key] = c;
        return c;
    }

    static IEnumerator Run(UnityEngine.Object target, (int, string) key, float duration, Func<float, float> ease,
        Action<float> step, Action done, float delay, bool unscaled)
    {
        for (float w = 0f; w < delay; w += (unscaled ? Time.unscaledDeltaTime : Time.deltaTime) * Speed)
            yield return null;
        for (float t = 0f; t < duration; t += (unscaled ? Time.unscaledDeltaTime : Time.deltaTime) * Speed)
        {
            if (target == null) yield break;
            step(ease(t / duration));
            yield return null;
        }
        if (target == null) yield break;
        step(ease(1f)); // land exactly on the end value
        running.Remove(key);
        done?.Invoke();
    }

    // ---- the usual shortcuts ----
    public static Coroutine Move(RectTransform rt, Vector2 to, float d, Func<float, float> ease, Action done = null, float delay = 0f, bool unscaled = true)
    {
        Vector2 from = rt.anchoredPosition;
        return Value(rt, "pos", d, ease, t => rt.anchoredPosition = Vector2.LerpUnclamped(from, to, t), done, delay, unscaled);
    }

    public static Coroutine Scale(Transform tr, Vector3 to, float d, Func<float, float> ease, Action done = null, float delay = 0f, bool unscaled = true)
    {
        Vector3 from = tr.localScale;
        return Value(tr, "scale", d, ease, t => tr.localScale = Vector3.LerpUnclamped(from, to, t), done, delay, unscaled);
    }

    public static Coroutine Fade(CanvasGroup g, float to, float d, Func<float, float> ease, Action done = null, float delay = 0f)
    {
        float from = g.alpha;
        return Value(g, "alpha", d, ease, t => g.alpha = Mathf.LerpUnclamped(from, to, t), done, delay);
    }

    // a quick bump in size that settles back: "this just changed"
    public static Coroutine PunchScale(Transform tr, float amount, float d)
    {
        Vector3 rest = Vector3.one;
        return Value(tr, "scale", d, Ease.Punch, t => tr.localScale = rest * (1f + amount * t));
    }

    // a sideways wobble: "no"
    // remembers where a shaking object rests, so a second shake mid-shake doesn't drift it
    static readonly Dictionary<int, Vector2> shakeRest = new Dictionary<int, Vector2>();

    public static Coroutine Shake(RectTransform rt, float px, float d)
    {
        int id = rt.GetInstanceID();
        if (!shakeRest.TryGetValue(id, out Vector2 rest)) rest = rt.anchoredPosition;
        shakeRest[id] = rest;
        rt.anchoredPosition = rest;
        return Value(rt, "pos", d, Ease.Shake, t => rt.anchoredPosition = rest + new Vector2(px * t, 0f),
            () => { rt.anchoredPosition = rest; shakeRest.Remove(id); });
    }
}
