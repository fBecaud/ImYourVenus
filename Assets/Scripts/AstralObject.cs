using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AstralObject : MonoBehaviour
{
    //TODO: Make this global
//Globals to put in another script
    //astronomical mass unit to kilogram
    float amu2kg = 5.972e24F;
    //astronomical distance unit to meters
    float adu2m = 1.495978707e11F;
    //astronomical speed unit to meters
    float asu2ms = 1.0e3F;

    //TODO: Add the field calculus here
    [SerializeField] private List<AstralObject> m_AstralActors;

    [SerializeField, Min(0f)] private static float s_m_TimeStep = 100*24*3600f; // /!\ CARE: this is not a standard DeltaTime
//

    [SerializeField, Tooltip("in astronomical mass unit")] private float m_Mass = 1f;
    //In kilograms
    public float convertedMass {get; private set;}
    //[SerializeField] private Vector3 m_Position= new Vector3( 1f, 0f, 0f );
    //In meters
    public Vector3 convertedPosition { get; private set; }

    [SerializeField] private Vector3 m_Velocity = Vector3.zero;
    public Vector3 convertedVelocity { get; private set; }

    public Vector3 acceleration { get; private set; }
    //seems useless
    //[SerializeField] private float m_Size {get { return m_Size; } set { transform.localScale = new Vector3(value, value, value); } }

    private void ConvertUnits()
    {
        convertedMass = m_Mass * amu2kg;
        convertedPosition = transform.position * adu2m;
        convertedVelocity = m_Velocity * asu2ms;
    }

    // Start is called before the first frame update
    void Start()
    {
        ConvertUnits();
        //transform.localScale = new Vector3(m_Size, m_Size, m_Size);
    }
    private void OnValidate()
    {
        ConvertUnits();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        convertedPosition += (convertedVelocity + 0.5f * acceleration * s_m_TimeStep * Time.fixedDeltaTime) * s_m_TimeStep * Time.fixedDeltaTime;
        //Reposition the planet
        transform.position = convertedPosition /adu2m;
        Vector3 newAcceleration = ComputeField(m_AstralActors);
        convertedVelocity += (acceleration +  newAcceleration)*0.5f * s_m_TimeStep * Time.fixedDeltaTime;
        acceleration = newAcceleration;
    }
    Vector3 ComputeField(List<AstralObject> _everyActors)
    {
        const float universalGravityConst = 6.6743e-11F;
        Vector3 newAcceleration = Vector3.zero;
        //TODO: TEST this
        foreach (AstralObject influence in _everyActors)
        {
            if (influence == this)
                continue;
            Vector3 oneStarToAnother = influence.convertedPosition - convertedPosition;
            newAcceleration += oneStarToAnother * (influence.convertedMass * Mathf.Pow(oneStarToAnother.sqrMagnitude, -1.5f));
        }
        newAcceleration = newAcceleration* universalGravityConst;
        return newAcceleration;
    }
}
