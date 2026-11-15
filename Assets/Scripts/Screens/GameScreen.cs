using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// 7. The game itself, small but playable: the hopper runs on its own and a tap anywhere makes it jump.
// Coins on the ground come free, coins in the air need a jump, and that decides the stars.
// It runs on the game clock (Time.deltaTime), so Pause stops it.
public class GameScreen : UIScreen, IPointerDownHandler
{
    [SerializeField] RectTransform world, hopper, hopperBody, coinTemplate, coinLayer;
    [SerializeField] Image hopperColor, progressFill;
    [SerializeField] TMP_Text coinText, levelText, bannerText;
    [SerializeField] RectTransform coinCounter, banner;
    [SerializeField] CanvasGroup bannerGroup;
    [SerializeField] RectTransform[] clouds, tufts;

    public const float Speed = 520f;        // px per second
    const float HopperX = -300f;
    const float JumpSpeed = 1500f, Gravity = 3600f;   // a jump peaks at v² / 2g = 312 px, 0.83 s in the air
    const float Ground = 60f;               // the hopper's centre above the ground line

    public int Level { get; private set; } = 1;
    public int Collected { get; private set; }
    public int Total => coins.Count;
    float length, time, y, vy;
    bool running, grounded;

    struct CoinSpot { public float at, height; public RectTransform rt; public bool taken; }
    readonly List<CoinSpot> coins = new List<CoinSpot>();

    public void Prepare(int level)
    {
        Level = level;
        length = 9f + level * 0.75f;
        foreach (var c in coins) if (c.rt != null) Destroy(c.rt.gameObject);
        coins.Clear();
        // runs of ground coins, then an arc in the air that needs a jump
        var rng = new System.Random(level * 97);
        for (float at = 1.4f; at < length - 1f;)
        {
            int ground = rng.Next(2, 4);
            for (int i = 0; i < ground; i++, at += 0.22f) Add(at, 0f);
            at += 0.5f;
            float[] arc = { 150f, 250f, 150f };
            foreach (float h in arc) { Add(at, h); at += 0.16f; }
            at += 0.7f + (float)rng.NextDouble() * 0.4f;
        }
    }

    void Add(float at, float height)
    {
        var rt = Instantiate(coinTemplate, coinLayer);
        rt.gameObject.SetActive(true);
        rt.localScale = Vector3.one;
        coins.Add(new CoinSpot { at = at, height = height, rt = rt });
    }

    public override void OnEnter() => StartRun();

    public void Restart()
    {
        Prepare(Level);
        StartRun();
    }

    public void StartLevel(int level)
    {
        Prepare(level);
        StartRun();
    }

    void StartRun()
    {
        time = 0f; y = 0f; vy = 0f; grounded = true;
        Collected = 0;
        coinText.text = "0";
        levelText.text = "Level " + Level;
        hopperColor.color = GameData.Hoppers[GameData.Selected].color;
        for (int i = 0; i < coins.Count; i++)
        {
            var c = coins[i]; c.taken = false; c.rt.gameObject.SetActive(true); c.rt.localScale = Vector3.one; coins[i] = c;
        }
        Place();
        running = false;
        // "Ready?" then "Go!": the banner pops (OutBack) and the run starts when it goes
        Banner("Ready?", () => Banner("Go!", () => running = true, 0.35f), 0.6f);
    }

    void Banner(string text, System.Action then, float hold)
    {
        bannerText.text = text;
        banner.localScale = Vector3.one * 0.6f;
        bannerGroup.alpha = 0f;
        Tween.Scale(banner, Vector3.one, 0.3f, Ease.OutBack, null, 0f, false);
        Tween.Fade(bannerGroup, 1f, 0.15f, Ease.OutQuad, () =>
            Tween.Fade(bannerGroup, 0f, 0.2f, Ease.InQuad, then, hold));
    }

    public void OnPointerDown(PointerEventData e) => Jump();

    public void Jump()
    {
        if (!running || !grounded) return;
        grounded = false;
        vy = JumpSpeed;
        Sfx.Click();
        // stretch on take-off
        Tween.Scale(hopperBody, new Vector3(0.85f, 1.2f, 1f), 0.1f, Ease.OutQuad, null, 0f, false);
    }

    public void Pause() => UIManager.Instance.Push(ScreenId.Pause);

    void Update()
    {
        if (UIManager.Instance.Top != this) return;
        var kb = Keyboard.current;
        if (kb != null && (kb.spaceKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame)) Jump();
        if (!running) return;

        float dt = Time.deltaTime;
        time += dt;
        if (!grounded)
        {
            vy -= Gravity * dt;
            y += vy * dt;
            if (y <= 0f)
            {
                y = 0f; vy = 0f; grounded = true;
                // squash on landing, then back to round (OutBack)
                hopperBody.localScale = new Vector3(1.25f, 0.78f, 1f);
                Tween.Scale(hopperBody, Vector3.one, 0.3f, Ease.OutBack, null, 0f, false);
            }
            else if (vy < 0f && hopperBody.localScale.y > 1.05f)
                Tween.Scale(hopperBody, Vector3.one, 0.15f, Ease.OutQuad, null, 0f, false);
        }
        Place();
        Collect();
        progressFill.fillAmount = Mathf.Clamp01(time / length);

        if (time >= length) Finish();
    }

    void Place()
    {
        hopper.anchoredPosition = new Vector2(HopperX, Ground + y);
        for (int i = 0; i < coins.Count; i++)
        {
            var c = coins[i];
            c.rt.anchoredPosition = new Vector2(HopperX + (c.at - time) * Speed, Ground + c.height);
        }
        // the clouds drift slower than the ground: depth
        float w = world.rect.width + 400f;
        for (int i = 0; i < clouds.Length; i++)
        {
            var p = clouds[i].anchoredPosition;
            p.x = Mathf.Repeat(i * 420f - time * Speed * 0.15f + w / 2f, w) - w / 2f;
            clouds[i].anchoredPosition = p;
        }
        for (int i = 0; i < tufts.Length; i++)
        {
            var p = tufts[i].anchoredPosition;
            p.x = Mathf.Repeat(i * 230f - time * Speed + w / 2f, w) - w / 2f;
            tufts[i].anchoredPosition = p;
        }
    }

    void Collect()
    {
        for (int i = 0; i < coins.Count; i++)
        {
            var c = coins[i];
            if (c.taken) continue;
            float dx = Mathf.Abs(c.rt.anchoredPosition.x - HopperX);
            float dy = Mathf.Abs(c.height - y);
            if (dx < 70f && dy < 90f)
            {
                c.taken = true;
                coins[i] = c;
                Collected++;
                coinText.text = Collected.ToString();
                Sfx.Coin();
                // the coin pops and vanishes, the counter bumps: you see where it went
                var rt = c.rt;
                Tween.Scale(rt, Vector3.one * 1.6f, 0.18f, Ease.OutQuad, () => rt.gameObject.SetActive(false), 0f, false);
                Tween.PunchScale(coinCounter, 0.25f, 0.25f);
            }
        }
    }

    void Finish()
    {
        running = false;
        int stars = 1 + (Collected >= Total * 0.6f ? 1 : 0) + (Collected >= Total * 0.9f ? 1 : 0);
        Banner("Finish!", () =>
        {
            UIManager.Instance.Get<ResultsScreen>().Set(Level, Collected, Total, stars);
            UIManager.Instance.Push(ScreenId.Results);
        }, 0.5f);
    }
}
