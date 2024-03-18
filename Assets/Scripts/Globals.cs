using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    public Camera mainCamera;

    [Tooltip("astronomical distance unit per distance unit in unity")] public float unity2astronomy = 100F;

    [Tooltip("astronomical mass unit to kilogram")] public float amu2kg = 5.972e24F;

    [Tooltip("astronomical distance unit to meters")] public float adu2m = 1.495978707e11F;

    [Tooltip("astronomical speed unit to meters per second")] public float asu2ms = 1.0e3F;

    public float universalGravityConst = 6.6743e-11F;

    public List<AstralObject> astralActors;
    public AstralObject selectedActor;
    public AstralObject sun;

    private enum TimeDivision
    {
        second, minute, hour, day
    }

    [SerializeField] private TimeDivision m_TimeDivision;

    [Range(0, 1500), Tooltip("How many timedivision by tick (0.2 seconds)")] public int StepPerTick = 1;

    private float m_TimeStep = 3600f * 50f; // /!\ CAREFULL: this is not a standard DeltaTime

    public float TimeStep
    { get { return m_TimeStep; } } // /!\ CAREFULL: this is not a standard DeltaTime

    public void SetTimeStepSecond()
    {
        m_TimeStep = 1f;
    }

    public void SetTimeStepMinute()
    {
        m_TimeStep = 60f;
    }

    //From here too quick
    public void SetTimeStepHour()
    {
        m_TimeStep = 3600f;
    }

    public void SetTimeStepDay()
    {
        m_TimeStep = 24f * 3600f;
    }

    //Too quick
    public void SetTimeStepMonth()
    {
        m_TimeStep = 30f * 24f * 3600f;
    }

    private void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    /*
    private void OnValidate()
    {
        switch (m_TimeDivision)
        {
            case (TimeDivision.second):
                SetTimeStepSecond();
                break;

            case (TimeDivision.minute):
                SetTimeStepMinute();
                break;

            case (TimeDivision.hour):
                SetTimeStepHour();
                break;

            case (TimeDivision.day):
                SetTimeStepDay();
                break;

            default:
                break;
        }
    }
    */

    private void FixedUpdate()
    {
        for (int i = 0; i < StepPerTick; i++)
        {
            foreach (AstralObject obj in astralActors)
                obj.ComputeFieldPosition(astralActors);

            foreach (AstralObject obj in astralActors)
                obj.ComputeFieldForces(astralActors);
        }
    }
}