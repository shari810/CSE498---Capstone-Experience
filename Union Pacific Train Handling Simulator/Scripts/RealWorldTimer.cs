using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class RealWorldTimer : MonoBehaviour
{
    [Header("Using 12-hour (AM/PM) time")]
    [Range(1,12)]
    private int startingHour = 6;
    [Range(0,59)]
    private int startingMinute = 30;
    private string AMOrPM = "AM";

    private int currentHour;
    private int currentMinute;
    private string currentAMOrPM;
    private float elapsedSeconds = 0;
    public float timeMultiplier = 50f;
    private Text text;

    private bool AMPMSwitched = false;

    [Header("Parallax backgrounds")]
    public Image leftBackground;
    public Image rightBackground;

    public GameObject terrain;
    private List<SpriteShapeRenderer> sprites = new List<SpriteShapeRenderer>();

    public Color dayColor;
    public Color morningEveningColor;
    public Color nightColor;
    // Start is called before the first frame update
    void Start()
    {
        startingHour = Random.Range(1, 13);
        startingMinute = Random.Range(0, 60);
        AMOrPM = Random.Range(0, 2) == 0 ? "AM" : "PM";
        terrain = LevelManager.S.terrain;
        text = GetComponent<Text>();
        currentHour = startingHour;
        currentMinute = startingMinute;
        currentAMOrPM = AMOrPM;
        foreach (Transform terrainPiece in terrain.transform)
        {
            if (!terrainPiece.name.ToLower().Contains("station"))
                sprites.Add(terrainPiece.GetComponent<SpriteShapeRenderer>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        elapsedSeconds += Time.deltaTime * timeMultiplier;
        if (elapsedSeconds >= 60)
        {
            currentMinute += ((int)elapsedSeconds) / 60;
            elapsedSeconds %= 60;
        }
        if (currentMinute >= 60)
        {
            currentHour += currentMinute / 60;
            currentMinute %= 60;
        }
        if (currentHour >= 13)
        {
            currentHour = 1;
        }
        if (currentHour == 12 && !AMPMSwitched)
        {
            currentAMOrPM = currentAMOrPM == "AM" ? "PM" : "AM";
            AMPMSwitched = true;
        }
        else if (currentHour != 12)
        {
            AMPMSwitched = false;
        }
        text.text = currentHour.ToString() + ":" + currentMinute.ToString("D2") + " " + currentAMOrPM;

        // HANDLE DAY-NIGHT CYCLE
        if (currentAMOrPM == "AM" && currentHour >= 5 && currentHour <= 7)  // Sunrise part 1 (5:00 am to 7:59 am)
        {
            leftBackground.color = Color.Lerp(nightColor, morningEveningColor, ((currentHour - 5) * 60 + currentMinute) / 180f);
            rightBackground.color = Color.Lerp(nightColor, morningEveningColor, ((currentHour - 5) * 60 + currentMinute) / 180f);
            foreach (var sprite in sprites)
            {
                sprite.color = Color.Lerp(nightColor, morningEveningColor, ((currentHour - 5) * 60 + currentMinute) / 180f) + new Color32(127,127,127,255);
            }
        }
        else if (currentAMOrPM == "AM" && currentHour == 8)  // Sunrise part 2 (8:00 am to 8:59 am)
        {
            leftBackground.color = Color.Lerp(morningEveningColor, dayColor, currentMinute/60f);
            rightBackground.color = Color.Lerp(morningEveningColor, dayColor, currentMinute/60f);
            foreach (var sprite in sprites)
            {
                sprite.color = Color.Lerp(morningEveningColor, dayColor, currentMinute / 60f) + new Color32(127, 127, 127, 255);
            }
        }
        else if ((currentAMOrPM == "AM" && currentHour >= 9 && currentHour != 12) || (currentAMOrPM == "PM" && (currentHour <= 4 || currentHour == 12)))  // Day (9:00 am to 4:59 pm)
        {
            leftBackground.color = dayColor;
            rightBackground.color = dayColor;
            foreach (var sprite in sprites)
            {
                sprite.color = dayColor + new Color32(127, 127, 127, 255);
            }
        }
        else if (currentAMOrPM == "PM" && currentHour == 5) // Sunset part 1 (5:00 pm to 5:59 pm)
        {
            leftBackground.color = Color.Lerp(dayColor, morningEveningColor, currentMinute / 60f);
            rightBackground.color = Color.Lerp(dayColor, morningEveningColor, currentMinute / 60f);
            foreach (var sprite in sprites)
            {
                sprite.color = Color.Lerp(dayColor, morningEveningColor, currentMinute / 60f) + new Color32(127, 127, 127, 255);
            }
        }
        else if (currentAMOrPM == "PM" && currentHour >= 6 && currentHour <= 8) // Sunset part 2 (6:00 pm to 8:59 pm)
        {
            leftBackground.color = Color.Lerp(morningEveningColor, nightColor, ((currentHour - 6) * 60 + currentMinute) / 180f);
            rightBackground.color = Color.Lerp(morningEveningColor, nightColor, ((currentHour - 6) * 60 + currentMinute) / 180f);
            foreach (var sprite in sprites)
            {
                sprite.color = Color.Lerp(morningEveningColor, nightColor, ((currentHour - 6) * 60 + currentMinute) / 180f) + new Color32(127, 127, 127, 255);
            }
        }
        else  // Night (9:00 pm to 4:59 am)
        {
            leftBackground.color = nightColor;
            rightBackground.color = nightColor;
            foreach (var sprite in sprites)
            {
                sprite.color = nightColor + new Color32(127, 127, 127, 255);
            }
        }
    }
}
