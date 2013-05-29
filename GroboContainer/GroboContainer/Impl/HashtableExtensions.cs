using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

namespace GroboContainer.Impl
{
    public static class HashtableExtensions
    {
        private static readonly Action<Hashtable, Array, int> copyValuesDelegate = EmitCopyValues();

        private static void EmitXPlusPlus(this ILGenerator il, LocalBuilder intLocalBuilder)
        {
            il.Emit(OpCodes.Ldloc_S, intLocalBuilder);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Add);
            il.Emit(OpCodes.Stloc_S, intLocalBuilder);
        }

        private static void EmitMinusMinusX(this ILGenerator il, LocalBuilder intLocalBuilder)
        {
            il.Emit(OpCodes.Ldloc_S, intLocalBuilder);
            il.Emit(OpCodes.Ldc_I4_1);
            il.Emit(OpCodes.Sub);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Stloc_S, intLocalBuilder);
        }

        private static void EmitLoadArrayItemRef(this ILGenerator il, LocalBuilder arrayLocalBuilder,
                                                 LocalBuilder indexLocalBuilder, Type elementType)
        {
            il.Emit(OpCodes.Ldloc_S, arrayLocalBuilder);
            il.Emit(OpCodes.Ldloc_S, indexLocalBuilder);
            il.Emit(OpCodes.Ldelema, elementType);
        }

        private static void EmitSetIntToZero(this ILGenerator il, LocalBuilder intLocalBuilder)
        {
            il.Emit(OpCodes.Ldc_I4_0);
            il.Emit(OpCodes.Stloc_S, intLocalBuilder);
        }

        private static Action<Hashtable, Array, int> EmitCopyValues()
        {
            /*
            private void CopyValues(Hashtable hashtable, Array array, int arrayLength)
            {
                bucket[] buckets = hashtable.buckets;
                int length = buckets.Length;
                int arrayIndex = 0;
                while (--length >= 0)
                {
                    object key = buckets[length].key;
                    if ((key != null) && (key != hashtable.buckets))
                    {
                        if (arrayIndex >= arrayLength)
                            return;   
                        array.SetValue(buckets[length].val, arrayIndex++);
                    }
                }
            }
            */
            var dynamicMethod = new DynamicMethod(
                Guid.NewGuid().ToString(), typeof (void), new[] {typeof (Hashtable), typeof (Array), typeof (int)},
                typeof (Hashtable), true);

            FieldInfo bucketsFieldInfo = typeof (Hashtable).GetField("buckets",
                                                                     BindingFlags.Instance | BindingFlags.NonPublic);
            Type bucketType = typeof (Hashtable).GetNestedType("bucket", BindingFlags.NonPublic);
            Type bucketArrayType = bucketType.MakeArrayType();

            FieldInfo keyFieldInfo = bucketType.GetField("key", BindingFlags.Instance | BindingFlags.Public);
            FieldInfo valFieldInfo = bucketType.GetField("val", BindingFlags.Instance | BindingFlags.Public);
            MethodInfo setValueMethodInfo = typeof (Array).GetMethod("SetValue", new[] {typeof (object), typeof (int)});

            ILGenerator il = dynamicMethod.GetILGenerator();
            LocalBuilder buckets = il.DeclareLocal(bucketArrayType);
            LocalBuilder length = il.DeclareLocal(typeof (int));
            LocalBuilder arrayIndex = il.DeclareLocal(typeof (int));
            LocalBuilder key = il.DeclareLocal(typeof (object));

            Label cycleStart = il.DefineLabel();
            Label cycleNext = il.DefineLabel();
            Label end = il.DefineLabel();

            il.EmitSetIntToZero(arrayIndex);

            il.Emit(OpCodes.Ldarg_0); // stack: hashtable
            il.Emit(OpCodes.Ldfld, bucketsFieldInfo); // stack: hashtable::buckets
            il.Emit(OpCodes.Stloc_S, buckets);

            il.Emit(OpCodes.Ldloc_S, buckets); // stack: hashtable::buckets
            il.Emit(OpCodes.Ldlen); // stack: buckets.length 
            il.Emit(OpCodes.Conv_I4); // stack: buckets.length (i4)
            il.Emit(OpCodes.Stloc_S, length);
            il.Emit(OpCodes.Br_S, cycleNext); // jump(cycleNext)

            il.MarkLabel(cycleStart);

            il.EmitLoadArrayItemRef(buckets, length, bucketType); // stack: *bucket[current]

            il.Emit(OpCodes.Ldfld, keyFieldInfo); // stack: key
            il.Emit(OpCodes.Stloc_S, key); // 2: key
            il.Emit(OpCodes.Ldloc_S, key); // stack: key
            il.Emit(OpCodes.Brfalse_S, cycleNext); // jump(cycleNext) if key == null
            il.Emit(OpCodes.Ldloc_S, key); // stack: key
            il.Emit(OpCodes.Ldarg_0); // stack+: hashtable
            il.Emit(OpCodes.Ldfld, bucketsFieldInfo); // stack: key, hashtable::buckets
            il.Emit(OpCodes.Beq_S, cycleNext);
            // jump(cycleNext) if key == hashtable::buckets (какой-то хитрый хак hashtable-а)

            il.Emit(OpCodes.Ldloc_S, arrayIndex); // stack: arrayIndex
            il.Emit(OpCodes.Ldarg_2); // stack+: arrayLength
            il.Emit(OpCodes.Bge_S, end); // jump(end) if arrayIndex >= arrayLength

            il.Emit(OpCodes.Ldarg_1); // stack: array (arg1)

            il.EmitLoadArrayItemRef(buckets, length, bucketType); // stack: array (arg1), *bucket[current]
            il.Emit(OpCodes.Ldfld, valFieldInfo); // stack: array (arg1), bucket[current].val
            il.EmitXPlusPlus(arrayIndex); // stack: array (arg1), bucket[current].val, arrayIndex++

            il.Emit(OpCodes.Callvirt, setValueMethodInfo); // array.SetValue(bucket[current].val, old_arrayIndex);

            il.MarkLabel(cycleNext);

            il.EmitMinusMinusX(length); // stack: --current
            il.Emit(OpCodes.Ldc_I4_0); // stack+: 0
            il.Emit(OpCodes.Bge_S, cycleStart); // jump(cycleStart) if --current >= 0

            il.MarkLabel(end);
            il.Emit(OpCodes.Ret);

            return (Action<Hashtable, Array, int>) dynamicMethod.CreateDelegate(typeof (Action<Hashtable, Array, int>));
        }

        public static T[] GetValues<T>(this Hashtable hashtable)
        {
            var result = new T[hashtable.Count];
            copyValuesDelegate(hashtable, result, result.Length);
            return result;
        }
    }
}