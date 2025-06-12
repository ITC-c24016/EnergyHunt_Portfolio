using UnityEngine;

public class GradualRotation : MonoBehaviour
{
    public Vector3 targetRotation;
    public float duration = 0.5f;

    private Vector3 initialRotation; 
    private float elapsedTime = 0f;

    void Start()
    {

    }

    void Update()
    {     
        elapsedTime += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        transform.eulerAngles = Vector3.Lerp(initialRotation, targetRotation, t);
        
    }
}
