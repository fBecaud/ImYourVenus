using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldGenerator : MonoBehaviour
{
    [SerializeField] private List<GameObject> m_AsteroidPrefabs;
    [SerializeField] private GameObject m_TrailPrefab;

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

    private void Start()
    {
        for (int i = 0; i < m_AsteroidNb; i++)
            InitOneAsteroid();
    }

    private void InitOneAsteroid()
    {
        int index = Random.Range(0, m_AsteroidPrefabs.Count - 1);
        GameObject newAsteroid = Instantiate(m_AsteroidPrefabs[index], new Vector3(1000f, 1000f, 1000f), Random.rotation, AsteroidsParent.transform);
        newAsteroid.SetActive(false);

        AstralObject astralInfos = newAsteroid.GetComponent<AstralObject>();
        Asteroids.Add(newAsteroid);
        float x = Random.Range(-2f, 2f);
        float y = Random.Range(-0.2f, .2f);
        float z = Random.Range(-2f, 2f);

        astralInfos.velocity = new Vector3(x, y, z);
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

        x = Random.Range(-200f, 200f);
        y = Random.Range(-20f, 20f);
        z = Random.Range(-200f, 200f);

        float size = Random.Range(0.001f, 10f);

        newAsteroid.transform.position = new Vector3(x, y, z);
        newAsteroid.transform.localScale = new Vector3(size, size, size);
        astralInfos.mass = size;
        newAsteroid.SetActive(true);
        _ = Instantiate(m_TrailPrefab, newAsteroid.transform);
    }
}