using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;

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
    private float m_SizeFactor = 1f;
    public float sizeFactor { get { return m_SizeFactor; } private set { } }
    private Vector3 m_originalSize;

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
        m_originalSize = transform.localScale;
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
    void Update()
    {
        Resize();
    }

    private void Resize()
    {
        if (m_CenterOfEllipse && m_CenterOfEllipse != globals.sun)
        {
            m_SizeFactor = m_CenterOfEllipse.m_SizeFactor;
        }
        else
        {
        m_SizeFactor = (globals.mainCamera.transform.position - transform.position).magnitude;
        m_SizeFactor = Mathf.Clamp(m_SizeFactor, 0.1f / m_originalSize.magnitude, 10f / m_originalSize.magnitude);
        }
        transform.localScale = m_originalSize * m_SizeFactor;

    }
    private void ProcessPosition()
    {
        if (!m_CenterOfEllipse)
        {
            return;
        }
        if (m_CenterOfEllipse == globals.sun)
        {
            transform.position = convertedPosition / (globals.adu2m * globals.unity2astronomy);
        }
        else
        {
            Vector3 orbitVector = convertedPosition - m_CenterOfEllipse.convertedPosition;
            transform.position = (m_CenterOfEllipse.convertedPosition + orbitVector * m_SizeFactor) / (globals.adu2m * globals.unity2astronomy);
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        ProcessPosition();
    }
    public void ComputeField(List<AstralObject> _everyActors)
    {
        convertedPosition += (convertedVelocity + 0.5f * acceleration * globals.timeStep * Time.fixedDeltaTime) * globals.timeStep * Time.fixedDeltaTime;
        //Reposition the planet
        Vector3 newAcceleration = ComputeAcceleration(globals.astralActors);
        convertedVelocity += (acceleration + newAcceleration) * 0.5f * globals.timeStep * Time.fixedDeltaTime;
        acceleration = newAcceleration;
    }

    Vector3 ComputeAcceleration(List<AstralObject> _everyActors)
    {
        Vector3 newAcceleration = Vector3.zero;
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
