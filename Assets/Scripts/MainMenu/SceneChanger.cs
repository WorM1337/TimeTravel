using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public static SceneChanger instance;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScene(int index)
    {
        LoadingWindow.SwitchScene(index);
    }
}
