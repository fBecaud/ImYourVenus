using System.Collections.Generic;
using UnityEngine;

public class AstralObject : MonoBehaviour
{
    private SphereCollider m_SphereCollider;

    [SerializeField] private Globals globals;
    [SerializeField] private VectorialFieldController m_VectorialField;

    AsteroidFieldGenerator m_AsteroidFieldGenerator;

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
    { get { return m_SizeFactor; } private set { m_SizeFactor = value; } }

    private Vector3 m_originalSize;

    public bool isAsteroid = false;
    public void ConvertUnits()
    {
        ConvertedMass = mass * globals.amu2kg;
        ConvertedPosition = transform.position * (globals.adu2m * globals.unity2astronomy);
        ConvertedVelocity = velocity * (globals.asu2ms);
    }

    private void Awake()
    {
        bool hasCollider = TryGetComponent(out m_SphereCollider);
        if (!globals)
            globals = FindObjectOfType<Globals>();
        if (!m_VectorialField)
            m_VectorialField = FindObjectOfType<VectorialFieldController>();
        if (!m_AsteroidFieldGenerator)
            m_AsteroidFieldGenerator = FindObjectOfType<AsteroidFieldGenerator>();

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
        Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
        rigidbody.isKinematic = true;
        m_SphereCollider.isTrigger = true;
    }

    private void OnDestroy()
    {
        globals.astralActors.Remove(this);
        if(isAsteroid) 
        {
            m_AsteroidFieldGenerator.Asteroids.Remove(this.gameObject);
        }
    }

    private void Start()
    {
        //Here to compute the velocity at aphelion
        if (m_CenterOfEllipse)
        {
            velocity.z =
                m_CenterOfEllipse.velocity.z + Mathf.Sqrt(
                globals.universalGravityConst * m_CenterOfEllipse.ConvertedMass * (1f - m_Eccentricity)
                / ((ConvertedPosition - m_CenterOfEllipse.ConvertedPosition).magnitude * (1f + m_Eccentricity))
                ) / globals.asu2ms;
        }
        ConvertedVelocity = velocity * globals.asu2ms;
        if (!isAsteroid)
            Resize();
    }

    private void OnValidate()
    {
        //Security for 1st validation
        if (!globals)
            globals = FindObjectOfType<Globals>();

        ConvertUnits();
        print("Change a value");
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
        if (!m_CenterOfEllipse || m_CenterOfEllipse == globals.sun)
        {
            transform.position = ConvertedPosition / (globals.adu2m * globals.unity2astronomy);
        }
        else
        {
            Vector3 orbitVector = ConvertedPosition - m_CenterOfEllipse.ConvertedPosition;
            transform.position = (m_CenterOfEllipse.ConvertedPosition + orbitVector * m_SizeFactor) / (globals.adu2m * globals.unity2astronomy);
        }
    }

    private void FixedUpdate()
    {
        ProcessPosition();

    }

    //Reposition the planet
    public void ComputeFieldPosition(List<AstralObject> _everyActors)
    {
        ConvertedPosition += (ConvertedVelocity + globals.TimeStep * 0.5f * Acceleration) * globals.TimeStep;
    }

    public void ComputeFieldForces(List<AstralObject> _everyActors)
    {
        Vector3 newAcceleration = ComputeAcceleration(globals.astralActors);
        Vector3 vector3 = (Acceleration + newAcceleration);
        ConvertedVelocity += globals.TimeStep * 0.5f * vector3;
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
        newAcceleration *= globals.universalGravityConst;
        return newAcceleration;
    }

    private void OnMouseDown()
    {
        globals.mainCamera.GetComponent<CameraBehaviour>().PlanetClicked(transform);
        globals.selectedActor = this;
        m_VectorialField.Retarget(transform);
    }
    private void OnMouseEnter()
    {
        //Glow Growth
        transform.GetChild(0).transform.localScale *= 2f; // Glow is 0
    }
    private void OnMouseExit()
    {
        //Glow Shrink
        transform.GetChild(0).transform.localScale *= 0.5f; // Glow is 0
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!isAsteroid && (other.ClosestPoint(transform.position).sqrMagnitude / m_SizeFactor / m_SizeFactor) > 1f) // check if the collision happens because of rendering effects
        { return; }
        print("Collision");
        if (other.gameObject.TryGetComponent(out AstralObject otherAstral))
        {
            if (otherAstral.mass > mass)
            {
                float ratio = mass / otherAstral.mass;
                otherAstral.mass += mass;
                otherAstral.ConvertedMass += ConvertedMass;
                otherAstral.m_originalSize *= ratio;
                if (gameObject == globals.selectedActor)
                    globals.selectedActor = this;
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            //else we let the smaller object handles the collision and thus its destruction
        }
    }
}