using UnityEngine;
using System.Collections;

namespace GhostGen
{
	public delegate void OnIntroTransitionHandle( UIView p_view );
	public delegate void OnOutroTransitionHandle( UIView p_view );
    public delegate void OnCreationFinishedHandle(UIView p_view);


    public class UIView : MonoBehaviour
    {
        public event OnCreationFinishedHandle OnCreationFinishedEvent
        {
            add { _creationFinishedEvent += value; }
            remove { _creationFinishedEvent -= value; }
        }

        public event OnIntroTransitionHandle OnIntroTransitionEvent
        {
            add { _introTransitionFinishEvent += value; }
            remove { _introTransitionFinishEvent -= value; }
        }

        public event OnOutroTransitionHandle OnOutroTransitionEvent
        {
            add { _outroTransitionFinishEvent += value; }
            remove { _outroTransitionFinishEvent -= value; }
        }

        public virtual void OnCreationFinished()
        {
        }

        public virtual void OnIntroTransitionFinished()
        {
            if(_introTransitionFinishEvent != null)
            {
                _introTransitionFinishEvent(this);
            }
        }

        public virtual void OnOutroTransitionFinished()
        {
            if (_outroTransitionFinishEvent != null)
            {
                _outroTransitionFinishEvent(this);
            }
        }

        public virtual void OnDispose()
        {
            _introTransitionFinishEvent = null;
            _outroTransitionFinishEvent = null;
        }


        //------------------- Private Implementation -------------------
        //--------------------------------------------------------------	
        private event OnIntroTransitionHandle _introTransitionFinishEvent;
        private event OnOutroTransitionHandle _outroTransitionFinishEvent;
        private event OnCreationFinishedHandle _creationFinishedEvent;
    }
}