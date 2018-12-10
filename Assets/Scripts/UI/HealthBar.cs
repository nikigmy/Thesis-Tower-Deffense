using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    [SerializeField]
    Gradient healthbarColors;
    [SerializeField]
    Image bar;

    // Use this for initialization
    void Start()
    {
        var color = healthbarColors.Evaluate(1);
        bar.fillAmount = 1;
        bar.rectTransform.anchorMin = new Vector2(0, 0);
        bar.rectTransform.offsetMin = new Vector2(0, 0);
        bar.color = color;
    }

    public void SetBar(float value)
    {
        var color = healthbarColors.Evaluate(value);
        bar.rectTransform.anchorMin = new Vector2(1 - value, 0);
        bar.rectTransform.offsetMin = new Vector2(0, 0);
        bar.fillAmount = value;
        bar.color = color;
    }
}
