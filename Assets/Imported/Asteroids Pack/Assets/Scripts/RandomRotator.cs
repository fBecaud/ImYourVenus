using UnityEngine;

public class RandomRotator : MonoBehaviour
{
    [SerializeField] private float tumbleFactor;

    private void Start()
    {
        GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumbleFactor;
    }
}