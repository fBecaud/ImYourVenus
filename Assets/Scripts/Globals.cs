using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static UnityEngine.GraphicsBuffer;
using UnityEditor;
using Unity.Jobs;


public class Globals : MonoBehaviour
{
    //TODO: Maybe set the 3 next var in read only
    [SerializeField] public Camera mainCamera;
    [SerializeField, Tooltip("astronomical distance unit per distance unit in unity")] public float unity2astronomy = 0.01F;

    //astronomical mass unit to kilogram
    [SerializeField, Tooltip("astronomical mass unit to kilogram")] public float amu2kg = 5.972e24F;
    //astronomical distance unit to meters
    [SerializeField, Tooltip("astronomical distance unit to meters")] public float adu2m = 1.495978707e11F;
    //astronomical speed unit to meters per second
    [SerializeField, Tooltip("astronomical speed unit to meters per second")] public float asu2ms = 1.0e3F;
    [SerializeField] public float universalGravityConst = 6.6743e-11F;

    [SerializeField] public List<AstralObject> astralActors;
    [SerializeField] public AstralObject sun;

    enum TimeDivision
    {
        second, minute, hour, day
    }
    [SerializeField]
    TimeDivision m_TimeDivision;
    private float m_TimeStep = 3600f * 50f; // /!\ CARE: this is not a standard DeltaTime
    [SerializeField, Range(0, 50), Tooltip("How many timedivision by tick (0.2 seconds)")] private int m_StepPerTick = 1;
    public float timeStep { get { return m_TimeStep; } } // /!\ CARE: this is not a standard DeltaTime


    public void SetTimeStepSecond()
    {
        m_TimeStep = 1f / Time.fixedDeltaTime;
    }
    public void SetTimeStepMinute()
    {
        m_TimeStep = 60f / Time.fixedDeltaTime;
    }
    public void SetTimeStepHour()
    {
        m_TimeStep = 3600f / Time.fixedDeltaTime;
    }
    public void SetTimeStepDay()
    {
        m_TimeStep = 24f * 3600f / Time.fixedDeltaTime;
    }
    //Too quick
    public void SetTimeStepMonth()
    {
        m_TimeStep = 30f * 24f * 3600f / Time.fixedDeltaTime;
    }
    // Start is called before the first frame update
    void Awake()
    {
        mainCamera = FindObjectOfType<Camera>();
    }

    void OnValidate()
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
    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < m_StepPerTick; i++)
        {
            foreach (AstralObject obj in astralActors)
                obj.ComputeField(astralActors);
        }
    }

}
