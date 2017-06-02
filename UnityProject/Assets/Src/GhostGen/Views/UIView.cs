using UnityEngine;
using System;
using UnityEngine.EventSystems;

namespace GhostGen
{
	public delegate void OnIntroTransitionHandle( UIView p_view );
	public delegate void OnOutroTransitionHandle( UIView p_view );
    public delegate void OnViewRemoved();

    public class UIView : MonoBehaviour
    {
        [System.Flags]
        public enum InvalidationFlag
        {
            NONE = 1 << 0,
            STATIC_DATA = 1 << 2,
            DYNAMIC_DATA = 1 << 3,

            ALL = STATIC_DATA | DYNAMIC_DATA
        }

        protected event Action<BaseEventData> _onTriggered;
        private InvalidationFlag _invalidateFlag = InvalidationFlag.ALL; // Default to invalidating everything

        public event Action<BaseEventData> onTriggered
        {
            add { _onTriggered += value; }
            remove { _onTriggered -= value; }
        }

        public InvalidationFlag invalidateFlag
        {
            get { return _invalidateFlag; }
            set { _invalidateFlag = value; }
        }
        
        public void Validate(InvalidationFlag flag = InvalidationFlag.ALL)
        {
            invalidateFlag |= flag;
            OnViewUpdate();
        }

        public event OnIntroTransitionHandle onIntroFinishedEvent
        {
            add { _introTransitionFinishEvent += value; }
            remove { _introTransitionFinishEvent -= value; }
        }

        public event OnOutroTransitionHandle onOutroTransitionEvent
        {
            add { _outroTransitionFinishEvent += value; }
            remove { _outroTransitionFinishEvent -= value; }
        }


        protected void OnIntroTransitionFinished()
        {
            if(_introTransitionFinishEvent != null)
            {
                _introTransitionFinishEvent(this);
            }
        }

        protected void OnOutroTransitionFinished()
        {
            if (_outroTransitionFinishEvent != null)
            {
                _outroTransitionFinishEvent(this);
            }
        }

        public void OnTriggered(BaseEventData eventData)
        {
            if(_onTriggered != null)
            {
                _onTriggered(eventData);
            }
        }
        protected virtual void OnViewUpdate()
        {
        }


        public virtual void OnViewOutro( Action finishedCallback)
        {
            if(finishedCallback != null)
            {
                finishedCallback();
            }
        }

        public virtual void OnViewDispose()
        {
            OnOutroTransitionFinished();
            
            _introTransitionFinishEvent = null;
            _outroTransitionFinishEvent = null;
        }

        protected bool IsInvalid(InvalidationFlag flag)
        {
            if (flag.IsFlagSet(InvalidationFlag.ALL)) { return true; }
            if(_invalidateFlag.IsFlagSet(InvalidationFlag.ALL)) { return true; }

            return _invalidateFlag.IsFlagSet(flag);         
        }
        
        public virtual void Update()
        {
            if(invalidateFlag != InvalidationFlag.NONE)
            {
                OnViewUpdate();
            }

            invalidateFlag = InvalidationFlag.NONE;
        }

        //------------------- Private Implementation -------------------
        //--------------------------------------------------------------
        private event OnIntroTransitionHandle _introTransitionFinishEvent;
        private event OnOutroTransitionHandle _outroTransitionFinishEvent;
    }
}