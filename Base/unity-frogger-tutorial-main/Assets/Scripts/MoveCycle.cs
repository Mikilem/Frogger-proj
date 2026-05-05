using UnityEngine;

public class MoveCycle : MonoBehaviour
{
    public Vector2 direction = Vector2.right;
    public float speed = 1f;
    public int size = 1;

    private Vector3 leftEdge;
    private Vector3 rightEdge;
    private Vector3 topEdge;
    private Vector3 bottomEdge;

    private void Start()
    {
        leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);
        bottomEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        topEdge = Camera.main.ViewportToWorldPoint(Vector3.up);
    }

    private void Update()
    {
        // Check if the object is past the right edge of the screen
        if (direction.x > 0 && (transform.position.x - size) > rightEdge.x) {
            transform.position = new Vector3(leftEdge.x - size, transform.position.y, transform.position.z);
        }
        // Check if the object is past the left edge of the screen
        else if (direction.x < 0 && (transform.position.x + size) < leftEdge.x) {
            transform.position = new Vector3(rightEdge.x + size, transform.position.y, transform.position.z);
        }
        //top
        else if (direction.y > 0 && (transform.position.y - size) > topEdge.y)
        {
            transform.position = new Vector3(transform.position.x, bottomEdge.y - size, transform.position.z);
        }
        //bottom
        else if (direction.y < 0 && (transform.position.y + size) < bottomEdge.y)
        {
            transform.position = new Vector3(transform.position.x, topEdge.y, transform.position.z);
        }
        // Move the object
        else {
            transform.Translate(speed * Time.deltaTime * direction);
        }
    }

}
