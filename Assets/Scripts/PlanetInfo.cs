using System;
using System.Collections;
using System.Xml.Schema;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text InfoDisplay = null;

    [SerializeField] private GameObject DisplayDisplay = null; // Text displays
    [SerializeField] private TMP_Text PlanetNameDisplay = null;

    [SerializeField] private TMP_Text VelocityDisplay = null; // Values displays
    [SerializeField] private TMP_Text MassDisplay = null;
    [SerializeField] private TMP_Text PositionDisplay = null;

    [SerializeField] private Toggle OrbitToggle = null;
    [SerializeField] private Toggle VFieldToggle = null;
    [SerializeField] private Toggle LinesToggle = null;
    [SerializeField] private Toggle RotateToggle = null;

    public bool IsObitingActive
    { get { return OrbitToggle.isOn && IsFollowingPlanet; } }

    public bool IsFollowingPlanet
    { get { return Planet != null; } }

    public AstralObject Planet
    { get; private set; }

    private CameraBehaviour CameraScript = null;
    private VectorialFieldController VFController = null;

    private void Start()
    {
        if (InfoDisplay == null || DisplayDisplay == null || PlanetNameDisplay == null
            || OrbitToggle == null || VFieldToggle == null || LinesToggle == null || RotateToggle == null
            || VelocityDisplay == null || MassDisplay == null || PositionDisplay == null)
        {
            Debug.LogError("One or multiple field(s) unset in NewPlanetPlacer");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        CameraScript = FindAnyObjectByType<CameraBehaviour>();
        VFController = FindAnyObjectByType<VectorialFieldController>();

        SwitchModes();

        OrbitToggle.onValueChanged.AddListener(OrbitMode);
        VFieldToggle.onValueChanged.AddListener(VFieldMode);
        LinesToggle.onValueChanged.AddListener(LinesMode);
        RotateToggle.onValueChanged.AddListener(RotateMode);

        VFController.bDisplayField = false;
        VFController.bDisplayLines = false;
        VFController.bDisplayRotational = false;
    }

    private void OrbitMode(bool _newValue)
    {
        if (Planet && _newValue)
            StartCoroutine(CameraScript.LockCam());
        else if (!_newValue)
            CameraScript.UnlockCam();
    }

    private void VFieldMode(bool _newValue)
    {
        VFController.bDisplayField = _newValue;
    }

    private void LinesMode(bool _newValue)
    {
        VFController.bDisplayLines = _newValue;
    }

    private void RotateMode(bool _newValue)
    {
        VFController.bDisplayRotational = _newValue;
    }

    private void LateUpdate()
    {
        if (Planet)
            PlanetFollower();
    }

    private void PlanetFollower()
    {
        {
            Vector3 Velocity = Planet.ConvertedVelocity;
            string VelocityTxt =
                "X = " + Velocity.x + "\n" +
                "Z = " + Velocity.z;
            VelocityDisplay.text = VelocityTxt;
        }
        {
            float Mass = Planet.ConvertedMass;
            string MassTxt =
                "" + Mass + "\n";
            MassDisplay.text = MassTxt;
        }
        {
            Vector3 Position = Planet.ConvertedPosition;
            string PositionTxt =
                "X = " + Position.x + "\n" +
                "Z = " + Position.z;
            PositionDisplay.text = PositionTxt;
        }
    }

    private void FollowPlanet(AstralObject _planet)
    {
        SwitchModes();

        Planet = _planet;
        PlanetNameDisplay.text = _planet.name.ToUpper();

        PlanetFollower();
    }

    public void FollowPlanet(GameObject _planet)
    {
        if (_planet.TryGetComponent(out AstralObject AstralObj))
            FollowPlanet(AstralObj);
        else
            Debug.LogError("Follow Planet not an AstralObject");
    }

    public IEnumerator StopFollowPlanet()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();

        Planet = null;
        SwitchModes();

        VFController.bDisplayField = false;
        VFController.bDisplayLines = false;
        VFController.bDisplayRotational = false;
    }

    private void SwitchModes()
    {
        bool enable = Planet == null;

        InfoDisplay.enabled = enable;
        PlanetNameDisplay.gameObject.SetActive(!enable);
        DisplayDisplay.SetActive(!enable);
        VelocityDisplay.gameObject.SetActive(!enable);
        MassDisplay.gameObject.SetActive(!enable);
        PositionDisplay.gameObject.SetActive(!enable);
        OrbitToggle.gameObject.SetActive(!enable);
        VFieldToggle.gameObject.SetActive(!enable);
        LinesToggle.gameObject.SetActive(!enable);
        RotateToggle.gameObject.SetActive(!enable);
    }
}