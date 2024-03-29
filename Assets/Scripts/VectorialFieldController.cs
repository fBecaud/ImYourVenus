using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    [SerializeField] private GameObject m_ArrowPrefab;
    public bool bDisplayRotational
    {
        get => displayRotational;
        set
        {
            if (value)
            {
                InitRotVectors();
                m_RotArrowsParent.SetActive(true);
            }

            else
                HideRotArrows();
            Debug.Log("Hide Rotational");
            displayRotational = value;
        }
    }
    [FormerlySerializedAs("bDisplayRotational")]
    [field: SerializeField] private bool displayRotational = true;

    List<GameObject> m_RotArrows = new List<GameObject>();
    GameObject m_RotArrowsParent;
    [SerializeField] private GameObject m_RotArrowPrefab;

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
        float newScale = m_GridSize / _newTarget.lossyScale.magnitude;
        m_ArrowsParent.SetActive(true);
        m_ArrowsParent.transform.parent = _newTarget;
        m_ArrowsParent.transform.localPosition = m_GridPosition;
        m_ArrowsParent.transform.localScale = new Vector3(newScale, newScale, newScale);
        InitVectors();
        m_ArrowsParent.SetActive(displayField);

        m_RotArrowsParent.SetActive(true);
        m_RotArrowsParent.transform.parent = _newTarget;
        m_RotArrowsParent.transform.localPosition = m_GridPosition;
        m_RotArrowsParent.transform.localScale = new Vector3(newScale, newScale, newScale);
        InitRotVectors();
        m_RotArrowsParent.SetActive(displayRotational);

        m_LinesParent.SetActive(true);
        InitLineRenderers();
        m_LinesParent.SetActive(displayLines);
    }
    public void UnTarget()
    {
        HideArrows();
        HideRotArrows();
        HideLines();
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
        if (!m_RotArrowsParent)
        {
            m_RotArrowsParent = new GameObject("Field of Rotational Vectors");
            m_RotArrowsParent.transform.localScale = new Vector3(m_GridSize, m_GridSize, m_GridSize);
        }
        InitRotVectors();
        if (!m_LinesParent)
        {
            m_LinesParent = new GameObject("Field of Lines");
            m_LinesParent.transform.localScale = new Vector3(m_GridSize, m_GridSize, m_GridSize);
        }
        InitLineRenderers();
        if (!m_Globals.selectedActor)
        {
            HideArrows();
            HideRotArrows();
            HideLines();
        }
    }

    private void OnDestroy()
    {
        foreach (GameObject arrow in m_Arrows)
            Destroy(arrow);
        m_Arrows.Clear();
        Destroy(m_ArrowsParent);
        foreach (GameObject rotArrow in m_RotArrows)
            Destroy(rotArrow);
        m_RotArrows.Clear();
        Destroy(m_RotArrowsParent);
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
    private void HideRotArrows()
    {
        m_RotArrowsParent.SetActive(false);
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
        if (m_RotArrowsParent != null)
        {
            m_RotArrowsParent.SetActive(true);
            InitRotVectors();
            m_RotArrowsParent.SetActive(displayRotational);
        }
    }
    private void InitLineRenderers()
    {
        m_LineModel.positionCount = maxSuccesiveLines + 1;
        LineNb = (int)m_Density;
        foreach (LineRenderer line in m_Lines)
            line.positionCount = maxSuccesiveLines + 1;
        if (m_3D)
        {
            LineNbHorizontal = LineNbVertical = LineNb;
            LineNb = LineNbHorizontal * LineNbVertical;
        }
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
            if (m_Density > 10)
                m_Density = 10;
        }
        m_MaxSizeVector = 1f / m_Density;
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
                            GameObject go = Instantiate(m_ArrowPrefab, m_ArrowsParent.transform.position + posArrow, Quaternion.identity, m_ArrowsParent.transform);
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
                        GameObject go = Instantiate(m_ArrowPrefab, m_ArrowsParent.transform.position + posArrow, Quaternion.identity, m_ArrowsParent.transform);
                        m_Arrows.Add(go);
                    }
                    index++;
                }
        for (int i = index; i < m_Arrows.Count; i++)
            m_Arrows[i].SetActive(false);
        if (!displayField)
            HideArrows();
    }
    public void InitRotVectors()
    {
        if (m_3D)
            if (m_Density > 10)
                m_Density = 10;
        m_MaxSizeVector = 1f / m_Density;

        int index = 0;
        float gridStep = m_GridSize / (m_Density - 1);
        float gridHalfSize = m_GridSize * 0.5f;
        float _gridHalfSize = -gridHalfSize;

        if (m_3D)
            for (float i = _gridHalfSize; i <= gridHalfSize; i += gridStep)
                for (float j = _gridHalfSize; j <= gridHalfSize; j += gridStep)
                    for (float k = _gridHalfSize; k <= gridHalfSize; k += gridStep)
                    {
                        Vector3 posRotArrow = m_RotArrowsParent.transform.position - new Vector3(i, k, j);
                        if (m_RotArrows.Count() > index)
                        {
                            m_RotArrows[index].SetActive(true);
                            m_RotArrows[index].transform.position = posRotArrow;
                        }
                        else
                        {
                            GameObject go = Instantiate(m_RotArrowPrefab, m_RotArrowsParent.transform.position + posRotArrow, Quaternion.identity, m_RotArrowsParent.transform);
                            m_RotArrows.Add(go);
                        }
                        index++;
                    }
        else
            for (float i = _gridHalfSize; i <= gridHalfSize; i += gridStep)
                for (float j = _gridHalfSize; j <= gridHalfSize; j += gridStep)
                {
                    Vector3 posRotArrow = m_RotArrowsParent.transform.position - new Vector3(i, 0f, j);
                    if (m_RotArrows.Count() > index)
                    {
                        m_RotArrows[index].SetActive(true);
                        m_RotArrows[index].transform.position = posRotArrow;
                    }
                    else
                    {
                        GameObject go = Instantiate(m_RotArrowPrefab, m_RotArrowsParent.transform.position + posRotArrow, Quaternion.identity, m_RotArrowsParent.transform);
                        m_RotArrows.Add(go);
                    }
                    index++;
                }
        for (int i = index; i < m_RotArrows.Count; i++)
            m_RotArrows[i].SetActive(false);
        if (!displayRotational)
            HideRotArrows();
    }

    // Start is called before the first frame update
    void Start()
    {
        m_LogScale = !m_bIgnoreSun;
        InitVectors();
        InitRotVectors();
    }

    // Update is called once per frame
    void Update()
    {
        if (displayField || displayRotational)
        {
            float parentRescale = m_ArrowsParent.transform.lossyScale.magnitude * m_Globals.universalGravityConst;
            for (int i = 0; i < m_Arrows.Count(); i++)
            {
                Vector3 acceleration = ComputeAcceleration(m_Arrows[i].transform.position);
                float size;
                float aMag = acceleration.magnitude;
                if (displayField)
                {
                    if (m_LogScale)
                        size = Mathf.Clamp(Mathf.Log10(1f + aMag * parentRescale), 0f, m_MaxSizeVector);
                    else
                        size = Mathf.Clamp(aMag * parentRescale*1000f, 0f, m_MaxSizeVector);
                    if (size != 0f)
                    {
                        m_Arrows[i].transform.LookAt(m_Arrows[i].transform.position + acceleration / aMag);
                    }
                    else
                    { print("No Force"); };
                    m_Arrows[i].transform.localScale = new Vector3(size, size, size) * m_Zoom;
                }
                if (displayRotational)
                {
                    Vector3 rotational = ComputeRotational(m_Arrows[i].transform.position, acceleration);
                    float rMag = rotational.magnitude;

                    if (m_LogScale)
                        size = Mathf.Clamp(Mathf.Log10(1f + rMag * parentRescale), 0f, m_MaxSizeVector);
                    else
                        size = Mathf.Clamp(rMag * parentRescale*1000f, 0f, m_MaxSizeVector);
                    if (size != 0f)
                    {
                        m_RotArrows[i].transform.LookAt(m_RotArrows[i].transform.position + rotational / rMag);
                    }
                    else
                    { print("No Force"); };
                    m_RotArrows[i].transform.localScale = new Vector3(size, size, size) * m_Zoom;
                }
            }
        }
        if (displayLines && m_Globals.selectedActor)

        {
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

                        Vector3 startPoint = new (m_Globals.selectedActor.transform.position.x + (Mathf.Cos(angleH) * transform.localScale.x * Mathf.Sin(angleV)) * convertedStep, m_Globals.selectedActor.transform.position.y + Mathf.Cos(angleV) * transform.localScale.y * convertedStep, m_Globals.selectedActor.transform.position.z + (Mathf.Sin(angleH) * transform.localScale.z * Mathf.Sin(angleV)) * convertedStep);
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
                    Vector3 startPoint = new (m_Globals.selectedActor.transform.position.x + Mathf.Cos(angle) * transform.localScale.x * convertedStep, m_Globals.selectedActor.transform.position.y, m_Globals.selectedActor.transform.position.z + Mathf.Sin(angle) * transform.localScale.z * convertedStep);
                    DrawAFieldLine(startPoint, i);
                }
            }
        }
    }

    private void DrawAFieldLine(Vector3 _startPoint, int index)
    {
        Vector3 firstPoint = m_Globals.selectedActor.transform.position;
        Vector3 currentPoint = _startPoint;

        LineRenderer line = m_Lines[index];

        line.SetPosition(0, firstPoint);
        for (int i = 0; i < maxSuccesiveLines; i++)
        {
            Vector3 force = ComputeAcceleration(currentPoint);
            currentPoint -= force.normalized * step;
            float sqrMag = (currentPoint - transform.position).sqrMagnitude;
            if (sqrMag > maxDistLines * maxDistLines)
                return;

            line.SetPosition(i + 1, currentPoint);
        }
    }
    private Vector3 ComputeAcceleration(Vector3 _positionUnity)
    {
        Vector3 newAcceleration = Vector3.zero;
        float minSqDistance = 1000f;
        float distConverter = (m_Globals.adu2m * m_Globals.unity2astronomy);
        foreach (AstralObject influence in m_Globals.astralActors)
        {
            if (m_bIgnoreSun && influence == m_Globals.sun)
                continue;
            Vector3 toStar = influence.ConvertedPosition - _positionUnity * distConverter;
            double sqrMag = toStar.sqrMagnitude;
            if (sqrMag < minSqDistance)
                continue;
            newAcceleration += toStar * (float)((double)influence.ConvertedMass * System.Math.Pow(sqrMag, -1.5));
        }

        return newAcceleration;
    }

    private Vector3 ComputeRotational(Vector3 _positionUnity, Vector3 _acceleration)
    {
        float step = 1;

        Vector3 x_Increased = _positionUnity + new Vector3(step, 0f, 0f);
        Vector3 y_Increased = _positionUnity + new Vector3(0f, step, 0f);
        Vector3 z_Increased = _positionUnity + new Vector3(0f, 0f, step);

        Vector3 newAccelerationX = Vector3.zero;
        Vector3 newAccelerationY = Vector3.zero;
        Vector3 newAccelerationZ = Vector3.zero;
        float minSqDistance = 1000f;
        float distConverter = (m_Globals.adu2m * m_Globals.unity2astronomy);
        foreach (AstralObject influence in m_Globals.astralActors)
        {
            if (m_bIgnoreSun && influence == m_Globals.sun)
                continue;
            Vector3 toStarX = influence.ConvertedPosition - x_Increased * distConverter;
            Vector3 toStarY = influence.ConvertedPosition - y_Increased * distConverter;
            Vector3 toStarZ = influence.ConvertedPosition - z_Increased * distConverter;
            double sqrMagX = toStarX.sqrMagnitude;
            double sqrMagY = toStarY.sqrMagnitude;
            double sqrMagZ = toStarZ.sqrMagnitude;
            if (sqrMagX > minSqDistance)
                newAccelerationX = toStarX * (float)((double)influence.ConvertedMass * System.Math.Pow(sqrMagX, -1.5));
            if (sqrMagY > minSqDistance)
                newAccelerationY = toStarY * (float)((double)influence.ConvertedMass * System.Math.Pow(sqrMagY, -1.5));
            if (sqrMagZ > minSqDistance)
                newAccelerationZ = toStarZ * (float)((double)influence.ConvertedMass * System.Math.Pow(sqrMagZ, -1.5));
        }
        Vector3 XDerivative = (newAccelerationX - _acceleration) /*/step*/; //Here step is one
        Vector3 YDerivative = (newAccelerationY - _acceleration) /*/step*/; //Here step is one
        Vector3 ZDerivative = (newAccelerationZ - _acceleration) /*/step*/; //Here step is one

        Vector3 rotational = new Vector3(YDerivative.z - ZDerivative.y, ZDerivative.x - XDerivative.z, XDerivative.y - YDerivative.x);
        return rotational;
    }
}
