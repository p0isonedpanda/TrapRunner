using UnityEngine;

public class ObjectSmash : MonoBehaviour
{
    public GameObject shattered;

    void OnCollisionEnter (Collision col)
    {
        Instantiate(shattered, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
