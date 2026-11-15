using UnityEngine;

// Short sounds made in code (no audio files), so every press has a sound as well as a motion.
// The music is a soft looping arpeggio, also made in code.
public class Sfx : MonoBehaviour
{
    public static Sfx Instance { get; private set; }

    AudioSource sfx, music;
    AudioClip click, back, coin, error, star, whoosh, buy;

    const int Rate = 44100;

    void Awake()
    {
        Instance = this;
        sfx = gameObject.AddComponent<AudioSource>();
        music = gameObject.AddComponent<AudioSource>();
        music.loop = true;
        music.clip = MakeMusic();

        click = Tone(660, 880, 0.06f, 0.5f);
        back = Tone(660, 440, 0.07f, 0.5f);
        coin = Arp(new[] { 988f, 1319f }, 0.06f, 0.45f);
        error = Tone(220, 160, 0.18f, 0.6f, square: true);
        star = Arp(new[] { 784f, 988f, 1175f }, 0.07f, 0.5f);
        buy = Arp(new[] { 523f, 659f, 784f, 1047f }, 0.06f, 0.5f);
        whoosh = Noise(0.18f, 0.25f);
        Apply();
        music.Play();
    }

    public void Apply()
    {
        sfx.volume = GameData.Sound;
        music.volume = GameData.Music * 0.35f;
    }

    public static void Click() => Play(i => i.click);
    public static void Back() => Play(i => i.back);
    public static void Coin() => Play(i => i.coin);
    public static void Error() => Play(i => i.error);
    public static void Star() => Play(i => i.star);
    public static void Buy() => Play(i => i.buy);
    public static void Whoosh() => Play(i => i.whoosh);

    static void Play(System.Func<Sfx, AudioClip> pick)
    {
        if (Instance != null) Instance.sfx.PlayOneShot(pick(Instance));
    }

    public static void Buzz()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (GameData.Vibration) Handheld.Vibrate();
#endif
    }

    // a sweep from f0 to f1 with a quick fade out
    static AudioClip Tone(float f0, float f1, float len, float vol, bool square = false)
    {
        int n = (int)(Rate * len);
        var d = new float[n];
        float phase = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / n;
            phase += 2f * Mathf.PI * Mathf.Lerp(f0, f1, t) / Rate;
            float s = Mathf.Sin(phase);
            if (square) s = Mathf.Sign(s) * 0.5f;
            d[i] = s * vol * (1f - t) * Mathf.Min(1f, i / 200f);
        }
        var c = AudioClip.Create("tone", n, 1, Rate, false);
        c.SetData(d, 0);
        return c;
    }

    static AudioClip Arp(float[] notes, float each, float vol)
    {
        int per = (int)(Rate * each), n = per * notes.Length + per;
        var d = new float[n];
        for (int k = 0; k < notes.Length; k++)
            for (int i = 0; i < per * 2 && k * per + i < n; i++)
            {
                float t = (float)i / (per * 2);
                d[k * per + i] += Mathf.Sin(2f * Mathf.PI * notes[k] * i / Rate) * vol * (1f - t) * 0.6f;
            }
        var c = AudioClip.Create("arp", n, 1, Rate, false);
        c.SetData(d, 0);
        return c;
    }

    static AudioClip Noise(float len, float vol)
    {
        int n = (int)(Rate * len);
        var d = new float[n];
        var r = new System.Random(3);
        float lp = 0f;
        for (int i = 0; i < n; i++)
        {
            float t = (float)i / n;
            lp += ((float)r.NextDouble() * 2f - 1f - lp) * 0.08f;   // a soft low-passed hiss
            d[i] = lp * vol * Mathf.Sin(t * Mathf.PI) * 3f;
        }
        var c = AudioClip.Create("whoosh", n, 1, Rate, false);
        c.SetData(d, 0);
        return c;
    }

    // C major then A minor, one soft note every quarter second
    static AudioClip MakeMusic()
    {
        float[] notes = { 523f, 659f, 784f, 659f, 523f, 659f, 784f, 988f, 440f, 523f, 659f, 523f, 440f, 523f, 659f, 784f };
        float step = 0.25f;
        int per = (int)(Rate * step), n = per * notes.Length;
        var d = new float[n];
        for (int k = 0; k < notes.Length; k++)
            for (int i = 0; i < per * 3; i++)
            {
                int j = (k * per + i) % n;
                float t = (float)i / (per * 3);
                d[j] += Mathf.Sin(2f * Mathf.PI * notes[k] * i / Rate) * 0.25f * Mathf.Exp(-4f * t);
            }
        var c = AudioClip.Create("music", n, 1, Rate, false);
        c.SetData(d, 0);
        return c;
    }
}
