using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public Transform creditsViewpoint;
    public Transform menuViewpoint;
    public float cameraMoveSpeed = 2f;
    public Camera mainCamera;

    private bool viewingCredits = false;

    void Update()
    {
        if (viewingCredits==false)
        {
            MoveCamera(creditsViewpoint);
        }
        else
        {
            MoveCamera(menuViewpoint);
        }
    }

    public void OnStartButton()
    {
        SceneManager.LoadScene(1); // Change to your actual game scene name
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Game is exiting...");
    }

    public void OnCreditsButton()
    {
        viewingCredits = true;
    }

    public void OnBackToMenu()
    {
        viewingCredits = false;
    }

    void MoveCamera(Transform target)
    {
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, target.position, Time.deltaTime * cameraMoveSpeed);
        mainCamera.transform.rotation = Quaternion.Slerp(mainCamera.transform.rotation, target.rotation, Time.deltaTime * cameraMoveSpeed);
    }
}
