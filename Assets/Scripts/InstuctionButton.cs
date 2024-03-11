using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class InstuctionButton : MonoBehaviour
{
    [Header("Instruction")]
    [SerializeField] private Button InstructionButton = null;

    [SerializeField] private GameObject InstructionPanel = null;

    // Start is called before the first frame update
    private void Start()
    {
        if (InstructionButton == null || InstructionPanel == null)
        {
            Debug.LogError("One or multiple field unset in InstuctionButton");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        InstructionPanel.SetActive(false);
        InstructionButton.onClick.AddListener(delegate { InstructionPressed(); });
    }

    private void InstructionPressed()
    {
        Vector3 currentScale = InstructionButton.transform.localScale;
        InstructionButton.transform.localScale = new(currentScale.x, -currentScale.y, currentScale.z);
        InstructionPanel.SetActive(!InstructionPanel.activeInHierarchy);
    }
}