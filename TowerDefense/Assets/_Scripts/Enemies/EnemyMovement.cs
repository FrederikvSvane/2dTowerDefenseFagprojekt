using UnityEngine;

public class BackAndForthMovement : MonoBehaviour
{
    public float speed = 5f; // Speed of the movement
    public float distance = 10f; // Maximum distance the object can move in one direction


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y >= distance)
        {
            speed = -speed;
        } else if (transform.position.y < -distance)
        {
            speed = -speed;
        }
        // Calculate the new position
        transform.Translate(Vector2.up*speed*Time.deltaTime);
    }
}
