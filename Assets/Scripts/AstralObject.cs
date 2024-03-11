using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Android;

public class AstralObject : MonoBehaviour
{
    //
    SphereCollider m_SphereCollider;
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

    public float sizeFactor
    { get { return m_SizeFactor; } private set { } }

    private Vector3 m_originalSize;

    private void ConvertUnits()
    {
        convertedMass = m_Mass * globals.amu2kg;
        convertedPosition = transform.position * (globals.adu2m * globals.unity2astronomy);
        convertedVelocity = m_Velocity * (globals.asu2ms);
    }

    private void Awake()
    {
        bool hasCollider = TryGetComponent<SphereCollider>(out m_SphereCollider);
        globals = FindObjectOfType<Globals>();
        globals.astralActors.Add(this);
        ConvertUnits();
        m_originalSize = transform.localScale;
        if (!hasCollider)
        {
            m_SphereCollider = gameObject.AddComponent<SphereCollider>();
            m_SphereCollider.center = Vector3.zero;
            m_SphereCollider.enabled = true;
            m_SphereCollider.radius = 1f;
        }
        m_SphereCollider.isTrigger = true;
    }
    private void OnDestroy()
    {
        globals.astralActors.Remove(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        //Here to compute the velocity at aphelion
        if (m_CenterOfEllipse)
        {
            m_Velocity.z = m_CenterOfEllipse.m_Velocity.z + Mathf.Sqrt(globals.universalGravityConst * m_CenterOfEllipse.convertedMass * (1f - m_Eccentricity) / ((convertedPosition - m_CenterOfEllipse.convertedPosition).magnitude * (1f + m_Eccentricity))) / globals.asu2ms;
        }
        convertedVelocity = m_Velocity * globals.asu2ms;
    }

    private void OnValidate()
    {
        //Security for 1st validation
        if (!globals)
            globals = FindObjectOfType<Globals>();

        ConvertUnits();
    }

    private void Update()
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
        //m_SphereCollider.radius = 1f / m_SizeFactor;
    }

    private void ProcessPosition()
    {
        if (!m_CenterOfEllipse || m_CenterOfEllipse == globals.sun)
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
    private void FixedUpdate()
    {
        ProcessPosition();
    }

    public void ComputeField(List<AstralObject> _everyActors)
    {
        convertedPosition += (convertedVelocity + 0.5f * acceleration * (globals.timeStep)) * (globals.timeStep);
        //Reposition the planet
        Vector3 newAcceleration = ComputeAcceleration(globals.astralActors);
        convertedVelocity += (acceleration + newAcceleration) * 0.5f * (globals.timeStep);
        acceleration = newAcceleration;
    }

    private Vector3 ComputeAcceleration(List<AstralObject> _everyActors)
    {
        Vector3 newAcceleration = Vector3.zero;
        foreach (AstralObject influence in _everyActors)
        {
            if (influence == this)
                continue;
            Vector3 oneStarToAnother = influence.convertedPosition - convertedPosition;
            newAcceleration += oneStarToAnother * (influence.convertedMass * Mathf.Pow(oneStarToAnother.sqrMagnitude, -1.5f));
        }
        newAcceleration *= globals.universalGravityConst;
        return newAcceleration;
    }

    private void OnMouseDown()
    {
        globals.mainCamera.GetComponent<CameraBehaviour>().PlanetClicked(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        AstralObject otherAstral;
        if (!other.gameObject.TryGetComponent(out otherAstral))
        {
            if (otherAstral.m_Mass > m_Mass)
            {
                float ratio = m_Mass / otherAstral.m_Mass;
                otherAstral.m_Mass += m_Mass;
                otherAstral.convertedMass += convertedMass;
                otherAstral.m_originalSize *= ratio;
                Destroy(this);
            }
            else
            {
                float ratio = otherAstral.m_Mass / m_Mass;
                m_Mass += otherAstral.m_Mass;
                convertedMass += otherAstral.convertedMass;
                m_originalSize *= ratio;
                Destroy(otherAstral);
            }
        }
    }
}
