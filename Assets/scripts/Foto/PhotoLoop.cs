using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PhotoLoop : MonoBehaviour

{
    public Material[] photoMaterials;
    public MeshRenderer MeshRenderer;
    public int waitSeconds;
    public float photoTime;
    public int photoCount;
    public bool photoChange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        photoTime = 0.0f;
        photoCount = 0;
        photoChange = false;

    }

    // Update is called once per frame
    private void Update()
    {
        //    if(photoTime == Time.deltaTime)
        //    {
        //PhotoCycle();
        //}
        
        if(photoTime < Time.time && photoChange == false)
        {
            photoChange = true;
            photocycle();
        }
        


    }


    
    private void photocycle()
    {
        Debug.Log("holi photo count");
        if (photoCount < photoMaterials.Length)
        {
            Debug.Log("este entro");
            MeshRenderer.material = photoMaterials[photoCount];
            photoCount++;
        }
        else
        {
            Debug.Log("este fue un else");
            photoCount = 0;
            MeshRenderer.material = photoMaterials[photoCount];
        }

        photoChange = false;
        photoTime = photoTime + 5.0f;
    }

}
