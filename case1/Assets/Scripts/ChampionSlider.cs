using UnityEngine;

public class ChampionSlider : MonoBehaviour
{
    [SerializeField] Transform slider;
    Transform[] sections;
    Camera cam;
    Transform cannonTr;
    Vector3 initialOffsetToCannon;
    
    public void Init(Camera _cam, Transform _cannonTr)
    {
        cam = _cam;
        cannonTr = _cannonTr;
        initialOffsetToCannon = transform.position - _cannonTr.position;
        sections = new Transform[slider.childCount];
        for (int i = 0; i < slider.childCount; i++)
        {
            sections[i] = slider.GetChild(i);
        }
        
        HideAllSections();
    }

    public void Show()
    {
        slider.gameObject.SetActive(true);
    }

    public void Hide()
    {
        slider.gameObject.SetActive(false);
    }

    public void UpdateSlider(float value)
    {
        value = Mathf.Clamp01(value);
        int count = Mathf.RoundToInt(Mathf.Lerp(0, sections.Length, value));
        count = Mathf.Clamp(count, 0, sections.Length);
        HideAllSections();
        for (int i = 0; i < count; i++)
        {
            sections[i].gameObject.SetActive(true);
        }
    }

    void HideAllSections()
    {
        foreach (var s in sections)
        {
            s.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, cannonTr.position + initialOffsetToCannon, Time.deltaTime * 25);
        Vector3 dir = (cam.transform.position - transform.position).normalized;
        dir.y = 0;
        transform.forward = Vector3.Slerp(transform.forward, -dir, Time.deltaTime * 5);// -dir;
    }
}
