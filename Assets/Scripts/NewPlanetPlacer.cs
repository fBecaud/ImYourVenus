using System.Collections;
using UnityEngine;
using TMPro;
using UnityEditor;
using UnityEngine.UI;

public class NewPlanetPlacer : MonoBehaviour
{
    [Header("New Planet")]
    // UI VARS
    [SerializeField] private Button PlaceButton = null;

    [SerializeField] private Button CancelButton = null;

    [SerializeField] private TMP_InputField NewWeight = null;
    [SerializeField] private TMP_InputField NewSpeed = null;

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

    public bool IsPlacingPlanet { get; private set; }

    // true = Placement Mode (starting mode)
    private const bool PlacementMode = true;

    // false = Cancel Mode (when place button pressed)
    private const bool CancelMode = false;

    private void Start()
    {
        if (PlaceButton == null || CancelButton == null
            || ErrorTxt == null || InfoTxt == null
            || NewWeight == null || NewSpeed == null
            || NewPlanetsPrefabs.Count == 0)
        {
            Debug.LogError("One or multiple field unset in UI_NewPlanet");
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
        NewPlanetsPrefabs_ToUse = NewPlanetsPrefabs;
    }

    private void OnPlaceTask()
    {
        // Check if any field is empty
        if (NewWeight.text.Length == 0 || NewSpeed.text.Length == 0)
        {
            ErrorTxt.gameObject.SetActive(true);
            StartCoroutine(ClearErrorTxt());
            return;
        }
        // else proceed

        ErrorTxt.gameObject.SetActive(false);
        SwitchModes(CancelMode);

        if (NewPlanetsPrefabs_ToUse.Count == 0)
            FillPrefabToUse();

        NewPlanetId = Random.Range(0, NewPlanetsPrefabs_ToUse.Count);

        NewPlanetToPlace = Instantiate(NewPlanetsPrefabs_ToUse[NewPlanetId]);

        Vector3 MouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        NewPlanetToPlace.transform.position =
            new Vector3(MouseWorldPos.x, 0f, MouseWorldPos.z);
        NewPlanetToPlace.transform.localScale = NewPlanetScale;

        StartCoroutine(FollowMouse());
    }

    private IEnumerator ClearErrorTxt()
    {
        yield return new WaitForSeconds(ErrorDisplayTime);
        ErrorTxt.gameObject.SetActive(false);
    }

    private IEnumerator FollowMouse()
    {
        while (NewPlanetToPlace != null)
        {
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane plane = new(Vector3.up, 0);

                if (plane.Raycast(ray, out float distance))
                {
                    var WorldPosition = ray.GetPoint(distance);
                    NewPlanetToPlace.transform.position = new(WorldPosition.x, 0f, WorldPosition.z);
                }
            }

            // Right mouse button pressed -> place planet
            if (Input.GetMouseButton(1))
            {
                //NewPlanetToPlace.GetComponent<PGSolidPlanet>().RandomizePlanet(true); // delete if never used
                var AstralScript = NewPlanetToPlace.AddComponent<AstralObject>();

                // TODO set script vars

                NewPlanetToPlace = null;
                InfoTxt.gameObject.SetActive(false);

                NewPlanetsPrefabs_ToUse.Remove(NewPlanetsPrefabs_ToUse[NewPlanetId]);
                NewPlanetId = -1;

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

    private void SwitchModes(bool Mode)
    // bool PlacementMode (starting mode)
    // bool CancelMode (when place button pressed)
    {
        IsPlacingPlanet = !Mode;

        InfoTxt.gameObject.SetActive(!Mode);
        NewWeight.interactable = NewSpeed.interactable = Mode;

        PlaceButton.gameObject.SetActive(Mode);
        CancelButton.gameObject.SetActive(!Mode);
    }
}