using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class VictoryChecker : MonoBehaviour
{
    public float distance = 0;
    public float endline = 3000;
    private AudioSource victorySound;

    // Start is called before the first frame update
    void Awake()
    {
      GameManager.Victory = false;
        distance = transform.position.x;
    }

    private void Start()
    {
        victorySound = GameObject.Find("LevelEssentials/SoundBank/UI/Success").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
      distance = transform.position.x;
      if (distance >= endline && !GameManager.Victory)
      {
        Debug.Log("VICTORY");
        victorySound.ignoreListenerPause = true;
        victorySound.Play();
        if (SceneManager.GetActiveScene().name != "Lordsburg_5")
            GameManager.S.Winning();
        else
            GameManager.S.Winning("You are a train handling master!");
        DataPersistenceManager.instance.SaveGame();
      }
    }
}
