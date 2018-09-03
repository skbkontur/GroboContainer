using System;
using System.Reflection;
using System.Reflection.Emit;

namespace GroboContainer.Tests
{
    public class FieldsCleaner
    {
        public FieldsCleaner(Type type)
        {
            this.type = type;
            method = new DynamicMethod(Guid.NewGuid().ToString(), typeof(void), new[] {typeof(object)}, true);
            il = method.GetILGenerator();
            local = il.DeclareLocal(type);
            il.Emit(OpCodes.Ldarg_0);
            il.Emit(OpCodes.Castclass, type);
            il.Emit(OpCodes.Stloc, local);
            EmitCleanFileds();
            il.Emit(OpCodes.Ret);
        }

        public Action<object> GetDelegate()
        {
            return (Action<object>)method.CreateDelegate(typeof(Action<object>));
        }

        private void EmitRefTypeFieldClean(FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldloc, local);
            il.Emit(OpCodes.Ldnull);
            il.Emit(OpCodes.Stfld, fieldInfo);
        }

        private void EmitValueTypeFieldClean(FieldInfo fieldInfo)
        {
            il.Emit(OpCodes.Ldloc, local);
            il.Emit(OpCodes.Ldflda, fieldInfo);
            il.Emit(OpCodes.Initobj, fieldInfo.FieldType);
        }

        private void EmitCleanFileds()
        {
            for (var objType = type; objType != null as Type; objType = objType.BaseType)
                ClearObjectFileds(objType);
        }

        private void ClearObjectFileds(Type objType)
        {
            foreach (var field in objType.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.SetField))
            {
                if (!field.IsInitOnly && !field.IsLiteral)
                {
                    if (field.FieldType.IsValueType)
                        EmitValueTypeFieldClean(field);
                    else
                        EmitRefTypeFieldClean(field);
                }
            }
        }

        private readonly ILGenerator il;
        private readonly LocalBuilder local;
        private readonly DynamicMethod method;
        private readonly Type type;
    }
}