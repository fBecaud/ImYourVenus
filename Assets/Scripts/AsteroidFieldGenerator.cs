using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class AsteroidFieldGenerator : MonoBehaviour
{
    [SerializeField] private GameObject m_AsteroidPrefab;
    [SerializeField] private Globals m_Globals;
    [SerializeField, Min(5)] private int m_AsteroidNb;

    public List<GameObject> Asteroids;
    public GameObject AsteroidsParent;

    private void Awake()
    {
        if (!m_Globals)
            m_Globals = FindAnyObjectByType<Globals>();
        if (!AsteroidsParent)
        {
            AsteroidsParent = new();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_AsteroidNb; i++)
            InitOneAsteroid();
    }

    void InitOneAsteroid()
    {
        float x = Random.Range(-200f, 200f);
        float y = Random.Range(-20f, 20f);
        float z = Random.Range(-200f, 200f);

        float size = Random.Range(0.001f, 10f);

        GameObject newAsteroid = Instantiate(m_AsteroidPrefab, new Vector3(x, y, z), Random.rotation, AsteroidsParent.transform);
        newAsteroid.transform.localScale = new Vector3(size, size, size);
        Asteroids.Add(newAsteroid);
        x = Random.Range(-2f, 2f);
        y = Random.Range(-0.2f, .2f);
        z = Random.Range(-2f, 2f);
        AstralObject astralInfos = newAsteroid.GetComponent<AstralObject>();
        astralInfos.velocity = new Vector3(x, y, z);
        astralInfos.mass = size;
        string name = "";
        char c = (char)('A' + Random.Range(0, 25));
        name += c;
        for (int i = 0; i < Random.Range(3, 7); i++)
        {
            c = (char)('a' + Random.Range(0, 25));
            name += c;
        }
        name += Random.Range(0, 499).ToString("000");
        astralInfos.name = name;
        astralInfos.isAsteroid = true;

        newAsteroid.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
