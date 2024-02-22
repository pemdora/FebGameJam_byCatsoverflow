using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] Texture2D _point;
    [SerializeField] Texture2D _hover;
    [SerializeField] Texture2D _grab;
    Texture2D _pointResized;
    Texture2D _hoverResized;
    Texture2D _grabResized;

    Vector2 _offsetBase = new(40, 21);
    Vector2 _offsetResized;

    int lastScreenheight = 0;
    public bool hovering = false;
    public bool grabbing = false;
    public void Start()
    {
        var pickManager = FindObjectOfType<PickManager>();
        pickManager.OnDropWare.AddListener(OnDropCargo);
        pickManager.OnPlaceWare.AddListener(OnDropCargo);
        pickManager.OnGrabWare.AddListener(OnGrabCargo);
        pickManager.OnHoverWare.AddListener(OnHoverCargo);
        pickManager.OnUnHoverWare.AddListener(OnUnHoverCargo);
        OnScreenSize();
    }
    public void FixedUpdate()
    {
        if (lastScreenheight != Screen.height)
        {
            OnScreenSize();
            lastScreenheight = Screen.height;
        }
    }

    public void OnScreenSize()
    {
        float factor = (float)Screen.height / 1080f;

        _pointResized = Resize(_point, Mathf.RoundToInt(_point.width * factor), Mathf.RoundToInt(_point.height * factor));
        _hoverResized = Resize(_hover, Mathf.RoundToInt(_hover.width * factor), Mathf.RoundToInt(_hover.height * factor));
        _grabResized = Resize(_grab, Mathf.RoundToInt(_grab.width * factor), Mathf.RoundToInt(_grab.height * factor));
        _offsetResized = _offsetBase * factor;

        Cursor.SetCursor(_pointResized, _offsetResized, CursorMode.ForceSoftware);
    }

    //https://stackoverflow.com/questions/56949217/how-to-resize-a-texture2d-using-height-and-width
    Texture2D Resize(Texture2D texture2D, int targetX, int targetY)
    {
        RenderTexture rt = new RenderTexture(targetX, targetY, 24);
        RenderTexture.active = rt;
        Graphics.Blit(texture2D, rt);
        Texture2D result = new Texture2D(targetX, targetY);
        result.ReadPixels(new Rect(0, 0, targetX, targetY), 0, 0);
        result.Apply();
        return result;
    }


    public void OnHoverCargo(WareEventData data)
    {
        hovering = true;
        if (!grabbing)
        {
            Cursor.SetCursor(_hoverResized, _offsetResized, CursorMode.ForceSoftware);
        }
    }
    public void OnUnHoverCargo(WareEventData data)
    {
        hovering = false;
        if (!grabbing)
        {
            Cursor.SetCursor(_pointResized, _offsetResized, CursorMode.ForceSoftware);
        }
    }
    public void OnGrabCargo(WareEventData data)
    {
        grabbing = true;
        Cursor.SetCursor(_grabResized, _offsetResized, CursorMode.ForceSoftware);
    }
    public void OnDropCargo(WareEventData data)
    {
        if (hovering)
        {
            Cursor.SetCursor(_hoverResized, _offsetResized, CursorMode.ForceSoftware);
        }
        else
        {
            Cursor.SetCursor(_pointResized, _offsetResized, CursorMode.ForceSoftware);
        }
    }
}