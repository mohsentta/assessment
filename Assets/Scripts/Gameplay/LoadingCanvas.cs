





using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private Canvas loadingCanvas;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadScene(int sceneNumber)
    {
        loadingCanvas.enabled = true;
        SceneManager.LoadScene(sceneNumber);
        loadingCanvas.enabled = false;
    }
    public void LoadScene(string sceneName)
    {
        loadingCanvas.enabled = true;
        SceneManager.LoadScene(sceneName);
        loadingCanvas.enabled = false;
    }
}