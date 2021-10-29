using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace EG
{
    [AddComponentMenu("Scripts/System/UI/Tween/UITweener")]
    public abstract class UITweener : UnityEngine.MonoBehaviour
    {
        static public UITweener current;
        
        public enum Style
        {
            Once,
            Loop,
            PingPong,
        }
        
        public enum GroupType
        {
            Free,
            Window,
            Event,
        }
        
        public enum WindowGroup
        {
            None,
            Open,
            Close,
            Loop,
        }
        
        public enum EventGroup
        {
            None,
            PointerUp,
            PointerDown,
            PointerClick,
            PointerEnter,
            PointerExit,
        }

        public static string[] EventGroupInputNames = new string[]
        {
            "None",
            "Up",
            "Down",
            "Click",
            "Enter",
            "Exit",
        };
        public static string GetInputName( EventGroup eventGroup )
        {
            return EventGroupInputNames[ (int)eventGroup ];
        }
        
        [HideInInspector]
        public UIEaseType method = UIEaseType.none;
        
        [HideInInspector]
        public Style style = Style.Once;
        
        [HideInInspector]
        public AnimationCurve animationCurve = new AnimationCurve(new Keyframe(0f, 0f, 0f, 1f), new Keyframe(1f, 1f, 1f, 0f));
        
        [HideInInspector]
        public bool ignoreTimeScale = true;
        
        [HideInInspector]
        public float delay = 0f;
        
        [HideInInspector]
        public float duration = 1f;
        
        [HideInInspector]
        public bool steeperCurves = false;
        
        [HideInInspector]
        public GroupType tweenGroupType = GroupType.Free;
        
        [HideInInspector]
        public string tweenGroup = "";
        
        public System.Action<object> onFinished = null;
        private object onFinishedValue = null;
        
        public UnityAction onUpdate = null;
        
        [HideInInspector]
        public GameObject eventReceiver;
        
        [HideInInspector]
        public string callWhenFinished;
        
        bool mStarted = false;
        float mStartTime = 0f;
        float mTime = 0f;
        float mDuration = 0f;
        float mAmountPerDelta = 1000f;
        float mFactor = 0f;
        float mSpeed = 1f;
        
        /// <summary>
        /// Amount advanced per delta time.
        /// </summary>
        public float amountPerDelta
        {
            get
            {
                if (mDuration != duration)
                {
                    mDuration = duration;
                    mAmountPerDelta = Mathf.Abs((duration > 0f) ? 1f / duration : 1000f) * Mathf.Sign(mAmountPerDelta);
                }
                return mAmountPerDelta;
            }
        }
        
        public float tweenFactor { get { return mFactor; } set { mFactor = Mathf.Clamp01(value); } }
        
        public UIDirection direction { get { return amountPerDelta < 0f ? UIDirection.Reverse : UIDirection.Forward; } }
        
        void Reset()
        {
            if (!mStarted)
            {
                SetStartToCurrentValue();
                SetEndToCurrentValue();
            }
        }
        
        protected virtual void Start() { Update(); }
        
#if UNITY_EDITOR
        public virtual void OnInspectorGUI() {}
#endif
        
        void Update( )
        {
            //float delta = ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
            //float time = ignoreTimeScale ? Time.unscaledTime : Time.time;
            float delta = TimerManager.DeltaTime * mSpeed;
            float time = TimerManager.Time;
            
            if( !mStarted )
            {
                mStarted = true;
                mTime = time;
                mStartTime = time + delay;
            }
            
            mTime += delta;
            if( mTime < mStartTime ) return;
            
            // Advance the sampling factor
            mFactor += amountPerDelta * delta;
            
            // Loop style simply resets the play factor after it exceeds 1.
            if( style == Style.Loop )
            {
                if( mFactor > 1f )
                {
                    mFactor -= Mathf.Floor( mFactor );
                }
            }
            else if( style == Style.PingPong )
            {
                // Ping-pong style reverses the direction
                if( mFactor > 1f )
                {
                    mFactor = 1f - (mFactor - Mathf.Floor( mFactor ));
                    mAmountPerDelta = -mAmountPerDelta;
                }
                else if( mFactor < 0f )
                {
                    mFactor = -mFactor;
                    mFactor -= Mathf.Floor( mFactor );
                    mAmountPerDelta = -mAmountPerDelta;
                }
            }
            
            // If the factor goes out of range and this is a one-time tweening operation, disable the script
            if( (style == Style.Once) && (duration == 0f || mFactor > 1f || mFactor < 0f) )
            {
                mFactor = Mathf.Clamp01( mFactor );
                Sample( mFactor, true );
                enabled = false;
                
                if( current != this )
                {
                    UITweener before = current;
                    current = this;
                    
                    if( onFinished != null )
                    {
                        onFinished( onFinishedValue );
                    }
                    
                    // Deprecated legacy functionality support
                    if( eventReceiver != null && !string.IsNullOrEmpty( callWhenFinished ) )
                        eventReceiver.SendMessage( callWhenFinished, this, SendMessageOptions.DontRequireReceiver );
                    
                    current = before;
                }
            }
            else Sample( mFactor, false );
        }
        
        public void SetOnFinished( System.Action<object> finishedCallBack, object finishValue )
        {
            onFinished = finishedCallBack;
            onFinishedValue = finishValue;
        }
        
        public void AddOnFinished(UnityEvent finishedCallBack) {  }
        
        public void RemoveOnFinished(UnityEvent finishedCallBack)
        {
           
        }
        
        void OnDisable() { mStarted = false; }
        
        /// <summary>
        /// Sample the tween at the specified factor.
        /// </summary>
        public void Sample( float factor, bool isFinished )
        {
            float val = Mathf.Clamp01( factor );
            
            val = (method == UIEaseType.none) ? animationCurve.Evaluate( val ) : UIEaseManager.EasingFromType( 0, 1, val, method );
            
            // Call the virtual update
            OnUpdate( val, isFinished );
            
            if( onUpdate != null )
            {
                onUpdate.Invoke();
            }
        }
        

        public void PlayForward() { Play(true); }
        

        public void PlayReverse() { Play(false); }


        public void ResetPlay(bool forward)
        {
            mAmountPerDelta = Mathf.Abs(amountPerDelta);
            if (!forward) mAmountPerDelta = -mAmountPerDelta;
            enabled = true;
            ResetToBeginning( );
            OnPlay();
            Update();
        }
        
        public void Play(bool forward)
        {
            mAmountPerDelta = Mathf.Abs(amountPerDelta);
            if (!forward) mAmountPerDelta = -mAmountPerDelta;
            enabled = true;
            OnPlay();
            Update();
        }
        
        public void Speed( float speed )
        {
            mSpeed = speed;
        }
        
        protected virtual void OnPlay(){}
        
        /// <summary>
        /// Manually reset the tweener's state to the beginning.
        /// If the tween is playing forward, this means the tween's start.
        /// If the tween is playing in reverse, this means the tween's end.
        /// </summary>
        public void ResetToBeginning()
        {
            mStarted = false;
            mFactor = (amountPerDelta < 0f) ? 1f : 0f;
            Sample(mFactor, false);
        }
        
        /// <summary>
        /// Manually start the tweening process, reversing its direction.
        /// </summary>
        public void Toggle()
        {
            if (mFactor > 0f)
            {
                mAmountPerDelta = -amountPerDelta;
            }
            else
            {
                mAmountPerDelta = Mathf.Abs(amountPerDelta);
            }
            enabled = true;
        }
        
        /// <summary>
        /// Actual tweening logic should go here.
        /// </summary>
        abstract protected void OnUpdate(float factor, bool isFinished);
                 
        public virtual void SetStartToCurrentValue() { }
        public virtual void SetEndToCurrentValue() { }
        public virtual void SetCurrentValueToStart() { }
        public virtual void SetCurrentValueToEnd() { }
    }
}
