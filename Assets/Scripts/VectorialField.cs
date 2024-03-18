using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;

public class VectorialField : MonoBehaviour
{
    [SerializeField] private Globals m_Globals;
    //Could be a Vec3 to be non-square

    [SerializeField] private bool m_bIgnoreSun = true;

    [Header("Field of Vector Settings")]
    [SerializeField, Min(0f)] private Vector3 m_Density = new Vector3(100f, 100f, 100f);
    [SerializeField] bool m_3D = false;
    [SerializeField] private bool m_LogScale;
    [SerializeField] private float m_Zoom = 10000f;
    [SerializeField] private float m_MaxSizeVector = 1f;
    public bool displayField;
    List<GameObject> m_Arrows = new List<GameObject>();
    [SerializeField] private GameObject m_ArrrowPrefab;

    [Header("Field of Lines Settings")]
    [SerializeField] LineRenderer m_LineModel;
    List<LineRenderer> m_LineList = new List<LineRenderer>();
    GameObject m_LineListParent;
    public bool displayLines;
    public int LineNb = 8;
    public int LineNbHorizontal = 5;
    public int LineNbVertical = 3;

    public int maxSuccesiveLines = 100;
    public float maxDistLines = 100000f;
    public float step = 10f;


    private void Awake()
    {
        m_Globals = FindObjectOfType<Globals>();
        if (!m_LineListParent)
            m_LineListParent = new GameObject("Field of Lines");
        InitLineRenderers();
    }

    private void OnDestroy()
    {
        foreach (LineRenderer line in m_LineList)
            Destroy(line);
        m_LineList.Clear();
        Destroy(m_LineListParent);
    }
    private void ResetLineRenderer()
    {
        InitLineRenderers();
    }

    //private void OnValidate()
    //{
    //    if (!m_LineListParent)
    //        m_LineListParent = new GameObject("Field of Lines");
    //    ResetLineRenderer();
    //}
    private void InitLineRenderers()
    {
        m_LineModel.positionCount = maxSuccesiveLines + 1;
        foreach (LineRenderer line in m_LineList)
            Destroy(line);
        m_LineList.Clear();
        for (int i = 0; i < LineNb; i++)
            InitLineRenderer();
    }

    private void InitLineRenderer()
    {
        LineRenderer newLine = Instantiate(m_LineModel);
        newLine.startColor = Color.red;
        newLine.endColor = Color.green;
        newLine.enabled = true;
        newLine.receiveShadows = false;
        newLine.shadowCastingMode = ShadowCastingMode.Off;
        newLine.transform.SetParent(m_LineListParent.transform);
        m_LineList.Add(newLine);
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
            if (m_3D)
                for (int i = 0; i < LineNbHorizontal; i++)
                    for (int j = 0; j < LineNbVertical; j++)
                    {
                        float angleH = i* 360/LineNbHorizontal * Mathf.Deg2Rad;
                        float angleV = (j* 180/(LineNbVertical) - 90*(LineNbVertical-1 )/ LineNbVertical) * Mathf.Deg2Rad;
                        //float angleV = (j * (90*(LineNbVertical-1) / LineNbVertical)) * Mathf.Deg2Rad;

                        Vector3 startPoint = new Vector3(m_Globals.selectedActor.transform.position.x + (Mathf.Cos(angleH) * transform.localScale.x * Mathf.Sin(angleV)) * step * m_Globals.unity2astronomy, m_Globals.selectedActor.transform.position.y + Mathf.Sin(angleV) * transform.localScale.y * step * m_Globals.unity2astronomy, m_Globals.selectedActor.transform.position.z + (Mathf.Sin(angleH) * transform.localScale.z * Mathf.Sin(angleV)) * step * m_Globals.unity2astronomy);
                        DrawAFieldLine(startPoint, i * LineNbVertical + j);
                    }
            else
            {
                for (int i = 0; i < 360; i += 360 / LineNb)
                {
                    float angle = i * Mathf.Deg2Rad;
                    Vector3 startPoint = new Vector3(m_Globals.selectedActor.transform.position.x + Mathf.Cos(angle) * transform.localScale.x * step * m_Globals.unity2astronomy, m_Globals.selectedActor.transform.position.y, m_Globals.selectedActor.transform.position.z + Mathf.Sin(angle) * transform.localScale.z * step * m_Globals.unity2astronomy);
                    DrawAFieldLine(startPoint, i * LineNb / 360);
                }
            }
        }
    }

    private void DrawAFieldLine(Vector3 _startPoint, int index)
    {
        Vector3 previousPoint = m_Globals.selectedActor.transform.position;
        Vector3 currentPoint = _startPoint;

        LineRenderer line = m_LineList[index];

        line.SetPosition(0, previousPoint);
        for (int i = 0; i < maxSuccesiveLines; i++)
        {
            Vector3 force = ComputeAcceleration(currentPoint);
            currentPoint -= force.normalized * step;
            //if ((currentPoint - transform.position).sqrMagnitude > maxDistLines * maxDistLines)
            //return;
            Color newColor2 = new Color(1f, ((float)(i + 1)) / maxSuccesiveLines, 0f, 1f);

            line.SetPosition(i + 1, currentPoint);


            //Color newColor = new Color(1f, ((float)(i)) / maxSuccesiveLines, 0f, 1f);
            //Debug.DrawLine(previousPoint, currentPoint, newColor);
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
