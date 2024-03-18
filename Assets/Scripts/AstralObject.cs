using System.Collections.Generic;
using UnityEngine;

public class AstralObject : MonoBehaviour
{
    private SphereCollider m_SphereCollider;

    [SerializeField] private Globals m_Globals;

    [Tooltip("in astronomical mass unit")] public float mass = 1f;

    //In kilograms
    public float ConvertedMass { get; private set; }

    [SerializeField] private AstralObject m_CenterOfEllipse;

    public Vector3 velocity = Vector3.zero;
    [SerializeField, Min(0F)] private float m_Eccentricity;

    //In meters
    public Vector3 ConvertedPosition { get; private set; }

    public Vector3 ConvertedVelocity { get; private set; }

    public Vector3 Acceleration { get; private set; }

    //seems useless
    private float m_SizeFactor = 1f;

    public float SizeFactor
    { get { return m_SizeFactor; } private set { } }

    private Vector3 m_originalSize;

    public void ConvertUnits()
    {
        ConvertedMass = mass * m_Globals.amu2kg;
        ConvertedPosition = transform.position * (m_Globals.adu2m * m_Globals.unity2astronomy);
        ConvertedVelocity = velocity * (m_Globals.asu2ms);
    }

    private void Awake()
    {
        bool hasCollider = TryGetComponent(out m_SphereCollider);
        m_Globals = FindObjectOfType<Globals>();
        m_Globals.astralActors.Add(this);
        ConvertUnits();
        m_originalSize = transform.localScale;
        if (!hasCollider)
        {
            m_SphereCollider = gameObject.AddComponent<SphereCollider>();
            m_SphereCollider.center = Vector3.zero;
            m_SphereCollider.enabled = true;
            m_SphereCollider.radius = 1f;
        }
        gameObject.AddComponent<Rigidbody>();

        m_SphereCollider.isTrigger = true;
    }

    private void OnDestroy()
    {
        m_Globals.astralActors.Remove(this);
    }

    private void Start()
    {
        //Here to compute the velocity at aphelion
        if (m_CenterOfEllipse)
        {
            velocity.z =
                m_CenterOfEllipse.velocity.z + Mathf.Sqrt(
                m_Globals.universalGravityConst * m_CenterOfEllipse.ConvertedMass * (1f - m_Eccentricity)
                / ((ConvertedPosition - m_CenterOfEllipse.ConvertedPosition).magnitude * (1f + m_Eccentricity))
                ) / m_Globals.asu2ms;
        }
        ConvertedVelocity = velocity * m_Globals.asu2ms;
        Resize();
    }

    private void OnValidate()
    {
        //Security for 1st validation
        if (!m_Globals)
            m_Globals = FindObjectOfType<Globals>();

        ConvertUnits();
        print("Change a value");
    }

    private void Resize()
    {
        if (m_CenterOfEllipse && m_CenterOfEllipse != m_Globals.sun)
        {
            m_SizeFactor = m_CenterOfEllipse.m_SizeFactor;
        }
        else
        {
            m_SizeFactor = (m_Globals.mainCamera.transform.position - transform.position).magnitude;
            m_SizeFactor = Mathf.Clamp(m_SizeFactor, 0.1f / m_originalSize.magnitude, 10f / m_originalSize.magnitude);
        }
        transform.localScale = m_originalSize * m_SizeFactor;
        //m_SphereCollider.radius = 1f / m_SizeFactor;
    }

    private void ProcessPosition()
    {
        if (!m_CenterOfEllipse || m_CenterOfEllipse == m_Globals.sun)
        {
            transform.position = ConvertedPosition / (m_Globals.adu2m * m_Globals.unity2astronomy);
        }
        else
        {
            Vector3 orbitVector = ConvertedPosition - m_CenterOfEllipse.ConvertedPosition;
            transform.position = (m_CenterOfEllipse.ConvertedPosition + orbitVector * m_SizeFactor) / (m_Globals.adu2m * m_Globals.unity2astronomy);
        }
    }

    private void FixedUpdate()
    {
        ProcessPosition();
    }

    //Reposition the planet
    public void ComputeFieldPosition(List<AstralObject> _everyActors)
    {
        ConvertedPosition += (ConvertedVelocity + m_Globals.TimeStep * 0.5f * Acceleration) * m_Globals.TimeStep;
    }

    public void ComputeFieldForces(List<AstralObject> _everyActors)
    {
        Vector3 newAcceleration = ComputeAcceleration(m_Globals.astralActors);
        Vector3 vector3 = (Acceleration + newAcceleration);
        ConvertedVelocity += m_Globals.TimeStep * 0.5f * vector3;
        Acceleration = newAcceleration;
    }

    private Vector3 ComputeAcceleration(List<AstralObject> _everyActors)
    {
        Vector3 newAcceleration = Vector3.zero;
        foreach (AstralObject influence in _everyActors)
        {
            if (influence == this)
                continue;
            Vector3 oneStarToAnother = influence.ConvertedPosition - ConvertedPosition;
            newAcceleration += oneStarToAnother * (influence.ConvertedMass * Mathf.Pow(oneStarToAnother.sqrMagnitude, -1.5f));
        }
        newAcceleration *= m_Globals.universalGravityConst;
        return newAcceleration;
    }

    private void OnMouseDown()
    {
        m_Globals.mainCamera.GetComponent<CameraBehaviour>().PlanetClicked(transform);
        m_Globals.selectedActor = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Collision");
        if (other.gameObject.TryGetComponent(out AstralObject otherAstral))
        {
            if (otherAstral.mass > mass)
            {
                float ratio = mass / otherAstral.mass;
                otherAstral.mass += mass;
                otherAstral.ConvertedMass += ConvertedMass;
                otherAstral.m_originalSize *= ratio;
                Destroy(gameObject);
            }
            //else we let the smaller object handles the collision and thus its destruction
        }
    }
}