using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Globals : MonoBehaviour
{
    //TODO: Maybe set the 3 next var in read only

    //astronomical mass unit to kilogram
    [SerializeField, Tooltip("astronomical mass unit to kilogram")]public float amu2kg = 5.972e24F;
    //astronomical distance unit to meters
    [SerializeField, Tooltip("astronomical distance unit to meters")]public float adu2m = 1.495978707e11F;
    //astronomical speed unit to meters per second
    [SerializeField, Tooltip("astronomical speed unit to meters per second")]public float asu2ms = 1.0e3F;
    [SerializeField] public float universalGravityConst = 6.6743e-11F;

    [SerializeField] public List<AstralObject> astralActors;
    [SerializeField] public AstralObject sun;

    [SerializeField, Min(0f)] private static float s_m_TimeStep = 100 * 24 * 3600f; // /!\ CARE: this is not a standard DeltaTime
    [SerializeField]public float timeStep { get; private set; } // /!\ CARE: this is not a standard DeltaTime

    // Start is called before the first frame update
    void Awake()
    {
        timeStep = s_m_TimeStep;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
