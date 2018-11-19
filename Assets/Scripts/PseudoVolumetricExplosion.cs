using UnityEngine;
using System.Collections;

public class PseudoVolumetricExplosion : MonoBehaviour
{
    public float loopDuration = 1;
    public float loopOffset = 0;
    public bool randomizeLoopOffset = true;
    public AnimationCurve scale = AnimationCurve.EaseInOut(0, 0.2f, 1, 10);
    public AnimationCurve minRange = AnimationCurve.Linear(0, 0, 1, 0.5f);
    public AnimationCurve maxRange = AnimationCurve.Linear(0, 0.2f, 1, 1);
    public AnimationCurve clip = AnimationCurve.Linear(0.5f, 0.7f, 1, 0.5f);
    public float timeScale = 1;

    private Vector3 endScale;
    private float startTime;
    [SerializeField]
    bool play;
    MeshRenderer rend;

    private void Awake()
    {
        rend = GetComponent<MeshRenderer>();
        play = false;
    }

    public void SetRadius(float radius)
    {
        scale = AnimationCurve.EaseInOut(0, 0.2f, 1, radius * 2);
        startTime = Time.time;
        play = true;
    }

    void Start()
    {
        loopDuration *= timeScale;
        loopOffset *= timeScale;
        if (randomizeLoopOffset)
        {
            loopOffset = Random.Range(0, loopDuration);
        }
        endScale = transform.localScale;
    }

    void Update()
    {
        if (play)
        {
            if (!rend.enabled)
            {
                rend.enabled = true;
            }
            float timeFromBegin = Time.time - startTime;
            if (timeFromBegin >= timeScale)
            {
                Destroy(gameObject);
            }
            float pos = (loopOffset + timeFromBegin) / loopDuration;
            float r = Mathf.Sin((pos) * (2 * Mathf.PI)) * 0.5f + 0.25f;
            float g = Mathf.Sin((pos + 0.33333333f) * 2 * Mathf.PI) * 0.5f + 0.25f;
            float b = Mathf.Sin((pos + 0.66666667f) * 2 * Mathf.PI) * 0.5f + 0.25f;
            float correction = 1 / (r + g + b);
            r *= correction;
            g *= correction;
            b *= correction;
            rend.material.SetVector("_ChannelFactor", new Vector4(r, g, b, 0));

            float scaleFactor = scale.Evaluate(timeFromBegin / timeScale);
            transform.localScale = endScale * scaleFactor;

            float beginRange = minRange.Evaluate(timeFromBegin / timeScale);
            float endRange = maxRange.Evaluate(timeFromBegin / timeScale);
            float clipVal = clip.Evaluate(timeFromBegin / timeScale);
            rend.material.SetVector("_Range", new Vector4(beginRange, endRange, 0, 0));
            rend.material.SetFloat("_ClipRange", clipVal);
        }
    }
}
