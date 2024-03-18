using TMPro;
using UnityEditor;
using UnityEngine;

public class PlanetInfo : MonoBehaviour
{
    [SerializeField] private TMP_Text InfoDisplay = null;

    [SerializeField] private GameObject DisplayDisplay = null;

    [SerializeField] private TMP_Text VelocityDisplay = null;
    [SerializeField] private TMP_Text MassDisplay = null;
    [SerializeField] private TMP_Text PositionDisplay = null;

    private AstralObject Planet = null;

    private void Start()
    {
        if (InfoDisplay == null || DisplayDisplay == null
            || VelocityDisplay == null || MassDisplay == null || PositionDisplay == null)
        {
            Debug.LogError("One or multiple field(s) unset in NewPlanetPlacer");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
        SwitchModes();
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

    public void StopFollowPlanet()
    {
        Planet = null;
        SwitchModes();
    }

    private void SwitchModes()
    {
        bool enable = Planet == null;

        InfoDisplay.enabled = enable;
        DisplayDisplay.SetActive(!enable);
        VelocityDisplay.enabled = !enable;
        MassDisplay.enabled = !enable;
        PositionDisplay.enabled = !enable;
    }
}