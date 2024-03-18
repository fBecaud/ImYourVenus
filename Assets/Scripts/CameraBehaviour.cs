using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBehaviour : MonoBehaviour
{
    private Transform Parent = null;
    private Vector3 Offset = Vector3.zero;
    private FreeFlyCamera FreeCam = null;

    [SerializeField] private TMP_Text LockedText = null;
    [SerializeField] private TMP_Text FreeText = null;

    private bool IsLockedIn
    {
        get { return LockedText.enabled && Parent != null; }
        set { LockedText.enabled = value; }
    }

    [SerializeField] private GameObject NewPlanetPlacerScriptLocation = null;

    [SerializeField] private GameObject PlanetInfoScriptLocation = null;

    private void Start()
    {
        FreeCam = GetComponent<FreeFlyCamera>();

        if (FreeCam == null
            || PlanetInfoScriptLocation == null || NewPlanetPlacerScriptLocation == null
            || LockedText == null || FreeText == null)
        {
            Debug.LogError("One or multiple field(s) unset in CameraBehaviour");
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
            if (Input.GetMouseButtonDown(0) && transform.position - Parent.position != Offset) // 0 -> left button
                UnlockCam();
            else
            {
                transform.LookAt(Parent.position);
                if (FreeCam.enabled)
                    Offset = transform.position - Parent.position;
            }
        }

        if (!FreeCam.enabled && Input.GetMouseButtonDown(1)) // 1 -> right button
            SwitchFreeCamMode();
        else if (FreeCam.enabled && Input.GetMouseButtonUp(1))
            SwitchFreeCamMode();
    }

    private void FixedUpdate()
    {
        if (IsLockedIn && !FreeCam.enabled) // needs to be in fixedUpdate for smooth camera mouvements
            transform.position = Parent.position + Offset;
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
        if (NewPlanetPlacerScriptLocation.GetComponent<NewPlanetPlacer>().IsPlacingPlanet)
            return;

        Parent = _newDaddy;
        Offset = transform.position - Parent.transform.position;

        StartCoroutine(LockCam());
    }

    private IEnumerator LockCam()
    {
        yield return new WaitForEndOfFrame();
        IsLockedIn = true;
        //SwitchFreeCamMode();
        FreeCam._enableRotation = false;

        PlanetInfoScriptLocation.GetComponent<PlanetInfo>().FollowPlanet(Parent.gameObject);
    }

    private void UnlockCam()
    {
        if (IsLockedIn)
        {
            IsLockedIn = false;
            Parent = null;

            FreeCam._enableRotation = true;
            PlanetInfoScriptLocation.GetComponent<PlanetInfo>().StopFollowPlanet();
        }
    }
}