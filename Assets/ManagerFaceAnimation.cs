using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class FaceAnimation
{
    public string name;
    public Sprite texture;
}

public class ManagerFaceAnimation : MonoBehaviour
{



    [Header("Face Animations")]
    [SerializeField] private List<FaceAnimation> eyesAnimations = new List<FaceAnimation>();
    [SerializeField] private List<FaceAnimation> mouthAnimations = new List<FaceAnimation>();
 
 
    [Header("références")]
    [SerializeField] private DecalProjector eyesDecalProjector;
    [SerializeField] private DecalProjector mouthDecalProjector;
    //decal material
    [SerializeField] private Material eyeDecalMaterial;    
    [SerializeField] private Material mouthDecalMaterial; 
    [SerializeField] private GameObject face;

  
    public void Start(){

        //selection of the first eyes and mouth
        changeEyes(eyesAnimations[0].name);
        changeMouth(mouthAnimations[0].name);
        
    }

    public void changeSpriteColor(Color color)
    {
        eyeDecalMaterial.SetColor("_BaseColor", color);
        mouthDecalMaterial.SetColor("_BaseColor", color);
    }


   public void changeEyes(string eyeName)
    {
        foreach(FaceAnimation eye in eyesAnimations)
        {
            if(eye.name == eyeName)
            {
                eyeDecalMaterial.SetTexture("_BaseMap", eye.texture.texture);
                eyesDecalProjector.material = eyeDecalMaterial;
            }
        }       
    }

    public void changeMouth(string mouthName)
    {
        foreach(FaceAnimation mouth in mouthAnimations)
        {
            if(mouth.name == mouthName)
            {
                mouthDecalMaterial.SetTexture("_BaseMap", mouth.texture.texture);
                mouthDecalProjector.material = mouthDecalMaterial;
            }
        }       
    }

    public void Update(){

        //eyesDecalProjector orientation must always be align with the face center
        eyesDecalProjector.transform.LookAt(face.transform.position);
        mouthDecalProjector.transform.LookAt(face.transform.position);

    }
}
