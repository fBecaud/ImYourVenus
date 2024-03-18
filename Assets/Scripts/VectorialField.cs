using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorialField : MonoBehaviour
{
    [SerializeField] private Globals m_Globals;
    [SerializeField] bool m_3D = false;
    //Could be a Vec3 to be non-square
    [SerializeField, Min(0f)] private Vector3 m_Density = new Vector3(100f, 100f, 100f);
    [SerializeField] private float m_MaxSizeVector = 1f;
    [SerializeField] private float m_Zoom = 10000f;
    [SerializeField] private bool m_LogScale;

    [SerializeField] private GameObject m_ArrrowPrefab;
    [SerializeField] private bool m_bIgnoreSun = true;


    List<GameObject> m_Arrows = new List<GameObject>();
    public bool displayField;
    public bool displayLines;

    private void Awake()
    {
        m_Globals = FindObjectOfType<Globals>();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_LogScale = !m_bIgnoreSun;
        if (m_3D)
            m_MaxSizeVector = transform.localScale.magnitude / m_Density.magnitude;
        else
        {
            m_MaxSizeVector = ((Vector2)transform.localScale).magnitude / ((Vector2)m_Density).magnitude;
        }
        for (float i = -transform.localScale.x * 0.5f; i <= transform.localScale.x * 0.5f; i += transform.localScale.x / m_Density.x)
            for (float j = -transform.localScale.z * 0.5f; j <= transform.localScale.z * 0.5f; j += transform.localScale.z / m_Density.y)
                for (float k = -transform.localScale.y * 0.5f; k <= transform.localScale.y * 0.5f; k += transform.localScale.y / m_Density.z)
                {
                    if (m_3D)
                    {
                        Vector3 posArrow = transform.position - new Vector3(i, k, j);
                        GameObject go = Instantiate(m_ArrrowPrefab, posArrow, Quaternion.identity, transform);
                        m_Arrows.Add(go);
                    }
                    else
                    {
                        Vector3 posArrow = transform.position - new Vector3(i, 0f, j);
                        GameObject go = Instantiate(m_ArrrowPrefab, posArrow, Quaternion.identity, transform);
                        m_Arrows.Add(go);
                        break;
                    }

                }
    }

    // Update is called once per frame
    void Update()
    {
        if (displayField)
        {
            foreach (GameObject go in m_Arrows)
            {
                Vector3 acceleration = ComputeAcceleration(go.transform.position);
                float size;
                if (m_LogScale)
                    size = Mathf.Clamp(Mathf.Log10(1f + acceleration.magnitude * m_Globals.universalGravityConst * m_Zoom), 0f, m_MaxSizeVector);
                else
                    size = Mathf.Clamp(acceleration.magnitude * m_Zoom / 10000f, 0f, m_MaxSizeVector);
                if (size != 0f)
                {
                    go.transform.LookAt(go.transform.position + acceleration.normalized);
                }
                else
                { print("No Force"); };

                go.transform.localScale = new Vector3(size, size, size);
            }
        }
        if (displayLines && m_Globals.selectedActor)
        {
            int LineNb = 8;
            for (int i = 0; i < 360; i += 360 / LineNb)
            {
                float angle = i * Mathf.Deg2Rad;
                Vector3 startPoint = new Vector3(m_Globals.selectedActor.ConvertedPosition.x +Mathf.Cos(angle), m_Globals.selectedActor.ConvertedPosition.y + Mathf.Sin(angle), m_Globals.selectedActor.ConvertedPosition.z);
                DrawAFieldLine(startPoint);
            }
        }
    }
    private void DrawAFieldLine(Vector3 _startPoint)
    {
        Vector3 previousPoint = _startPoint;
        Vector3 currentPoint = previousPoint;
        const int MAX_LINES = 100;
        const float MAX_DIST_LINES = 100;
        for (int i = 0;i < MAX_LINES; i++)
        {
            currentPoint += ComputeAcceleration(currentPoint);
            if ((currentPoint / (m_Globals.adu2m * m_Globals.unity2astronomy) - transform.position).sqrMagnitude > MAX_DIST_LINES)
                return;
            Debug.DrawLine(previousPoint / (m_Globals.adu2m * m_Globals.unity2astronomy), currentPoint /(m_Globals.adu2m * m_Globals.unity2astronomy),Color.red);
            previousPoint = currentPoint;
        }
    }

    private Vector3 ComputeAcceleration(Vector3 _positionUnity)
    {
        Vector3 newAcceleration = Vector3.zero;
        float minSqDistance = 1000f;
        foreach (AstralObject influence in m_Globals.astralActors)
        {
            //float minDistance = 100 * (m_Globals.adu2m * m_Globals.unity2astronomy);
            if (m_bIgnoreSun && influence == m_Globals.sun)
                continue;
            Vector3 toStar = influence.ConvertedPosition - _positionUnity * (m_Globals.adu2m * m_Globals.unity2astronomy);
            double sqrMag = toStar.sqrMagnitude;
            if (sqrMag < minSqDistance)
                continue;
            newAcceleration += toStar * (float)((double)influence.ConvertedMass * System.Math.Pow(sqrMag, -1.5));
        }

        return newAcceleration;
    }
}
