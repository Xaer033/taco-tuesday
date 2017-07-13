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

    private List<Hashtable> info;

    void Awake()
    {
        info = new List<Hashtable>();
        for(int i = 0; i < 8000; ++i)
        {
            Hashtable room = new Hashtable();
            room.Add("roomName", string.Format("Room: {0}", i));
            room.Add("playerCount", string.Format("{0}/4", UnityEngine.Random.Range(1, 5)));
            info.Add(room);
        }

        _listScrollRect.itemRendererFactory = itemRendererFactory;
        _listScrollRect.dataProvider = info;
        _listScrollRect.onSelectedItem += onItemClicked;
    }

    private void onItemClicked(int index)
    {
        Debug.Log("Item: " + info[index]["roomName"]);
    }
    protected override void OnViewUpdate()
    {
        base.OnViewUpdate();
    }
    
    private GameObject itemRendererFactory(int itemType, Transform parent)
    {
        RoomItemView view = Instantiate(_listItemPrefab, parent, false);
        view.toggle.group = _toggleGroup;
        return view.gameObject;
    }
}
