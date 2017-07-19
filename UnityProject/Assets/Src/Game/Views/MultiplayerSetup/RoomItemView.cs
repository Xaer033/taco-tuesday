using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GhostGen;
using UnityEngine.EventSystems;

public class RoomItemView : UIView, IListItemView
{
    public Toggle toggle;
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI playerCount;


    private Hashtable _data;
    private event Action<IListItemView, bool> _onSelected;

    private void Start()
    {
        //onTriggered += OnSelected;
        toggle.onValueChanged.AddListener(OnButtonClick);
    }
    

    public event Action<IListItemView, bool> OnSelected
    {
        add { _onSelected += value; }
        remove { _onSelected -= value; }
    }

    public int GetItemType()
    {
        return 0;
    }
    
    public bool isSelected { get; set; }

    public Hashtable viewData
    {
        get { return _data; }
        set
        {
            if(_data != value)
            {
                _data = value;
                invalidateFlag = InvalidationFlag.DYNAMIC_DATA;
            }
        }
    }

    private void OnButtonClick(bool value)
    {
        if(_onSelected != null)
        {
            _onSelected(this, value);
        }
    }

    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
        if(IsInvalid(InvalidationFlag.DYNAMIC_DATA) && _data != null)
        {
            roomName.text = _data["roomName"] as string;
            playerCount.text = _data["playerCount"] as string;
            
            toggle.onValueChanged.RemoveListener(OnButtonClick);
            toggle.isOn = isSelected;
            toggle.onValueChanged.AddListener(OnButtonClick);
        }
    }
}
