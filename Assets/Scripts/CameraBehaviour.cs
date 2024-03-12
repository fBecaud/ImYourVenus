using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBehaviour : MonoBehaviour
{
    private Transform Parent = null;

    private FreeFlyCamera FreeCam = null;

    [SerializeField] private TMP_Text LockedText = null;
    [SerializeField] private TMP_Text FreeText = null;

    private bool IsLockedIn
    {
        get { return LockedText.enabled && Parent != null; }
        set { LockedText.enabled = value; }
    }

    [SerializeField] private GameObject NewPlanetPlacerScript = null;

    [SerializeField] private GameObject PlanetInfoScript = null;

    private void Start()
    {
        FreeCam = GetComponent<FreeFlyCamera>();

        if (FreeCam == null
            || PlanetInfoScript == null || NewPlanetPlacerScript == null
            || LockedText == null || FreeText == null)
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
        if (IsLockedIn)
        {
            if (Input.GetMouseButtonDown(0)) // 0 -> left button
                UnlockCam();
            else
                transform.LookAt(Parent.position);
        }

        if ((Input.GetMouseButtonDown(1) || Input.GetMouseButtonUp(1))) // 1 -> right button
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
        if (NewPlanetPlacerScript.GetComponent<NewPlanetPlacer>().IsPlacingPlanet)
            return;

        Parent = _newDaddy;

        if (!IsLockedIn)
            StartCoroutine(LockCam());
    }

    private IEnumerator LockCam()
    {
        yield return new WaitForEndOfFrame();
        IsLockedIn = true;
        //SwitchFreeCamMode();

        PlanetInfoScript.GetComponent<PlanetInfo>().FollowPlanet(Parent.gameObject);
    }

    private void UnlockCam()
    {
        if (IsLockedIn)
        {
            IsLockedIn = false;
            Parent = null;

            PlanetInfoScript.GetComponent<PlanetInfo>().StopFollowPlanet();
        }
    }
}