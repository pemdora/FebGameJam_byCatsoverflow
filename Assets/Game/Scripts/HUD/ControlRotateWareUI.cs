using UnityEngine;
using UnityEngine.UI;

public class ControlRotateWareUI : MonoBehaviour
{
    Vector2 _refRes;
    Image _image;
    public void Start()
    {
        _image = GetComponent<Image>();
        _refRes = GetComponentInParent<CanvasScaler>().referenceResolution;
        var pickManager = FindObjectOfType<PickManager>();
        pickManager.OnGrabWare.AddListener(OnGrabCargo);
        pickManager.OnDropWare.AddListener(OnDropCargo);
        pickManager.OnPlaceWare.AddListener(OnDropCargo);
        _image.color = new Color(1, 1, 1, 0);
        FindObjectOfType<GameManager>().OnGameOverEvent.AddListener(OnGameOver);
    }
    public void Update()
    { 
        float refRatioAspect  = _refRes.x/_refRes.y;
        float screenRatioAspect = (float)Screen.width/(float)Screen.height;
        float refWidth = _refRes.x * screenRatioAspect/refRatioAspect;
        Vector2 pos = new(Input.mousePosition.x / (float)Screen.width * refWidth - refWidth/2, 
        Input.mousePosition.y / (float)Screen.height * _refRes.y - _refRes.y /2);
        pos.x += refWidth * 0.06f;
        GetComponent<RectTransform>().localPosition = pos;
    }
    public void OnGameOver()
    {
        _image.color = new Color(1, 1, 1, 0);
    }
    public void OnGrabCargo(WareEventData data)
    {
        _image.color = new Color(1, 1, 1, 1);
    }
    public void OnDropCargo(WareEventData data)
    {
        _image.color = new Color(1, 1, 1, 0);
    }
}