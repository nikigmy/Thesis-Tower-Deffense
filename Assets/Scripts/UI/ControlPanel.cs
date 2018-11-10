using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour {
    [SerializeField]
    private Image pauseButtonImage;
    [SerializeField]
    private Sprite playImage;
    private Sprite pauseImage;

    [SerializeField]
    private Image speedUpButtonImage;

    public void Start()
    {
        pauseImage = pauseButtonImage.sprite;
    }

    public void PauseClicked()
    {
        GameManager.instance.PausePlayGame();
        if (GameManager.instance.Paused)
        {
            pauseButtonImage.sprite = playImage;
        }
        else
        {
            pauseButtonImage.sprite = pauseImage;
        }
    }

    public void SpeedUpClicked()
    {
        GameManager.instance.SpeedUpGame();
        if (GameManager.instance.GameSpedUp)
        {
            speedUpButtonImage.color = Color.green;
        }
        else
        {
            speedUpButtonImage.color = Color.white;
        }
    }

    public void SettingsClicked()
    {

    }
}
