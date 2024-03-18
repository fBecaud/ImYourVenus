using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class TimeController : MonoBehaviour
{
    [SerializeField] private Button PlayPauseButton = null;
    [SerializeField] private TMP_Dropdown DropdownTime = null;
    [SerializeField] private Slider SliderTime = null;

    private Globals Globals = null;
    [SerializeField] private TMP_Text ButtonTxt = null;
    [SerializeField] private TMP_Text SliderTxt = null;

    private bool IsPaused = true;

    private void Start()
    {
        if (PlayPauseButton == null || DropdownTime == null || SliderTime == null
            || ButtonTxt == null || SliderTxt == null)
        {
            Debug.LogError("One or multiple field(s) unset in TimeController");
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }

        PlayPauseButton.GetComponent<Button>().onClick.AddListener(OnPlayPauseTask);

        DropdownTime.onValueChanged.AddListener(OnDropChanged);
        SliderTime.onValueChanged.AddListener(OnSliderChanged);
        SliderTxt.text = "" + (int)SliderTime.value;

        Globals = FindFirstObjectByType<Globals>();
        TimeControl();
        Globals.StepPerTick = 0;
    }

    private void OnSliderChanged(float _newVal)
    {
        if (!IsPaused)
            Globals.StepPerTick = (int)_newVal;
        SliderTxt.text = "" + (int)SliderTime.value;
    }

    private void OnDropChanged(int _newVal)
    {
        if (!IsPaused)
            switch (_newVal)
            {
                case 0/* seconds */:
                    Globals.SetTimeStepSecond();
                    break;

                case 1/* minutes */:
                    Globals.SetTimeStepMinute();
                    break;

                case 2/* hours */:
                    Globals.SetTimeStepHour();
                    break;

                case 3/* days */:
                    Globals.SetTimeStepDay();
                    break;

                default:
                    Debug.LogError("TimeController TimeControl() case not handled");
                    break;
            }
    }

    private void TimeControl()
    {
        Globals.StepPerTick = (int)SliderTime.value;
        switch (DropdownTime.value)
        {
            case 0/* seconds */:
                Globals.SetTimeStepSecond();
                break;

            case 1/* minutes */:
                Globals.SetTimeStepMinute();
                break;

            case 2/* hours */:
                Globals.SetTimeStepHour();
                break;

            case 3/* days */:
                Globals.SetTimeStepDay();
                break;

            default:
                Debug.LogError("TimeController TimeControl() case not handled");
                break;
        }
    }

    private void OnPlayPauseTask()
    {
        IsPaused = !IsPaused;
        if (!IsPaused)
        {
            ButtonTxt.text = "Pause";
            TimeControl();
        }
        else
        {
            ButtonTxt.text = "Play";
            Globals.StepPerTick = 0;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#endif
            Application.Quit();
        }
    }
}