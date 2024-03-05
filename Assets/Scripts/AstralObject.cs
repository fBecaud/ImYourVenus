using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AstralObject : MonoBehaviour
{

    //

    [SerializeField] private Globals globals;
    [SerializeField, Tooltip("in astronomical mass unit")] private float m_Mass = 1f;
    //In kilograms
    public float convertedMass { get; private set; }
    //[SerializeField] private Vector3 m_Position= new Vector3( 1f, 0f, 0f );

    [SerializeField] private AstralObject m_CenterOfEllipse;

    [SerializeField] private Vector3 m_Velocity = Vector3.zero;
    [SerializeField, Min(0F)] private float m_Eccentricity;
    //In meters
    public Vector3 convertedPosition { get; private set; }

    public Vector3 convertedVelocity { get; private set; }

    public Vector3 acceleration { get; private set; }
    //seems useless
    //[SerializeField] private float m_Size {get { return m_Size; } set { transform.localScale = new Vector3(value, value, value); } }

    private void ConvertUnits()
    {
        convertedMass = m_Mass * globals.amu2kg;
        convertedPosition = transform.position * (globals.adu2m * globals.unity2astronomy);
        convertedVelocity = m_Velocity * globals.asu2ms;
    }

    private void Awake()
    {
        globals = FindObjectOfType<Globals>();
        globals.astralActors.Add(this);
        ConvertUnits();
    }
    // Start is called before the first frame update
    void Start()
    {
        //Here to compute the velocity at aphelion
        if (m_CenterOfEllipse)
        {
            m_Velocity.z = m_CenterOfEllipse.m_Velocity.z + Mathf.Sqrt(globals.universalGravityConst * m_CenterOfEllipse.convertedMass * (1F - m_Eccentricity) / ((convertedPosition - m_CenterOfEllipse.convertedPosition).magnitude * (1F + m_Eccentricity))) / globals.asu2ms;
        }
        convertedVelocity = m_Velocity * globals.asu2ms;
        //transform.localScale = new Vector3(m_Size, m_Size, m_Size);
    }
    private void OnValidate()
    {
        //Security for 1st validation
        if (!globals)
            globals = FindObjectOfType<Globals>();

        ConvertUnits();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        convertedPosition += (convertedVelocity + 0.5f * acceleration * globals.timeStep * Time.fixedDeltaTime) * globals.timeStep * Time.fixedDeltaTime;
        //Reposition the planet
        transform.position = convertedPosition / (globals.adu2m * globals.unity2astronomy);
        Vector3 newAcceleration = ComputeField(globals.astralActors);
        convertedVelocity += (acceleration + newAcceleration) * 0.5f * globals.timeStep * Time.fixedDeltaTime;
        acceleration = newAcceleration;
    }
    Vector3 ComputeField(List<AstralObject> _everyActors)
    {
        Vector3 newAcceleration = Vector3.zero;
        //TODO: TEST this
        foreach (AstralObject influence in _everyActors)
        {
            if (influence == this)
                continue;
            Vector3 oneStarToAnother = influence.convertedPosition - convertedPosition;
            newAcceleration += oneStarToAnother * (influence.convertedMass * Mathf.Pow(oneStarToAnother.sqrMagnitude, -1.5f));
        }
        newAcceleration = newAcceleration * globals.universalGravityConst;
        return newAcceleration;
    }
}
