
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour
{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 5f;

    float movementFactor; //0 for not moved, 1 for fully moved

    Vector3 startingpos;  //storing starting position for absolute movement

    // Start is called before the first frame update
    void Start()
    {
        startingpos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (period <= Mathf.Epsilon)
        {
            return;
        }

        float cycles = Time.time / period; //grows continually from 0
        const float tau = Mathf.PI * 2f; //2pi
        float rawSinWave = Mathf.Sin(cycles * tau);

        movementFactor = rawSinWave / 2f + 0.5f;  //scaling rawSinWave (-1 to 1) to (0-1)

        Vector3 offset = movementFactor * movementVector;
        transform.position = startingpos + offset;
    }
}
