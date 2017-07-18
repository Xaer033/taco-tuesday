using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GhostGen
{
    public interface IListItemView  
    {
        Hashtable   viewData    { get; set; }
        bool        isSelected  { get; set; }

        int         GetItemType();
        
        event Action<IListItemView, bool> OnSelected;
    }
}
