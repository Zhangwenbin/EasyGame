using UnityEngine;


namespace EG
{
    public class Game : MonoBehaviour
    {
        private static Game Instance;

        private void Awake()
        {
            Instance = this;
        }

        private FsmMachine _fsmMachine;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            _fsmMachine = new FsmMachine();
            _fsmMachine.Initialize(InitState.name,this);
        }

        private void Update()
        {
            _fsmMachine.Update();
        }

        public static void Goto(string name)
        {
            Instance._fsmMachine.GoTo(name);
        }
    }
}