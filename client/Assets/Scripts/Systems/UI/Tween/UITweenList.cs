using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace EG
{
    public class UITweenList 
    {
        public class Group
        {
            public UITweenList      owner;
            public string           id;
            public UITweener[]      tweens;

            public T GetComponent<T>() where T : UITweener
            {
                if( tweens != null )
                {
                    for( int i = 0; i < tweens.Length; ++i )
                    {
                        if( tweens[i] is T )
                        {
                            return tweens[i] as T;
                        }
                    }
                }
                return null;
            }
            
            public void Play( bool forward, float speed )
            {
                Play( forward, speed, false );
            }
            
            public void Play( bool forward, float speed, bool includeInactive )
            {
                if( tweens != null )
                {
                    for( int i = 0; i < tweens.Length; ++i )
                    {
                        if( tweens[ i ] != null )
                        {
                            if( includeInactive == false )
                            {
                                if( tweens[ i ].gameObject.activeInHierarchy == false )
                                {
                                    continue;
                                }
                            }
                            
                            tweens[ i ].Speed( speed );
                            tweens[ i ].ResetPlay( forward );
                        }
                    }
                }
                owner.Current = this;
            }
            
            public void Stop( bool reset )
            {
                if( tweens != null )
                {
                    for( int i = 0; i < tweens.Length; ++i )
                    {
                        if( tweens[i] != null )
                        {
                            if( reset )
                            {
                                tweens[i].ResetToBeginning();
                            }
                            tweens[i].enabled = false;
                        }
                    }
                }
                owner.Current = this;
            }
            
            public void Reset( )
            {
                if( tweens != null )
                {
                    for( int i = 0; i < tweens.Length; i++ )
                    {
                        if( tweens[i] != null )
                        {
                            tweens[i].ResetToBeginning( );
                            tweens[i].enabled = false;
                        }
                    }
                }
                owner.Current = this;
            }
            
            public bool IsEnable()
            {
                if( tweens != null )
                {
                    for( int i = 0; i < tweens.Length; ++i )
                    {
                        if( tweens[i] == null )
                        {
                            continue;
                        }
                        
                        //if( tweens[i].style == UITweener.Style.Loop || tweens[i].style == UITweener.Style.PingPong )
                        //{
                        //  continue;
                        //}
                        
                        if( tweens[i].gameObject.activeInHierarchy && tweens[i].enabled )
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            
            public bool IsDisable()
            {
                return !IsEnable();
            }
            
            public void SetOnFinished( System.Action<object> callback, object callbackValue )
            {
                if( tweens.Length > 0 && tweens[0] != null )
                {
                    tweens[ 0 ].SetOnFinished( callback, callbackValue );
                }
            }
        }
        

        GameObject              m_Owner     = null;
        UITweener.GroupType     m_GroupType = UITweener.GroupType.Free;
        List<Group>             m_Tweens    = new List<Group>();
        
        bool                    m_Pause     = false;
        List<UITweener>         m_PauseList = new List<UITweener>();
        
        Group                   m_Current   = null;
        
        public bool hasGroup
        {
            get { return m_Tweens.Count > 0; }
        }
        
        public Group[] Groups
        {
            get { return m_Tweens.ToArray(); }
        }
        
        public UITweener.GroupType GroupType
        {
            get { return m_GroupType; }
        }
        
        public Group Current
        {
            set { m_Current = value; }
            get { return m_Current; }
        }
        
        public bool TweenPause( bool isPaused )
        {
            if( isPaused )
            {
                if( m_Pause == false )
                {
                    for( int i = 0; i < m_Tweens.Count; ++i )
                    {
                        if( m_Tweens[i] == null || m_Tweens[i].tweens == null )
                        {
                            continue;
                        }
                        
                        for( int j = 0; j < m_Tweens[i].tweens.Length; ++j )
                        {
                            UITweener tw = m_Tweens[i].tweens[j];
                            if( tw != null )
                            {
                                if( tw.isActiveAndEnabled )
                                {
                                    tw.enabled = false;
                                    m_PauseList.Add( tw );
                                }
                            }
                        }
                    }
                    
                    m_Pause = true;
                }
            }
            else
            {
                if( m_Pause )
                {
                    for( int i = 0; i < m_PauseList.Count; ++i )
                    {
                        UITweener tw = m_PauseList[i];
                        if( tw != null )
                        {
                            tw.enabled = true;
                        }
                    }
                    m_PauseList.Clear();
                    
                    m_Pause = false;
                }
            }
            
            return m_Pause;
        }
        
        public void Initialize( GameObject owner, bool includeInactive, UITweener.GroupType groupType, string[] groups )
        {
            m_Owner = owner;
            
            m_GroupType = groupType;
            m_Tweens.Clear();
            
            UITweener[] tweens = m_Owner.GetComponentsInChildren<UITweener>( includeInactive );
            if( tweens != null )
            {
                if( groups != null && groups.Length > 0 )
                {
                    for( int i = 0; i < tweens.Length; ++i )
                    {
                        if( tweens[i].tweenGroupType == groupType )
                        {
                            for( int grp = 0; grp < groups.Length; ++grp )
                            {
                                if( groups[grp] == tweens[i].tweenGroup )
                                {
                                    //tweens[i].enabled = false;
                                    AddTweener( tweens[i] );
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    for( int i = 0; i < tweens.Length; ++i )
                    {
                        if( tweens[i].tweenGroupType == groupType )
                        {
                            //tweens[i].enabled = false;
                            AddTweener( tweens[i] );
                        }
                    }
                }
            }
        }
        
        public void Initialize( GameObject owner, UITweener.GroupType groupType )
        {
            Initialize( owner, true, groupType, null );
        }
        public void Initialize( GameObject owner, bool includeInactive, UITweener.GroupType groupType )
        {
            Initialize( owner, includeInactive, groupType, null );
        }
        public void Initialize( GameObject owner, UITweener.GroupType groupType, string[] groups )
        {
            Initialize( owner, true, groupType, groups );
        }
        

        public void Release()
        {
            m_Tweens.Clear();
        }
        
        public void Clear( )
        {
            m_Tweens.Clear();
        }
        
        
        public void AddTweener( UITweener tweener )
        {
            Group grp = GetGroup( tweener.tweenGroup );
            
            if( grp != null )
            {
                int n = grp.tweens.Length;
                System.Array.Resize( ref grp.tweens, n+1 );
                grp.tweens[n] = tweener;
            }
            else
            {
                grp = new Group();
                
                grp.owner = this;
                grp.id = tweener.tweenGroup;
                grp.tweens = new UITweener[1];
                grp.tweens[0] = tweener;
                
                m_Tweens.Add( grp );
            }
        }
        
        public Group GetGroup( string grpId )
        {
            return m_Tweens.Find( delegate( Group obj ){ return obj.id == grpId; } );
        }
        public Group GetGroup( UITweener.WindowGroup grpId )
        {
            return m_Tweens.Find( delegate( Group obj ){ return obj.id == grpId.ToString(); } );
        }
        public Group GetGroup( UITweener.EventGroup grpId )
        {
            return m_Tweens.Find( delegate( Group obj ){ return obj.id == grpId.ToString(); } );
        }
        
        public UITweener[] GetTweens()
        {
            if( m_Tweens == null || m_Tweens.Count <= 0 )
            {
                return null;
            }
            
            List<UITweener> list = new List <UITweener>();
            foreach( Group grp in m_Tweens )
            {
                list.AddRange( grp.tweens );
            }
            
            return list.ToArray();
        }
        
        public void Reset( string grpId )
        {
            Group grp = GetGroup( grpId );
            if( grp != null )
            {
                grp.Reset( );
            }
        }
        public void Reset( UITweener.WindowGroup grpId )
        {
            Group grp = GetGroup( grpId.ToString() );
            if( grp != null )
            {
                grp.Reset( );
            }
            else if( grpId == UITweener.WindowGroup.Close )
            {
                grp = GetGroup( UITweener.WindowGroup.Open );
                if( grp != null )
                {
                    grp.Reset( );
                }
            }
        }
        public void Reset( UITweener.EventGroup grpId )
        {
            Group grp = GetGroup( grpId.ToString() );
            if( grp != null )
            {
                grp.Reset( );
            }
        }
        public void Reset( )
        {
            if( m_Tweens != null )
            {
                for( int i = 0; i < m_Tweens.Count; ++i )
                {
                    if( m_Tweens[i] != null )
                    {
                        m_Tweens[i].Reset( );
                    }
                }
            }
        }
        
        public void Play( string grpId, bool forward, float speed )
        {
            Group grp = GetGroup( grpId );
            if( grp != null )
            {
                grp.Play( forward, speed );
            }
        }
        public void Play( UITweener.WindowGroup grpId, bool forward, float speed )
        {
            Group grp = GetGroup( grpId.ToString() );
            if( grp != null )
            {
                grp.Play( forward, speed );
            }
            else if( grpId == UITweener.WindowGroup.Close )
            {
                grp = GetGroup( UITweener.WindowGroup.Open );
                if( grp != null )
                {
                    grp.Play( false, speed );
                }
            }
        }
        public void Play( UITweener.EventGroup grpId, bool forward, float speed )
        {
            Group grp = GetGroup( grpId.ToString() );
            if( grp != null )
            {
                grp.Play( forward, speed );
            }
        }
        
        public void Stop( string grpId )
        {
            Group grp = GetGroup( grpId );
            if( grp != null )
            {
                grp.Stop( true );
            }
        }
        public void Stop( UITweener.WindowGroup grpId )
        {
            Stop( grpId.ToString() );
        }
        public void Stop( UITweener.EventGroup grpId )
        {
            Stop( grpId.ToString() );
        }
        public void Stop( bool reset )
        {
            if( m_Tweens != null )
            {
                for( int i = 0; i < m_Tweens.Count; ++i )
                {
                    if( m_Tweens[i] != null && m_Tweens[i].IsEnable() )
                    {
                        m_Tweens[i].Stop( reset );
                    }
                }
            }
        }
        
        public bool HasGroup( string grpId )
        {
            return m_Tweens.Find( delegate( Group obj ){ return obj.id == grpId; } ) != null;
        }
        public bool HasGroup( UITweener.WindowGroup grpId )
        {
            return m_Tweens.Find( delegate( Group obj ){ return obj.id == grpId.ToString(); } ) != null;
        }
        public bool HasGroup( UITweener.EventGroup grpId )
        {
            return m_Tweens.Find( delegate( Group obj ){ return obj.id == grpId.ToString(); } ) != null;
        }
        
        public bool IsEnable( string grpId )
        {
            Group grp = GetGroup( grpId.ToString() );
            if( grp != null )
            {
                return grp.IsEnable( );
            }
            return false;
        }
        public bool IsEnable( UITweener.WindowGroup grpId )
        {
            Group grp = GetGroup( grpId );
            if( grp != null )
            {
                return grp.IsEnable( );
            }
            else if( grpId == UITweener.WindowGroup.Close )
            {
                grp = GetGroup( UITweener.WindowGroup.Open );
                return grp.IsEnable( );
            }
            return false;
        }
        public bool IsEnable( UITweener.EventGroup grpId )
        {
            Group grp = GetGroup( grpId );
            if( grp != null )
            {
                return grp.IsEnable( );
            }
            return false;
        }
        
        public bool IsDisable( string grpId )
        {
            return !IsEnable( grpId );
        }
        public bool IsDisable( UITweener.WindowGroup grpId )
        {
            return !IsEnable( grpId );
        }
        public bool IsDisable( UITweener.EventGroup grpId )
        {
            return !IsEnable( grpId );
        }
        
    }
}

