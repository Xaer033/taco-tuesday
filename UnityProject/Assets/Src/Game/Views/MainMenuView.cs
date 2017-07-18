using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GhostGen;
using UnityEngine.UI;


public class MainMenuView : UIView 
{
    [System.Serializable]
    public class ButtonGroupOne
    {
        public GameObject _container;
        public Button _startButton;
        public Button _creditsButton;
        public Button _quitButton;
    }

    [System.Serializable]
    public class ButtonGroupTwo
    {
        public GameObject _container;
        public Button _singlePlayerBtn;
        public Button _passAndPlayBtn;
        public Button _onlineBtn;
        public Button _backBtn;
    }

    public ButtonGroupOne buttonGroupOne;
    public ButtonGroupTwo buttonGroupTwo;
    
}
