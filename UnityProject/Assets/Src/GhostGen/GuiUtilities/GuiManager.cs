using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu]
public class GuiManager : ScriptableObject, IPostInit
{
    public Canvas       mainCanvas      { get; private set; }
    public Camera       guiCamera       { get; private set; }

    public ScreenFader  screenFader     { get; private set; }
    public ViewFactory  viewFactory     { get; private set; }
  
    public void Awake()
    {

    }

    public void PostInit()
    {
        _createGuiObject();

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

    private GameObject _createGuiObject()
    {
        GameObject prefab = Resources.Load<GameObject>("GUI/GuiPrefab");
        Assert.IsNotNull(prefab);
        GameObject instance = GameObject.Instantiate<GameObject>(prefab, null, false);

        mainCanvas  = instance.GetComponentInChildren<Canvas>();
        guiCamera   = instance.GetComponentInChildren<Camera>();
        return instance;
    }
}
