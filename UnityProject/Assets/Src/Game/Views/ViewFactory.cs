using UnityEngine;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;
using GhostGen;


public class ViewFactory
{
    internal class AsyncBlock
    {
        public static AsyncBlock Create(ResourceRequest p_request, OnViewCreated p_callback, Transform p_parent )
        {
            AsyncBlock block = new AsyncBlock();
            block.request = p_request;
            block.callback = p_callback;
            block.parent = p_parent;
            return block;
        }

        public ResourceRequest request;
        public OnViewCreated callback;
        public Transform    parent;
       
    }

    public delegate void OnViewCreated(UIView p_view);

    public Canvas canvas { get; set; }

    private ScreenFader _screenFader;
    private List<AsyncBlock> _asyncList = new List<AsyncBlock>();
    
       
    public ViewFactory(Canvas guiCanvas)
    {
        canvas = guiCanvas;
        _screenFader = _createScreenFader();
    }   

    public ScreenFader screenFader
    {
        get { return _screenFader; }
        set { _screenFader = value; }
    }

    public void Step()
    {
        int asyncLength = _asyncList.Count;
        for(int i = 0; i < asyncLength; ++i)
        { 
            AsyncBlock block = _asyncList[i];
            Assert.IsNotNull(block);
            Assert.IsNotNull(block.request);
            
            if(!block.request.isDone) { continue; }

            UIView view = _createView((UIView)block.request.asset, block.parent);
            Assert.IsNotNull(view);
            
            if(block.callback != null)
            {
                block.callback(view);
            }

            _asyncList[i] = null;        
        }

        for(int i = asyncLength-1; i >=0; --i)
        {
            if(_asyncList[i] == null)
            {
                _asyncList.RemoveAt(i);
            }
        }
    }

    public T Create<T>(string viewPath, Transform parent = null)
    {
        UIView viewBase = Resources.Load<UIView>("GUI/" + viewPath);
        Assert.IsNotNull(viewBase);
        
        return (T)(object)_createView(viewBase, parent);
    }


    public bool CreateAsync<T>(string viewPath, OnViewCreated callback, Transform parent = null)
    {
        Debug.Log("View At: " + viewPath);

        ResourceRequest request = Resources.LoadAsync<UIView>("GUI/" + viewPath);
        if (request == null) { return false; }

        AsyncBlock block = AsyncBlock.Create(request, callback, parent);
        _asyncList.Add(block);

        return true;
    }

    public void RemoveView(UIView view, bool immediately = false)
    {
        Assert.IsNotNull(view);
        if(immediately)
        {
            _removeView(view);
        }
        else
        {
            view.OnViewOutro(()=>
            {
                _removeView(view);
            });
        }
    }

    private void _removeView(UIView view)
    {
        if(view != null)
        {
            view.OnViewDispose();
            GameObject.Destroy(view.gameObject);     
        }
    }

    private UIView _createView(UIView viewBase, Transform parent)
    {
        Transform viewParent = (parent != null) ? parent : canvas.transform;
        Assert.IsNotNull(viewBase);
        return GameObject.Instantiate<UIView>(viewBase, viewParent, false);
    }

    private ScreenFader _createScreenFader()
    {
        ScreenFader prefab = Resources.Load<ScreenFader>("GUI/ScreenFader");
        Assert.IsNotNull(prefab);
        return GameObject.Instantiate<ScreenFader>(prefab, canvas.transform, false);
    }
}
