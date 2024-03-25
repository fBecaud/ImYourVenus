using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text InfoDisplay = null;

    [SerializeField] private GameObject DisplayDisplay = null;

    [SerializeField] private TMP_Text VelocityDisplay = null;
    [SerializeField] private TMP_Text MassDisplay = null;
    [SerializeField] private TMP_Text PositionDisplay = null;

    [SerializeField] private Toggle OrbitToggle = null;

    public bool IsObitingActive
    { get { return OrbitToggle.isOn && IsFollowingPlanet; } }

    public bool IsFollowingPlanet
    { get { return Planet != null; } }

    public AstralObject Planet
    { get; private set; }

    private CameraBehaviour CameraScript = null;

    private void Start()
    {
        if (InfoDisplay == null || DisplayDisplay == null || OrbitToggle == null
            || VelocityDisplay == null || MassDisplay == null || PositionDisplay == null)
        {
            Debug.LogError("One or multiple field(s) unset in NewPlanetPlacer");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        CameraScript = FindAnyObjectByType<CameraBehaviour>();

        SwitchModes();

        OrbitToggle.onValueChanged.AddListener(OrbitMode);
    }

    private void OrbitMode(bool _newValue)
    {
        if (Planet && _newValue)
            StartCoroutine(CameraScript.LockCam());
        else if (!_newValue)
            CameraScript.UnlockCam();
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

    public void FollowPlanet(AstralObject _planet)
    {
        Planet = _planet;
        SwitchModes();
    }

    public void FollowPlanet(GameObject _planet)
    {
        if (_planet.TryGetComponent(out AstralObject AstralObj))
            FollowPlanet(AstralObj);
    }

    public IEnumerator StopFollowPlanet()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        Planet = null;
        SwitchModes();
    }

    private void SwitchModes()
    {
        bool enable = Planet == null;

        InfoDisplay.enabled = enable;
        DisplayDisplay.SetActive(!enable);
        VelocityDisplay.gameObject.SetActive(!enable);
        MassDisplay.gameObject.SetActive(!enable);
        PositionDisplay.gameObject.SetActive(!enable);
        OrbitToggle.gameObject.SetActive(!enable);
    }
}