using System;
using System.Collections.Generic;
using GhostGen;

public class PassPlaySetupController : BaseController 
{
    private PassPlaySetupView _passPlaySetup;
    private Action _onStart;
    private Action _onCancel;
    
    public void Start(Action onStart, Action onCancel)
    {
        _onStart = onStart;
        _onCancel = onCancel;

        viewFactory.CreateAsync<PassPlaySetupView>("GameSetup/PassPlaySetupView", (passView) =>
        {
            _passPlaySetup = passView as PassPlaySetupView;
            view = _passPlaySetup;

            _passPlaySetup.startButton.onClick.AddListener(onStartButton);
            _passPlaySetup.cancelButton.onClick.AddListener(onCancelButton);

            for (int i = 0; i < PlayerGroup.kMaxPlayerCount; ++i)
            {
                _passPlaySetup.playerInputSetups[i].playerName.text = string.Format("Player {0}", (i+1).ToString());
            }
        });
    }

    public PlayerState[] GetPlayerList()
    {
        if(_passPlaySetup == null)
        {
            return null;
        }

        PlayerState[] playerStateList = new PlayerState[PlayerGroup.kMaxPlayerCount];
        for (int i = 0; i < PlayerGroup.kMaxPlayerCount; ++i)
        {
            TMPro.TMP_InputField input = _passPlaySetup.playerInputSetups[i].nameInput;
            playerStateList[i] = PlayerState.Create(i, input.text);
        }
        return playerStateList;
    }

    private void onStartButton()
    {
        _passPlaySetup.startButton.onClick.RemoveListener(onStartButton);
        if (_onStart != null)
        {
            _onStart();
        }
    }
    private void onCancelButton()
    {
        _passPlaySetup.cancelButton.onClick.RemoveListener(onCancelButton);
        if (_onCancel != null)
        {
            _onCancel();
        }
    }
}
