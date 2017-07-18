using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GhostGen;
using System;

public class MultiplayerLobbyView : UIView
{
    public ListScrollRect   _listScrollRect = null;
    public RoomItemView     _listItemPrefab = null;
    public ToggleGroup      _toggleGroup;
    public Button           _joinButton;
    public Button           _backButton;
    public Button           _createButton;
    
    void Awake()
    {
        _joinButton.interactable = false;
    }
    
    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
    }
    
}
