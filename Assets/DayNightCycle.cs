using UnityEngine;

public class DayNightCycle : MonoBehaviour
{

    [SerializeField] private float dayDuration = 120f;
    private float currentTime = 0f;
    private float daySpeed = 1f;

    [SerializeField] private Transform sun; // 50 : 250

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime * daySpeed;

        sun.transform.localEulerAngles = new Vector3(Mathf.Lerp(50, 250, currentTime/dayDuration), sun.transform.localEulerAngles.y, sun.transform.localEulerAngles.z);

        if(Input.GetKeyDown(KeyCode.K))
        {
            daySpeed *= 5f;
        }

        if (currentTime >= dayDuration)
        {
            DayOver();
        }
    }

    public void DayOver()
    {

    }
}
