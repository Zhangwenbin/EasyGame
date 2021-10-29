using UnityEngine;

namespace EG
{
    public interface IEventTrackMarker
    {
        void OnMarkerStart( string id );
        void OnMarkerEnd( string id );
    }
    
    public class EventTrackMarker : EventTrack
    {
        public string ID;
        
        public override ECategory Category
        {
            get { return ECategory.Common; }
        }
        
        public override void OnStart( AppMonoBehaviour behaviour )
        {
            IEventTrackMarker marker = behaviour as IEventTrackMarker;
            if( marker != null )
            {
                marker.OnMarkerStart( ID );
            }
        }
        
        public override void OnEnd( AppMonoBehaviour behaviour )
        {
            IEventTrackMarker marker = behaviour as IEventTrackMarker;
            if( marker != null )
            {
                marker.OnMarkerEnd( ID );
            }
        }
        
        
        #if UNITY_EDITOR
        
        public static string    ClassName  { get { return "marker";                               } }
        public override Color   TrackColor { get { return new Color32( 0xff, 0xff, 0x00, 0xff );    } }
        
        #endif
    }
}
