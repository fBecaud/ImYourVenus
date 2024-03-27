using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class VectorialFieldController : MonoBehaviour
{
    [SerializeField] private Globals m_Globals;
    //Could be a Vec3 to be non-square

    [SerializeField] private bool m_bIgnoreSun = true;

    [Header("Grid Settings")]
    [SerializeField, Range(10f, 6000f)] private float m_GridSize = 100f;
    [SerializeField] private Vector3 m_GridPosition = Vector3.zero;


    [Header("Field of Vector Settings")]
    [SerializeField, Range(2f, 100f)] private uint m_Density = 20;
    [SerializeField] bool m_3D = false;
    [SerializeField] private bool m_LogScale;
    [SerializeField] private float m_Zoom = 10000f;
    private float m_MaxSizeVector = 1f;
    public bool bDisplayField
    {
        get => displayField;
        set
        {
            if (value)
            {
                InitVectors();
                m_ArrowsParent.SetActive(true);
            }

            else
                HideArrows();
            Debug.Log("Hide Field");
            displayField = value;
        }
    }
    [FormerlySerializedAs("bDisplayField")]
    [field: SerializeField] private bool displayField = true;

    List<GameObject> m_Arrows = new List<GameObject>();
    GameObject m_ArrowsParent;
    [SerializeField] private GameObject m_ArrrowPrefab;

    [Header("Field of Lines Settings")]
    [SerializeField] LineRenderer m_LineModel;
    List<LineRenderer> m_Lines = new List<LineRenderer>();
    GameObject m_LinesParent;

    [SerializeField]
    public bool bDisplayLines
    {
        get { return displayLines; }
        set
        {
            if (value)
            {
                ResetLineRenderer();
                m_LinesParent.SetActive(true);
            }
            else
            {
                HideLines();
            }
            Debug.Log("Hide Field");
            displayLines = value;
        }
    }
    [FormerlySerializedAs("bDisplayLines")]
    [SerializeField] private bool displayLines = true;
    public int LineNb = 8;
    public int LineNbHorizontal = 5;
    public int LineNbVertical = 3;

    public int maxSuccesiveLines = 100;
    public float maxDistLines = 100000f;
    public float step = 10f;

    public void Retarget(Transform _newTarget)
    {
        m_ArrowsParent.SetActive(true);
        m_ArrowsParent.transform.parent = _newTarget;
        m_ArrowsParent.transform.localPosition = m_GridPosition;
        m_ArrowsParent.transform.localScale = new Vector3(m_GridSize, m_GridSize, m_GridSize);
        InitVectors();
        if (!displayField)
            m_ArrowsParent.SetActive(false);

        m_LinesParent.SetActive(true);
        InitLineRenderers();
        if (!displayLines)
            m_LinesParent.SetActive(false);
    }
    private void Awake()
    {
        m_Globals = FindObjectOfType<Globals>();
        if (!m_ArrowsParent)
        {
            m_ArrowsParent = new GameObject("Field of Vectors");
            m_ArrowsParent.transform.localScale = new Vector3(m_GridSize, m_GridSize, m_GridSize);
        }
        InitVectors();
        if (!m_LinesParent)
        {
            m_LinesParent = new GameObject("Field of Lines");
            m_LinesParent.transform.localScale = new Vector3(m_GridSize, m_GridSize, m_GridSize);
        }
        InitLineRenderers();
        if (!m_Globals.selectedActor)
        {
            HideArrows();
            HideLines();
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject arrow in m_Arrows)
            Destroy(arrow);
        m_Arrows.Clear();
        Destroy(m_ArrowsParent);
        foreach (LineRenderer line in m_Lines)
            Destroy(line);
        m_Lines.Clear();
        Destroy(m_LinesParent);
    }
    private void ResetLineRenderer()
    {
        InitLineRenderers();
    }

    private void HideLines()
    {
        m_LinesParent.SetActive(false);
    }
    private void HideArrows()
    {
        m_ArrowsParent.SetActive(false);
    }
    private void OnValidate()
    {
        if (m_LinesParent != null)
        {
            m_LinesParent.SetActive(true);
            ResetLineRenderer();
            m_LinesParent.SetActive(displayLines);
        }
        if (m_ArrowsParent != null)
        {
            m_ArrowsParent.SetActive(true);
            InitVectors();
            m_ArrowsParent.SetActive(displayField);
        }
    }
    private void InitLineRenderers()
    {
        m_LineModel.positionCount = maxSuccesiveLines + 1;
        foreach (LineRenderer line in m_Lines)
            line.positionCount = maxSuccesiveLines + 1;
        if (m_3D)
            LineNb = LineNbHorizontal * LineNbVertical;
        for (int i = m_Lines.Count(); i < LineNb; i++)
            InitLineRenderer();
        for (int i = 0; i < m_Lines.Count(); i++)
            m_Lines[i].gameObject.SetActive(i < LineNb);
    }

    private void InitLineRenderer()
    {
        LineRenderer newLine = Instantiate(m_LineModel);
        newLine.startColor = Color.red;
        newLine.endColor = Color.green;
        newLine.enabled = true;
        newLine.receiveShadows = false;
        newLine.shadowCastingMode = ShadowCastingMode.Off;
        newLine.transform.SetParent(m_LinesParent.transform);
        m_Lines.Add(newLine);
        if (!displayLines)
            HideLines();
    }

    public void InitVectors()
    {
        if (m_3D)
        {
            if (m_Density > 20)
                m_Density = 20;
            m_MaxSizeVector = 1f / m_Density;
        }
        else
        {
            m_MaxSizeVector = 1f / m_Density;
        }
        int index = 0;
        float gridStep = m_GridSize / (m_Density - 1);
        float gridHalfSize = m_GridSize * 0.5f;
        float _gridHalfSize = -gridHalfSize;

        if (m_3D)
            for (float i = _gridHalfSize; i <= gridHalfSize; i += gridStep)
                for (float j = _gridHalfSize; j <= gridHalfSize; j += gridStep)
                    for (float k = _gridHalfSize; k <= gridHalfSize; k += gridStep)
                    {
                        Vector3 posArrow = m_ArrowsParent.transform.position - new Vector3(i, k, j);
                        if (m_Arrows.Count() > index)
                        {
                            m_Arrows[index].SetActive(true);
                            m_Arrows[index].transform.position = posArrow;
                        }
                        else
                        {
                            GameObject go = Instantiate(m_ArrrowPrefab, m_ArrowsParent.transform.position + posArrow, Quaternion.identity, m_ArrowsParent.transform);
                            m_Arrows.Add(go);
                        }
                        index++;
                    }
        else
            for (float i = _gridHalfSize; i <= gridHalfSize; i += gridStep)
                for (float j = _gridHalfSize; j <= gridHalfSize; j += gridStep)
                {
                    Vector3 posArrow = m_ArrowsParent.transform.position - new Vector3(i, 0f, j);
                    if (m_Arrows.Count() > index)
                    {
                        m_Arrows[index].SetActive(true);
                        m_Arrows[index].transform.position = posArrow;
                    }
                    else
                    {
                        GameObject go = Instantiate(m_ArrrowPrefab, m_ArrowsParent.transform.position + posArrow, Quaternion.identity, m_ArrowsParent.transform);
                        m_Arrows.Add(go);
                    }
                    index++;
                }
        for (int i = index; i < m_Arrows.Count; i++)
            m_Arrows[i].SetActive(false);
        if (!displayField)
            HideArrows();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_LogScale = !m_bIgnoreSun;
        InitVectors();
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
                    size = Mathf.Clamp(Mathf.Log10(1f + acceleration.magnitude * go.transform.parent.lossyScale.magnitude * m_Globals.universalGravityConst), 0f, m_MaxSizeVector);
                else
                    size = Mathf.Clamp(acceleration.magnitude / 10000f, 0f, m_MaxSizeVector);
                if (size != 0f)
                {
                    go.transform.LookAt(go.transform.position + acceleration.normalized);
                }
                else
                { print("No Force"); };

                go.transform.localScale = new Vector3(size, size, size) * m_Zoom;
            }
        }
        if (displayLines && m_Globals.selectedActor)

        {
            int index = 0;
            float convertedStep = step * m_Globals.unity2astronomy;

            if (m_3D)
            {
                float angleHStep = 360f / LineNbHorizontal * Mathf.Deg2Rad;
                float angleHOffset = (180f / LineNbHorizontal) * Mathf.Deg2Rad;
                float angleVStep = 180f / LineNbVertical * Mathf.Deg2Rad;
                float angleVOffset = (90f / LineNbVertical) * Mathf.Deg2Rad;


                for (int i = 0; i < LineNbHorizontal; i++)
                {
                    float angleH = i * angleHStep + angleHOffset;
                    for (int j = 0; j < LineNbVertical; j++)
                    {
                        float angleV = j * angleVStep + angleVOffset;

                        Vector3 startPoint = new Vector3(m_Globals.selectedActor.transform.position.x + (Mathf.Cos(angleH) * transform.localScale.x * Mathf.Sin(angleV)) * convertedStep, m_Globals.selectedActor.transform.position.y + Mathf.Cos(angleV) * transform.localScale.y * convertedStep, m_Globals.selectedActor.transform.position.z + (Mathf.Sin(angleH) * transform.localScale.z * Mathf.Sin(angleV)) * convertedStep);
                        DrawAFieldLine(startPoint, i * LineNbVertical + j);
                    }
                }
            }
            else
            {
                float angleStep = 360f / LineNb * Mathf.Deg2Rad;
                for (int i = 0; i < m_Lines.Count(); i++)
                {
                    float angle = i * angleStep;
                    Vector3 startPoint = new Vector3(m_Globals.selectedActor.transform.position.x + Mathf.Cos(angle) * transform.localScale.x * convertedStep, m_Globals.selectedActor.transform.position.y, m_Globals.selectedActor.transform.position.z + Mathf.Sin(angle) * transform.localScale.z * convertedStep);
                    DrawAFieldLine(startPoint, i);
                }
            }
        }
    }

    private void DrawAFieldLine(Vector3 _startPoint, int index)
    {
        Vector3 previousPoint = m_Globals.selectedActor.transform.position;
        Vector3 currentPoint = _startPoint;

        LineRenderer line = m_Lines[index];

        line.SetPosition(0, previousPoint);
        for (int i = 0; i < maxSuccesiveLines; i++)
        {
            Vector3 force = ComputeAcceleration(currentPoint);
            currentPoint -= force.normalized * step;
            float sqrMag = (currentPoint - transform.position).sqrMagnitude;
            if (sqrMag > maxDistLines * maxDistLines)
                return;
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
