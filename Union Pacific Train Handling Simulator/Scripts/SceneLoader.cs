using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader instance;

    //[SerializeField] GameObject loaderCanvas;
    //[SerializeField] Image progressBar;
    //[SerializeField] RawImage progressTrain;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    public async void LoadScene(string sceneName)
    {
        Debug.Log(sceneName + " loaded");
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        //loaderCanvas.SetActive(true);

        //do
        //{
        //    await Task.Delay(100);
        //    progressBar.fillAmount = scene.progress;
        //    float trainX = -236 + (scene.progress * (236 * 2));
        //    progressTrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(trainX, 44);
        //} while (scene.progress < 0.9f);

        //await Task.Delay(1000);
        //loaderCanvas.SetActive(false);

        scene.allowSceneActivation = true;
    }

    public async void LoadCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        Debug.Log(sceneName + " loaded");
        var scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        //loaderCanvas.SetActive(true);

        //do
        //{
        //    await Task.Delay(100);
        //    progressBar.fillAmount = scene.progress;
        //    float trainX = -236 + (scene.progress * (236 * 2));
        //    progressTrain.GetComponent<RectTransform>().anchoredPosition = new Vector2(trainX, 44);
        //} while (scene.progress < 0.9f);

        //await Task.Delay(1000);
        //loaderCanvas.SetActive(false);

        scene.allowSceneActivation = true;
    }
}
