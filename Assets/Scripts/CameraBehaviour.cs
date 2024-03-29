using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Camera))]
public class CameraBehaviour : MonoBehaviour
{
    private Vector3 Offset = Vector3.zero;
    private FreeFlyCamera FreeCam = null;

    [SerializeField] private TMP_Text LockedText = null;
    [SerializeField] private TMP_Text FreeText = null;

    [SerializeField] private GameObject NewPlanetPlacerScriptLocation = null;

    [SerializeField] private GameObject PlanetInfoScriptLocation = null;

    private NewPlanetPlacer NewPlanetPlacerScript = null;
    private PlanetInfo PlanetInfoScript = null;

    private Transform ParentPlanet = null;

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

        NewPlanetPlacerScript = NewPlanetPlacerScriptLocation.GetComponent<NewPlanetPlacer>();
        PlanetInfoScript = PlanetInfoScriptLocation.GetComponent<PlanetInfo>();

        if (NewPlanetPlacerScript == null || PlanetInfoScript == null)
        {
            Debug.LogError("One or multiple scripts not found in CameraBehaviour");
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
        if (PlanetInfoScript.IsFollowingPlanet)
        {
            if (!EventSystem.current.IsPointerOverGameObject()
                && Input.GetMouseButtonDown(0/*left button*/)
                && !(LockPlanet_IsRunning || UnlockPlanet_IsRunning)
                )
                StartCoroutine(UnlockPlanet());
            else if (ParentPlanet && PlanetInfoScript.IsObitingActive)
            {
                transform.LookAt(ParentPlanet.position);
                if (FreeCam.enabled)
                    Offset = transform.position - ParentPlanet.position;
            }
        }

        if (!FreeCam.enabled
            && Input.GetMouseButtonDown(1/*right button*/)
            )
            SwitchFreeCamMode();
        else if (FreeCam.enabled
            && Input.GetMouseButtonUp(1/*right button*/)
            )
            SwitchFreeCamMode();
    }

    private void FixedUpdate()
    {
        if (PlanetInfoScript.IsObitingActive
            && !FreeCam.enabled
            && ParentPlanet
            )
            transform.position = ParentPlanet.position + Offset; // needs to be in fixedUpdate for smooth camera mouvements
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
            //Cursor.lockState = CursorLockMode.Locked; // done by script
            Cursor.visible = false;
            FreeText.enabled = true;
        }
    }

    public void PlanetClicked(Transform _newDaddy)
    {
        if (NewPlanetPlacerScript.IsPlacingPlanet)
            return;

#if UNITY_EDITOR
        if (_newDaddy == null)
        {
            Debug.LogError("PlanetClicked called but new daddy is null");
            return;
        }
#endif
        if (UnlockPlanet_IsRunning)
            StopCoroutine(UnlockPlanet());

        ParentPlanet = _newDaddy;

        if (PlanetInfoScript.IsObitingActive)
            StartCoroutine(LockCam());
        else
            StartCoroutine(LockPlanet());
    }

    public bool LockCam_IsRunning { get; private set; }

    public IEnumerator LockCam()
    {
        LockCam_IsRunning = true;
        StartCoroutine(LockPlanet());
        yield return new WaitForEndOfFrame();
        LockedText.enabled = true;
        FreeCam._enableRotation = false;
        LockCam_IsRunning = false;
    }

    public bool LockPlanet_IsRunning { get; private set; }

    private IEnumerator LockPlanet()
    {
        LockPlanet_IsRunning = true;
        yield return new WaitForEndOfFrame();
        PlanetInfoScript.FollowPlanet(ParentPlanet.gameObject);
        Offset = transform.position - ParentPlanet.transform.position;
        LockPlanet_IsRunning = false;
    }

    public void UnlockCam()
    {
        FreeCam._enableRotation = true;
        LockedText.enabled = false;
    }

    public bool UnlockPlanet_IsRunning { get; private set; }

    private IEnumerator UnlockPlanet()
    {
        UnlockPlanet_IsRunning = true;
        yield return new WaitForEndOfFrame();
        StartCoroutine(PlanetInfoScript.StopFollowPlanet());
        if (PlanetInfoScript.IsObitingActive)
            UnlockCam();
        ParentPlanet = null;
        UnlockPlanet_IsRunning = false;
    }
}