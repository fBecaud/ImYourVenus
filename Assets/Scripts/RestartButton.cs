using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RestartButton : MonoBehaviour
{
    private Button Button = null;

    private void Start()
    {
        Button = GetComponent<Button>();
        Button.onClick.AddListener(ButtonClicked);
    }

    private void ButtonClicked()
    {
        var nameScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(nameScene, LoadSceneMode.Single);
    }
}