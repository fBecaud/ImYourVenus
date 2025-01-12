using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class NewPlanetPlacer : MonoBehaviour
{
    // UI VARS
    [SerializeField] private Button PlaceButton = null;

    [SerializeField] private Button CancelButton = null;

    [SerializeField] private TMP_InputField NewMassInput = null;
    [SerializeField] private TMP_InputField NewSpeedInputX = null;
    [SerializeField] private TMP_InputField NewSpeedInputZ = null;

    public bool isAsteroidPlacer = false;

    private float NewMass
    {
        get
        {
            string input = NewMassInput.text.ToString();
            return (float)(input.Length > 0 ? System.Convert.ToDouble(input) : 0f);
        }
    }

    private Vector3 NewSpeed
    {
        get
        {
            string inputX = NewSpeedInputX.text.ToString();
            string inputZ = NewSpeedInputZ.text.ToString();
            return new(
                (float)(inputX.Length > 0 ? System.Convert.ToDouble(inputX) : 0f),
                0f,
                (float)(inputZ.Length > 0 ? System.Convert.ToDouble(inputZ) : 0f)
                );
        }
    }

    [SerializeField] private TMP_Text ErrorTxt = null;
    [SerializeField] private float ErrorDisplayTime = 8f;

    [SerializeField] private TMP_Text InfoTxt = null;

    // NEW PLANETS VAR
    //[SerializeField] private GameObject NewPlanetPrefab = null;

    private GameObject NewPlanetToPlace = null;
    [SerializeField] private System.Collections.Generic.List<GameObject> NewPlanetsPrefabs;
    private System.Collections.Generic.List<GameObject> NewPlanetsPrefabs_ToUse;

    private int NewPlanetId = -1;

    [SerializeField] private Vector3 NewPlanetScale = new(0.08f, 0.08f, 0.08f);

    // Trails
    [SerializeField] private GameObject TrailsPrefab = null;

    public bool IsPlacingPlanet { get; private set; }

    // true = Placement Mode (starting mode)
    private const bool PlacementMode = true;

    // false = Cancel Mode (when place button pressed)
    private const bool CancelMode = false;

    private void Start()
    {
        if (PlaceButton == null || CancelButton == null
            || ErrorTxt == null || InfoTxt == null
            || NewMassInput == null || NewSpeedInputX == null || NewSpeedInputZ == null
            || TrailsPrefab == null
            || NewPlanetsPrefabs.Count == 0)
        {
            Debug.LogError("One or multiple field(s) unset in NewPlanetPlacer");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        PlaceButton.GetComponent<Button>().onClick.AddListener(OnPlaceTask);
        CancelButton.GetComponent<Button>().onClick.AddListener(OnCancelTask);

        CancelButton.gameObject.SetActive(false);
        ErrorTxt.gameObject.SetActive(false);
        InfoTxt.gameObject.SetActive(false);

        FillPrefabToUse();
    }

    private void FillPrefabToUse()
    {
        NewPlanetsPrefabs_ToUse = new(NewPlanetsPrefabs);
    }

    private void OnPlaceTask()
    {
        // Check if any field is empty
        if (NewMass == 0 || NewSpeed.magnitude == 0f)
        {
            StartCoroutine(ErrorDisplay());
            return;
        }
        // else proceed

        ErrorTxt.gameObject.SetActive(false);
        SwitchModes(CancelMode);

        NewPlanetId = Random.Range(0, NewPlanetsPrefabs_ToUse.Count - 1);

        NewPlanetToPlace = Instantiate(NewPlanetsPrefabs_ToUse[NewPlanetId]);
        NewPlanetToPlace.name = names[Random.Range(0, names.Length - 1)];

        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        NewPlanetToPlace.transform.position =
            new Vector3(MouseWorldPos.x, 0f, MouseWorldPos.z);
        NewPlanetToPlace.transform.localScale = NewPlanetScale * NewMass;
        NewPlanetToPlace.SetActive(true);

        StartCoroutine(FollowMouse());
    }

    private IEnumerator ErrorDisplay()
    {
        ErrorTxt.gameObject.SetActive(true);
        yield return new WaitForSeconds(ErrorDisplayTime);
        ErrorTxt.gameObject.SetActive(false);
    }

    private IEnumerator FollowMouse()
    {
        while (NewPlanetToPlace != null)
        {
            {
                Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane Plane = new(Vector3.up, 0);

                if (Plane.Raycast(Ray, out float distance))
                {
                    var WorldPosition = Ray.GetPoint(distance);
                    NewPlanetToPlace.transform.position = new(WorldPosition.x, 0f, WorldPosition.z);
                }
            }

            // Right mouse button pressed -> place planet
            if (Input.GetMouseButton(1))
            {
                var AstralScript = NewPlanetToPlace.AddComponent<AstralObject>();
                AstralScript.mass = NewMass;
                AstralScript.velocity = NewSpeed;
                AstralScript.ConvertUnits();
                AstralScript.isAsteroid = isAsteroidPlacer;

                _ = Instantiate(TrailsPrefab, NewPlanetToPlace.transform);

                NewPlanetToPlace = null;
                InfoTxt.gameObject.SetActive(false);

                NewPlanetsPrefabs_ToUse.Remove(NewPlanetsPrefabs_ToUse[NewPlanetId]);
                NewPlanetId = -1;
                if (NewPlanetsPrefabs_ToUse.Count == 0)
                    FillPrefabToUse();

                SphereCollider SC;
                if (AstralScript.TryGetComponent(out SC))
                    SC.radius = 0.5f;

                SwitchModes(PlacementMode);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void OnCancelTask()
    {
        Destroy(NewPlanetToPlace);
        NewPlanetToPlace = null;

        SwitchModes(PlacementMode);
    }

    // bool PlacementMode (starting mode)
    // bool CancelMode (when place button pressed)
    private void SwitchModes(bool Mode)
    {
        IsPlacingPlanet = !Mode;

        InfoTxt.gameObject.SetActive(!Mode);
        NewMassInput.interactable = NewSpeedInputX.interactable =
            NewSpeedInputZ.interactable = Mode;

        PlaceButton.gameObject.SetActive(Mode);
        CancelButton.gameObject.SetActive(!Mode);
    }

    private readonly string[] names =
    {
        "Dedrephus",
        "Bumiter",
        "Vogrion",
        "Tostrore",
        "Niuphus",
        "Puiter",
        "Gnoiwei",
        "Strigoclite",
        "Stryke O73",
        "Ceron G13",
        "Reconides",
        "Pebounides",
        "Xenkarth",
        "Pagade",
        "Uclite",
        "Doagantu",
        "Bremonides",
        "Phoponus",
        "Llarvis 84O7",
        "Liuq 3Z5",
        "Vunvunides",
        "Vingethea",
        "Pastrapus",
        "Otruna",
        "Augantu",
        "Xowei",
        "Gralolia",
        "Stragotania",
        "Phorix 8",
        "Lagua 2",
        "Kilnuehiri",
        "Onvephus",
        "Tholmiuq",
        "Mebburn",
        "Iatera",
        "Uaturn",
        "Llisuzuno",
        "Nipogawa",
        "Dichi 14Q",
        "Losie RWX"
    };
}