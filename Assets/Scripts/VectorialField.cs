using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorialField : MonoBehaviour
{
    [SerializeField] private Globals m_Globals;
    [SerializeField] bool m_3D = false;
    //Could be a Vec3 to be non-square
    [SerializeField, Min(0f)] private float m_Density = 100f;
    [SerializeField] private float m_MaxSizeVector = 1f;
    [SerializeField] private GameObject m_ArrrowPrefab;
    [SerializeField] private bool m_bIgnoreSun = true;


    List<GameObject> m_Arrows = new List<GameObject>();
    private void Awake()
    {
        m_Globals = FindObjectOfType<Globals>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_ArrrowPrefab.transform.localScale = new Vector3(1f / m_Density, 1f / m_Density, 1f / m_Density);
        for (float i = -transform.localScale.x * 0.5f; i <= transform.localScale.x * 0.5f; i += transform.localScale.x / m_Density)
            for (float j = -transform.localScale.z * 0.5f; j <= transform.localScale.z * 0.5f; j += transform.localScale.z / m_Density)
                for (float k = -transform.localScale.y * 0.5f; k <= transform.localScale.y * 0.5f; k += transform.localScale.y / m_Density)
                {
                    if (m_3D)
                    {
                        Vector3 posArrow = transform.position - new Vector3(i, j, transform.localScale.z * 0.5f + k);
                        GameObject go = Instantiate(m_ArrrowPrefab, posArrow, Quaternion.identity,transform);
                        m_Arrows.Add(go);
                    }
                    else
                    {
                        Vector3 posArrow = transform.position - new Vector3(i, 0f,j);
                        GameObject go = Instantiate(m_ArrrowPrefab, posArrow, Quaternion.identity,transform);
                        m_Arrows.Add(go);
                        break;
                    }

                }
    }

    // Update is called once per frame
    void Update()
    {
        foreach( GameObject go in m_Arrows)
        {
           Vector3 acceleration  = ComputeAcceleration(go.transform.position);
            go.transform.LookAt(go.transform.position + acceleration.normalized);
            float size = Mathf.Max(acceleration.magnitude, m_MaxSizeVector / m_Density);
            go.transform.localScale = new Vector3(size, size, size);
        }
    }
    private Vector3 ComputeAcceleration(Vector3 _localPosition)
    {
        Vector3 newAcceleration = Vector3.zero;
        foreach (AstralObject influence in m_Globals.astralActors)
        {
            float minDistance = 100 * (m_Globals.adu2m * m_Globals.unity2astronomy);
            if (m_bIgnoreSun && influence == m_Globals.sun)
                continue;
            Vector3 toStar = influence.convertedPosition - _localPosition * (m_Globals.adu2m * m_Globals.unity2astronomy);
            float sqrMag = toStar.sqrMagnitude;
            if (sqrMag > minDistance * minDistance)
                continue;
            newAcceleration += toStar * (influence.convertedMass * Mathf.Pow(sqrMag, -1.5f));
        }
        newAcceleration *= m_Globals.universalGravityConst;
        return newAcceleration;
    }
}
