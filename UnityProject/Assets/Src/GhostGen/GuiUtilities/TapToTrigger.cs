using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using GhostGen;
using UnityEngine.Assertions;

[RequireComponent(typeof(EventTrigger))]
public class TapToTrigger : MonoBehaviour 
{
    private EventTrigger _eventTrigger;
    private UIView _view;
    private EventTrigger.Entry _onClickEntry;

	// Use this for initialization
	void Start () 
	{
        _eventTrigger = GetComponent<EventTrigger>();
        _view = GetComponent<UIView>();
        Assert.IsNotNull(_eventTrigger);
        Assert.IsNotNull(_view);

        _onClickEntry = _eventTrigger.triggers.Find((x) => x.eventID == EventTriggerType.PointerClick);
        if(_onClickEntry == null)
        {
            _onClickEntry = new EventTrigger.Entry();
            _eventTrigger.triggers.Add(_onClickEntry);
        }
        _onClickEntry.eventID = EventTriggerType.PointerClick;
        _onClickEntry.callback.AddListener(_onClick);
    }

    void OnDestroy()
    {
        if(_onClickEntry != null)
        {
            _onClickEntry.callback.RemoveListener(_onClick);
        }
    }

    private void _onClick(BaseEventData e)
    {
        if(_view != null)
        {
            _view.OnTriggered(e);
        }
    }
}
