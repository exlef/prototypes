using UnityEngine;

public class SwapingTextures : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    [SerializeField] Material mat;
    float factor = 0;
    void Update()
    {
        if(Input.GetKey(KeyCode.Space))
        {
            factor += Time.deltaTime;
            factor = Mathf.Clamp01(factor);
            mat.SetFloat("_Factor", factor);
        }
    }
}
