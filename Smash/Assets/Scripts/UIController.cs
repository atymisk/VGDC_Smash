using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private float currentDamage = 0;
    private int numLives = 0;
    private Text damageReadout;
    private Image livesReadout;
    private Image background;
    private CanvasGroup group;          //for using UI alpha to enable/disable stuff
	// Use this for initialization
	void Start () {
        damageReadout = this.transform.FindChild("HealthNumber").GetComponent<Text>();
        livesReadout = this.transform.FindChild("LivesImage").GetComponent<Image>();
        background = this.GetComponent<Image>();
        group = GetComponent<CanvasGroup>();
	}

    void UpdateText()
    {
        damageReadout.text = currentDamage + "%";
    }

    void UpdateLives()
    {
        float newWidth = (numLives) * 20;
        livesReadout.rectTransform.sizeDelta = new Vector2(newWidth, 20);
        livesReadout.rectTransform.localPosition = new Vector2(newWidth / 2 - 40, 15);
    }

    void Show(bool enabled = true)
    {
        if (enabled)
            group.alpha = 1;
        else
            group.alpha = 0;
    }

    public void UpdateUI(float damage = -1, int lives = -1)
    {
        if (damageReadout == null || livesReadout == null || background == null) //then this hasn't been initialized yet
            Start();
        if (damage != -1)
            this.currentDamage = damage;
        if (lives != -1)
            this.numLives = lives;
        UpdateText();
        UpdateLives();
        Show();
    }
}
