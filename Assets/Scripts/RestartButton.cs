using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartButton : MonoBehaviour
{
    [SerializeField] private Button Button = null;

    private void Start()
    {
        if (Button == null)
        {
            Debug.LogError("One or multiple field(s) unset in SwitchMap");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        Button.onClick.AddListener(ButtonClicked);
    }

    private void ButtonClicked()
    {
        var nameScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nameScene, LoadSceneMode.Single);
    }
}