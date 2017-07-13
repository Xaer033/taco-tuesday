using UnityEngine;
using UnityEngine.Assertions;
using GhostGen;

[CreateAssetMenu]
public class GuiManager : ScriptableObject, IPostInit
{
    public Canvas       mainCanvas      { get; private set; }
    public Camera       guiCamera       { get; private set; }

    public ScreenFader  screenFader     { get; private set; }
    public ViewFactory  viewFactory     { get; private set; }

    private GameObject _guiObject;
    
    public void PostInit()
    {
        _guiObject = _getOrCreateGuiObject();
        mainCanvas = _guiObject.GetComponentInChildren<Canvas>();
        guiCamera = _guiObject.GetComponentInChildren<Camera>();

        viewFactory = new ViewFactory(mainCanvas);
        screenFader = _createScreenFader(mainCanvas);

        Assert.IsNotNull(mainCanvas);
    }

    public void Step(float deltaTime)
    {
        viewFactory.Step();
    }

    private ScreenFader _createScreenFader(Canvas canvas)
    {
        ScreenFader prefab = Resources.Load<ScreenFader>("GUI/ScreenFader");
        Assert.IsNotNull(prefab);
        return GameObject.Instantiate<ScreenFader>(prefab, canvas.transform, false);
    }

    private GameObject _getOrCreateGuiObject()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("GuiObject");
        if(obj)
        {
            return obj;
        }

        GameObject prefab = Resources.Load<GameObject>("GUI/GuiPrefab");
        Assert.IsNotNull(prefab);
        obj = GameObject.Instantiate<GameObject>(prefab, null, false);

        return obj;
    }
}
