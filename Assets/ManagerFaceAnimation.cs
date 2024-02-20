using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.Rendering.Universal;


[System.Serializable]
public class FaceAnimation
{
    public string name;
    public Sprite texture;
}

//emission const



public class ManagerFaceAnimation : MonoBehaviour
{

    private string EMISSION = "_Emission";
    private string COLOR = "_Color";
    private string BASE_MAP = "Base_Map";

    [Header("Animations settings")]

    [SerializeField] private Color _color = Color.white;
    //hdr color emission
    [ColorUsageAttribute(true, true)]
    [SerializeField] private Color _emissionColor = Color.white;    
    [SerializeField] private float _eyesAnimationSpeed = 1f;

    // [Range(0f, 10f)]
    // [SerializeField] private float _mouthAnimationSpeed = 1f; //polish

    [Header("Face Animations")]
    [SerializeField] private List<FaceAnimation> eyesAnimations = new List<FaceAnimation>();
    [SerializeField] private List<FaceAnimation> mouthAnimations = new List<FaceAnimation>();
 
 
    [Header("références")]
    [SerializeField] private DecalProjector eyesDecalProjector;
    [SerializeField] private DecalProjector mouthDecalProjector;
    [SerializeField] private Material eyeDecalMaterial;    
    [SerializeField] private Material mouthDecalMaterial; 
    [SerializeField] private GameObject face;

  
    public void Awake(){
        //selection of the first eyes and mouth
        changeEyes(eyesAnimations[0].name);
        changeMouth(mouthAnimations[0].name);
        changeSpriteColor(_color);        
    }

    public void changeSpriteColor(Color color)
    {
        eyeDecalMaterial.SetColor(COLOR, color);
        mouthDecalMaterial.SetColor(COLOR, color);

        eyeDecalMaterial.SetColor(EMISSION, _emissionColor);
        mouthDecalMaterial.SetColor(EMISSION, _emissionColor);
    }


   public void changeEyes(string eyeName)
    {
        foreach(FaceAnimation eyes in eyesAnimations)
        {
            if(eyes.name == eyeName)
            {
                eyeDecalMaterial.SetTexture(BASE_MAP, eyes.texture.texture);
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
                mouthDecalMaterial.SetTexture(BASE_MAP, mouth.texture.texture);
                mouthDecalProjector.material = mouthDecalMaterial;
            }
        }       
    }

    public void Update(){

        //for polish animation
        //eyesDecalProjector orientation must always be align with the face center
        eyesDecalProjector.transform.LookAt(face.transform.position);
        mouthDecalProjector.transform.LookAt(face.transform.position);

    }
}
