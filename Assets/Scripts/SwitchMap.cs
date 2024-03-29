using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchMap : MonoBehaviour
{
    [SerializeField] private Button SwitchButton = null;

    [SerializeField] private string SpaceSceneName = "SPACE";
    [SerializeField] private string AsteroidsSceneName = "AsteroidField";

    private void Start()
    {
        if (SwitchButton == null)
        {
            Debug.LogError("One or multiple field(s) unset in SwitchMap");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        SwitchButton.onClick.AddListener(ButtonClicked);
    }

    private void ButtonClicked()
    {
        if (SceneManager.GetActiveScene().name == SpaceSceneName)
            SceneManager.LoadScene(AsteroidsSceneName, LoadSceneMode.Single);
        else
            SceneManager.LoadScene(SpaceSceneName, LoadSceneMode.Single);
    }
}