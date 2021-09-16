using UnityEngine;
using System.Collections;

namespace EG
{
    [AddComponentMenu("Scripts/System/Manager/TimerManager")]
    public class TimerManager : MonoSingleton<TimerManager>
    {
        public static int                   FPS_NORMAL          = 30;
        public static int                   FPS_LOW             = 20;
        
        public static float                 FPS_60              = 1.0f / 60.0f;
        public static float                 FPS_30              = 1.0f / 30.0f;
        public static float                 FPS_20              = 1.0f / 20.0f;
        
        public static readonly string       ISO_8601_FORMAT     = "yyyy-MM-ddTHH:mm:ss.fffZ";
        
        // 
        #region フィールド
        
        [SerializeField]    private bool    m_Burdening         = false;
        [SerializeField]    private bool    m_FixFrameRate      = false;
                            private int     m_FrameRate         = FPS_NORMAL;
        [SerializeField]    private float   m_SpeedRate         = 1.0f;
                            
                            private float   m_DeltaTimeSystem   = 0.0f;
                            private float   m_DeltaTime         = 0.0f;
                            private float   m_UnscaledDeltaTime = 0.0f;
                            private float   m_Fps               = 1.0f / 60.0f;
                            
                            private float   m_LocalSpeedRate    = 1.0f;
                            
                            private float   m_SlowNowTime       = 0.0f;
                            private float   m_SlowTime          = 0.0f;
                            private float   m_SlowTime_S        = 0.0f;
                            private float   m_SlowTime_E        = 0.0f;
                            private float   m_SlowRate          = 0.0f;
                            
                            private float   m_Time              = 0.0f;
                            private float   m_SinceTime         = 0.0f;
                            
                            private long    m_FrameCount        = 0;
                            private long    m_FrameStartCount   = 0;
        #endregion
        
        // プロパティ
        #region プロパティ
        
        // フレームカウントの取得
        public static long      FrameCount
        {
            get { return Instance.m_FrameCount; }
        }
        public static long      FrameDeltaCount
        {
            get { return Instance.m_FrameCount - Instance.m_FrameStartCount; }
        }
        
        // 処理落ちレートの取得
        public static float     DeltaFall
        {
            get { return Instance.m_DeltaTime * (float)Instance.m_FrameRate; }
        }
        
        // フレームレートの取得
        public static int       FrameRate
        {
            get { return Instance.m_FrameRate == -1 ? 60: Instance.m_FrameRate; }
            set { Instance.m_FrameRate = value; }
        }
        
        // FPSの取得
        public static float     Fps
        {
            get { return Instance.m_Fps;        }
        }
        
        // 可変更新時間の取得(速度レートの影響を受けない)
        public static float     DeltaTimeSystem
        {
            get { return Instance.m_DeltaTimeSystem; }
        }
        // 可変更新時間の取得(速度レートの影響を受ける)
        public static float     DeltaTime
        {
            get { return Instance.m_DeltaTime; }
        }
        public static float     UnscaledDeltaTime
        {
            get { return Instance.m_UnscaledDeltaTime; }
        }
        
        // ゲーム起動からの経過時間
        public static float     Time
        {
            get { return Instance.m_Time; }
        }
        
        // 速度レート
        public static float     SpeedRate
        {
            set { Instance.m_SpeedRate = value;     }
            get { return Instance.m_SpeedRate;      }
        }
        
        // ローカル速度レート
        public static float     LocalSpeedRate
        {
            set { Instance.m_LocalSpeedRate = value;     }
            get { return Instance.m_LocalSpeedRate;      }
        }
        
        // レベルロードからの経過時間
        public static float     SinceTime
        {
            get { return Instance.m_SinceTime; }
        }
        
        public static bool      isActiveSlow
        {
            get { return Instance.m_SlowTime > 0; }
        }
        
        #endregion
        
        //=========================================================================
        //. 初期化
        //=========================================================================
        #region 初期化
        
        /// ***********************************************************************
        /// <summary>
        /// 初期化
        /// </summary>
        /// ***********************************************************************
        public override void Initialize()
        {
            if( isInitialized )
            {
                return;
            }
            
            base.Initialize();
            
            Application.targetFrameRate = -1;
            
            // タイマー時間を固定にする
    #if FIX_FRAMERATE
            Time.captureFramerate = m_FrameRate;
    #endif
        }
        
        #endregion
        
        //=========================================================================
        //. 更新
        //=========================================================================
        #region 更新
        
        /// ***********************************************************************
        /// <summary>
        /// 更新
        /// </summary>
        /// ***********************************************************************
        private void Update()
        {
            float rate = 1.0f;
            
            // フレームカウントの保存
            m_FrameCount = UnityEngine.Time.frameCount;
            
            // フレーム開始時間
            m_Time += m_DeltaTime; 
            
            if( m_SlowTime > 0 )
            {
                m_SlowNowTime += UnityEngine.Time.unscaledDeltaTime;
                if( m_SlowNowTime >= m_SlowTime )
                {
                    m_SlowTime = 0.0f;
                }
                else
                {
                    float def = rate;
                    
                    rate = m_SlowRate;
                    
                    if( m_SlowNowTime < m_SlowTime_S )
                    {
                        float t = m_SlowNowTime / m_SlowTime_S;
                        rate = def + ( m_SlowRate - def ) * t;
                    }
                    else if( m_SlowNowTime > m_SlowTime_E )
                    {
                        float t = ( m_SlowNowTime - m_SlowTime_E ) / ( m_SlowTime - m_SlowTime_E );
                        rate = m_SlowRate + ( def - m_SlowRate ) * t;
                    }
                }
            }
            
            if( Application.targetFrameRate != m_FrameRate )
            {
                if( m_FrameRate != -1 )
                {
                    QualitySettings.vSyncCount = 0;
                }
                else
                {
                    QualitySettings.vSyncCount = 1;
                }
                Application.targetFrameRate = m_FrameRate;
            }
            if( Application.targetFrameRate != -1 )
            {
                m_Fps = 1.0f / (float)Application.targetFrameRate;
            }
            
            if( m_FixFrameRate == false )
            {
                // 可変時間
                m_DeltaTimeSystem   = 
                m_DeltaTime         = UnityEngine.Time.deltaTime;
                m_UnscaledDeltaTime = UnityEngine.Time.timeScale == 0 ? UnityEngine.Time.fixedDeltaTime: UnityEngine.Time.deltaTime / UnityEngine.Time.timeScale;
                m_SinceTime         = UnityEngine.Time.timeSinceLevelLoad;
            }
            else
            {
                // 固定時間
                m_DeltaTimeSystem   = 
                m_DeltaTime         = m_Fps * m_SpeedRate * m_LocalSpeedRate * rate;
                m_UnscaledDeltaTime = m_Fps;
            }
            
            // 擬似処理負荷モード
            if( m_Burdening && m_FrameRate > 0 )
            {
                Application.targetFrameRate = (int)( m_FrameRate + (float)m_FrameRate * Random.Range( -0.5f, 0.0f ) );
                //Application.targetFrameRate = (int)( m_FrameRate + (float)m_FrameRate * Random.Range( -0.8f, 0.0f ) );
            }
            
            // 速度レートを掛ける
            //m_DeltaTime *= m_SpeedRate;
            UnityEngine.Time.timeScale = m_SpeedRate * m_LocalSpeedRate * rate;
            
        }
        
#endregion
        
        //=========================================================================
        //. スロー
        //=========================================================================
#region スロー
        
        /// ***********************************************************************
        /// <summary>
        /// スロー開始
        /// </summary>
        /// <param name="time">スロー最大時間</param>
        /// <param name="time_s">スロー開始時間</param>
        /// <param name="time_e">スロー戻り時間</param>
        /// <param name="rate">スローレート</param>
        /// <returns></returns>
        /// <remarks></remarks>
        /// ***********************************************************************
        public void SetSlow( float time, float time_s, float time_e, float rate )
        {
            m_SlowNowTime   = 0.0f;
            m_SlowTime      = time;
            m_SlowTime_S    = time_s;
            m_SlowTime_E    = time_e;
            m_SlowRate      = rate;
        }
        
        public void SetSlow( float time, float rate )
        {
            SetSlow( time, 0.0f, time, rate );
        }
        
        public void SetSlow( float time, float time_e, float rate )
        {
            SetSlow( time, 0.0f, time_e, rate );
        }
        
        /// ***********************************************************************
        /// <summary>
        /// スロー解除
        /// </summary>
        /// ***********************************************************************
        public void ResetSlow( )
        {
            m_SlowTime      = 0;
        }
        
#endregion
        
        //=========================================================================
        //. 設定/取得
        //=========================================================================
#region 設定/取得
        
        /// ***********************************************************************
        /// <summary>
        /// フレームカウントの初期値を設定
        /// </summary>
        /// ***********************************************************************
        public static void SetFrameStartCount( )
        {
            TimerManager inst = TimerManager.Instance;
            
            inst.m_FrameStartCount = inst.m_FrameCount;
        }
        
        /// ***********************************************************************
        /// <summary>
        /// 係数を速度倍率に合わせて補正
        /// </summary>
        /// ***********************************************************************
        public static float CalcValueSpeedRate( ref float value, float rate )
        {
            TimerManager inst = TimerManager.Instance;
            
            value *= rate;
            float result = value;
            if( inst != null )
            {
                float speed = inst.m_SpeedRate * inst.m_LocalSpeedRate;
                for( int i = 1; i < speed; ++i )
                {
                    value *= rate;
                    result += value;
                }
            }
            
            return result;
        }
        
#endregion
    }
}
