using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    private float currentDamage = 0;
    private int numLives = 0;
    Text damageReadout;
    Image livesReadout;
    Image background;
	// Use this for initialization
	void Start () {
        damageReadout = this.transform.FindChild("HealthNumber").GetComponent<Text>();
        livesReadout = this.transform.FindChild("LivesImage").GetComponent<Image>();
        background = this.GetComponent<Image>();
	}
    private static Color backgroundColor = new Color(1, 1, 1, 1);

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

    void ShowBackground()
    {
        background.color = backgroundColor;
    }

    public void UpdateUI(float damage = -1, int lives = -1)
    {
        if (damageReadout == null || livesReadout == null || background == null) //then this hasn't been initialized yet
            Start();
        if (damage != -1)
            this.currentDamage = damage;
        if (lives != -1)
            this.numLives = lives;
        Debug.Log("Update!");
        UpdateText();
        UpdateLives();
        ShowBackground();
    }
}
