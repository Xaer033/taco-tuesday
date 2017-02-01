using UnityEngine;
using System.Collections;

namespace GhostGen
{
	public delegate void OnIntroTransitionHandle( UIView p_view );
	public delegate void OnOutroTransitionHandle( UIView p_view );
    public delegate void OnViewRemoved();

    public class UIView : MonoBehaviour
    {
        public const string INVALIDATE_ALL = "invalidate_all";
        protected const string INVALIDATE_STATIC_DATA = "invalidate_static_data";
        protected const string INVALIDATE_DYNAMIC_DATA = "invalidate_dynamic_data";

        private string _invalidateFlag = INVALIDATE_ALL; // Default to invalidating everything

        public string invalidateFlag
        {
            get { return _invalidateFlag; }
            set { _invalidateFlag = value; }
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

        protected virtual void OnViewUpdate()
        {
        }


        public virtual void OnViewOutro(bool immediately, OnViewRemoved removedCallback)
        {
            OnDisposeView(removedCallback);
        }

        protected virtual void OnDisposeView(OnViewRemoved removedCallback)
        {
            OnOutroTransitionFinished();
            
            _introTransitionFinishEvent = null;
            _outroTransitionFinishEvent = null;

            if (removedCallback != null)
            {
                removedCallback();
            }
        }

        protected virtual bool IsInvalid(string validateStr)
        {
            if (validateStr == INVALIDATE_ALL) { return true; }

            return _invalidateFlag == validateStr;         
        }





        public virtual void Update()
        {
            if(invalidateFlag != null)
            {
                OnViewUpdate();
            }

            invalidateFlag = null;
        }

        //------------------- Private Implementation -------------------
        //--------------------------------------------------------------
        private event OnIntroTransitionHandle _introTransitionFinishEvent;
        private event OnOutroTransitionHandle _outroTransitionFinishEvent;
    }
}