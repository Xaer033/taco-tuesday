using System;
using GhostGen;

public class PassInterludeController : BaseController 
{
    private PassInterlude _passInterlude;
    private Action _introFinshed;

	public void Start(string instructionText, Action introFinishedCallback)
    {
        _introFinshed = introFinishedCallback;
        viewFactory.CreateAsync<PassInterlude>("PassInterlude", (view) =>
        {
            _passInterlude = view as PassInterlude;
            _passInterlude.instructionText = instructionText;
            _passInterlude._confirmButton.onClick.AddListener(OnConfirm);
            _passInterlude.onIntroFinishedEvent += OnIntroFinished;
        });
    }

    private void OnIntroFinished(UIView view)
    {
        if(_introFinshed != null)
        {
            _introFinshed();
        }
    }
    private void OnConfirm()
    {
        _passInterlude._confirmButton.onClick.RemoveListener(OnConfirm);
        viewFactory.RemoveView(_passInterlude);
    }
}
