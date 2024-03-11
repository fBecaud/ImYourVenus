using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private Transform Parent = null;

    private FreeFlyCamera FreeCam = null;

    private bool IsLockedIn = false;

    [SerializeField] private TMP_Text LockedText = null;
    [SerializeField] private TMP_Text FreeText = null;

    private void Start()
    {
        FreeCam = GetComponent<FreeFlyCamera>();

        if (FreeCam == null || LockedText == null || FreeText == null)
        {
            Debug.LogError("One or multiple field unset in CameraBehaviour");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        LockedText.enabled = false;
        FreeText.enabled = false;
        FreeCam.enabled = false;
    }

    private void Update()
    {
        if (IsLockedIn && Parent != null)
            transform.position =
                new(Parent.position.x, transform.position.y, Parent.position.z);

        if (IsLockedIn && Parent != null && Input.GetMouseButtonDown(0)) //Input.GetKeyDown(KeyCode.Escape))
            UnlockCam();

        if (Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))
            SwitchFreeCamMode();
    }

    public void SwitchFreeCamMode()
    {
        if (FreeCam.enabled) // If freecam enabled -> disable it
        {
            FreeCam.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            FreeText.enabled = false;
        }
        else
        {
            FreeCam.enabled = true;
            //Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            FreeText.enabled = true;
        }
    }

    public void PlanetClicked(Transform _newDaddy)
    {
        Parent = _newDaddy;

        StartCoroutine(LockCam());
    }

    private IEnumerator LockCam()
    {
        yield return new WaitForEndOfFrame();
        IsLockedIn = true;
        LockedText.enabled = true;
    }

    private void UnlockCam()
    {
        IsLockedIn = false;
        LockedText.enabled = false;

        Parent = null;
    }
}