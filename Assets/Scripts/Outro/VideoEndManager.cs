using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoEndManager : MonoBehaviour
{
    private PlayerInput playerInput;
    [SerializeField] private VideoPlayer player;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        var escAction = playerInput.actions["Pause"];
        escAction.performed += (_) => { EndVideo(); };
    }
    void Start()
    {
        player.loopPointReached += OnVideoEnd;
    }

    private void EndVideo()
    {
        SceneChanger.instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        SceneChanger.instance.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
