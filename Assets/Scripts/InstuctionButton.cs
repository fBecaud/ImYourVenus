using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InstructionButton : MonoBehaviour
{
    [Header("Instruction")]
    [SerializeField] private Button InstructButton = null;

    [SerializeField] private GameObject InstructPanel = null;

    // Start is called before the first frame update
    private void Start()
    {
        if (InstructButton == null || InstructPanel == null)
        {
            Debug.LogError("One or multiple field unset in InstuctionButton");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        InstructPanel.SetActive(false);
        InstructButton.onClick.AddListener(delegate { InstructionPressed(); });
    }

    private void InstructionPressed()
    {
        Vector3 currentScale = InstructButton.transform.localScale;
        InstructButton.transform.localScale = new(currentScale.x, -currentScale.y, currentScale.z);
        InstructPanel.SetActive(!InstructPanel.activeInHierarchy);
    }
}