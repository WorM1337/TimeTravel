using UnityEngine;

public class LinkWindowManager : MonoBehaviour
{
    [SerializeField] private GameObject StartWindow;
    [SerializeField] private GameObject DescriptionWindow;


    public void EnableStartWindow()
    {
        StartWindow.SetActive(true);
        DescriptionWindow.SetActive(false);
    }

    public void EnableDescriptionWindow()
    {
        StartWindow.SetActive(false);
        DescriptionWindow.SetActive(true);
    }
}
