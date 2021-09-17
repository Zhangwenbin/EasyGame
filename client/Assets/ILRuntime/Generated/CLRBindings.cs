using System;
using System.Collections.Generic;
using System.Reflection;

namespace ILRuntime.Runtime.Generated
{
    class CLRBindings
    {

        internal static ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3> s_UnityEngine_Vector3_Binding_Binder = null;

        /// <summary>
        /// Initialize the CLR binding, please invoke this AFTER CLR Redirection registration
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            UnityEngine_Debug_Binding.Register(app);
            UnityEngine_Component_Binding.Register(app);
            UnityEngine_MonoBehaviour_Binding.Register(app);
            EG_MonoSingleton_1_ConfigManager_Binding.Register(app);
            MotionFramework_Config_ConfigManager_Binding.Register(app);
            System_Object_Binding.Register(app);
            MotionFramework_Config_AssetConfig_Binding.Register(app);
            CfgLanguageTable_Binding.Register(app);
            System_String_Binding.Register(app);
            System_Collections_Generic_List_1_MotionFramework_Config_ConfigManager_Binding_LoadPair_Binding.Register(app);
            System_Type_Binding.Register(app);
            System_Enum_Binding.Register(app);
            System_Array_Binding.Register(app);
            System_Collections_IEnumerator_Binding.Register(app);
            System_Exception_Binding.Register(app);
            MotionFramework_Config_ConfigManager_Binding_LoadPair_Binding.Register(app);
            System_IDisposable_Binding.Register(app);
            System_NotSupportedException_Binding.Register(app);
            System_Collections_Generic_List_1_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_ILTypeInstance_Binding.Register(app);
            System_Collections_Generic_Dictionary_2_String_Int32_Binding.Register(app);
            LitJson_JsonMapper_Binding.Register(app);
            System_Diagnostics_Stopwatch_Binding.Register(app);
            UnityEngine_Vector3_Binding.Register(app);
            System_Text_StringBuilder_Binding.Register(app);
            ValueTypeBindingDemo_Binding.Register(app);
            UnityEngine_Time_Binding.Register(app);
            System_Single_Binding.Register(app);
            UnityEngine_GameObject_Binding.Register(app);
            CoroutineDemo_Binding.Register(app);
            UnityEngine_WaitForSeconds_Binding.Register(app);
            CLRBindingTestClass_Binding.Register(app);
            System_Int32_Binding.Register(app);
            TestClassBase_Binding.Register(app);
            TestDelegateMethod_Binding.Register(app);
            TestDelegateFunction_Binding.Register(app);
            System_Action_1_String_Binding.Register(app);
            DelegateDemo_Binding.Register(app);
            System_Collections_Generic_List_1_Int32_Binding.Register(app);

            ILRuntime.CLR.TypeSystem.CLRType __clrType = null;
            __clrType = (ILRuntime.CLR.TypeSystem.CLRType)app.GetType (typeof(UnityEngine.Vector3));
            s_UnityEngine_Vector3_Binding_Binder = __clrType.ValueTypeBinder as ILRuntime.Runtime.Enviorment.ValueTypeBinder<UnityEngine.Vector3>;
        }

        /// <summary>
        /// Release the CLR binding, please invoke this BEFORE ILRuntime Appdomain destroy
        /// </summary>
        public static void Shutdown(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            s_UnityEngine_Vector3_Binding_Binder = null;
        }
    }
}
