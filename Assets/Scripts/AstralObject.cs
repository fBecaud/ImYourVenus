using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AstralObject : MonoBehaviour
{
    [SerializeField] private float m_ConvertedMass { get { return m_Mass; } set { m_Mass = value * 5.972e24F; } }
    private float m_Mass;
    [SerializeField] private Vector3 m_Position { get { return m_Position; } set { m_Position = value * 1.495978707e11F; } }
    [SerializeField] private Vector3 m_Velocity { get { return m_Velocity; } set { m_Velocity = value * 1.0e3F; } }
    public Vector3 m_Acceleration { get { return m_Acceleration; } private set { m_Acceleration = value; } }
    //TODO: Add the field calculus here
    [SerializeField] private List<AstralObject> m_AstralActors;

    //TODO: Make this global
    [SerializeField, Min(0f)] private static float s_m_TimeStep = 3600f; // /!\ CARE: this is not a standard DeltaTime

    [SerializeField, Min(0f)] private float m_Size = 1f;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(m_Size, m_Size, m_Size);
    }

    // Update is called once per frame
    void PhysicsUpdate()
    {
        m_Position += (m_Velocity + 0.5f * m_Acceleration * s_m_TimeStep) * s_m_TimeStep;
        Vector3 newAcceleration = ComputeField(m_AstralActors);
        m_Velocity += (m_Acceleration +  newAcceleration)*0.5f * s_m_TimeStep;

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
            Vector3 oneStarToAnother = influence.m_Position - m_Position;
            newAcceleration += influence.m_Mass * oneStarToAnother * Mathf.Pow(oneStarToAnother.sqrMagnitude, -1.5f);
        }
        newAcceleration *= universalGravityConst;
        return m_Acceleration;
    }
}
