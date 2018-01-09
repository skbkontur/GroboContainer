using System;
using System.Collections;
using System.Reflection;
using System.Reflection.Emit;

using GrEmit;

namespace GroboContainer.Impl
{
    public static class HashtableExtensions
    {
        private static void EmitXPlusPlus(this GroboIL il, GroboIL.Local intLocal)
        {
            il.Ldloc(intLocal);
            il.Dup();
            il.Ldc_I4(1);
            il.Add();
            il.Stloc(intLocal);
        }

        private static void EmitMinusMinusX(this GroboIL il, GroboIL.Local intLocal)
        {
            il.Ldloc(intLocal);
            il.Ldc_I4(1);
            il.Sub();
            il.Dup();
            il.Stloc(intLocal);
        }

        private static void EmitLoadArrayItemRef(this GroboIL il, GroboIL.Local arrayLocal, GroboIL.Local indexLocal, Type elementType)
        {
            il.Ldloc(arrayLocal);
            il.Ldloc(indexLocal);
            il.Ldelema(elementType);
        }

        private static void EmitSetIntToZero(this GroboIL il, GroboIL.Local intLocal)
        {
            il.Ldc_I4(0);
            il.Stloc(intLocal);
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
                Guid.NewGuid().ToString(), typeof(void), new[] {typeof(Hashtable), typeof(Array), typeof(int)},
                typeof(Hashtable), true);

            var bucketsFieldInfo = typeof(Hashtable).GetField("buckets",
                                                              BindingFlags.Instance | BindingFlags.NonPublic);
            var bucketType = typeof(Hashtable).GetNestedType("bucket", BindingFlags.NonPublic);
            var bucketArrayType = bucketType.MakeArrayType();

            var keyFieldInfo = bucketType.GetField("key", BindingFlags.Instance | BindingFlags.Public);
            var valFieldInfo = bucketType.GetField("val", BindingFlags.Instance | BindingFlags.Public);
            var setValueMethodInfo = typeof(Array).GetMethod("SetValue", new[] {typeof(object), typeof(int)});

            using(var il = new GroboIL(dynamicMethod))
            {
                var buckets = il.DeclareLocal(bucketArrayType, "buckets");
                var length = il.DeclareLocal(typeof(int), "length");
                var arrayIndex = il.DeclareLocal(typeof(int), "arrayIndex");
                var key = il.DeclareLocal(typeof(object), "key");

                var cycleStartLabel = il.DefineLabel("cycleStart");
                var cycleNextLabel = il.DefineLabel("cycleNext");
                var endLabel = il.DefineLabel("end");

                il.EmitSetIntToZero(arrayIndex);

                il.Ldarg(0); // stack: hashtable
                il.Ldfld(bucketsFieldInfo); // stack: hashtable::buckets
                il.Stloc(buckets);

                il.Ldloc(buckets); // stack: hashtable::buckets
                il.Ldlen(); // stack: buckets.length 
                il.Conv<int>(); // stack: buckets.length (i4)
                il.Stloc(length);
                il.Br(cycleNextLabel); // jump(cycleNext)

                il.MarkLabel(cycleStartLabel);

                il.EmitLoadArrayItemRef(buckets, length, bucketType); // stack: *bucket[current]

                il.Ldfld(keyFieldInfo); // stack: key
                il.Stloc(key); // 2: key
                il.Ldloc(key); // stack: key
                il.Brfalse(cycleNextLabel); // jump(cycleNext) if key == null
                il.Ldloc(key); // stack: key
                il.Ldarg(0); // stack+: hashtable
                il.Ldfld(bucketsFieldInfo); // stack: key, hashtable::buckets
                il.Beq(cycleNextLabel);
                // jump(cycleNext) if key == hashtable::buckets (какой-то хитрый хак hashtable-а)

                il.Ldloc(arrayIndex); // stack: arrayIndex
                il.Ldarg(2); // stack+: arrayLength
                il.Bge(endLabel, false); // jump(end) if arrayIndex >= arrayLength

                il.Ldarg(1); // stack: array (arg1)

                il.EmitLoadArrayItemRef(buckets, length, bucketType); // stack: array (arg1), *bucket[current]
                il.Ldfld(valFieldInfo); // stack: array (arg1), bucket[current].val
                il.EmitXPlusPlus(arrayIndex); // stack: array (arg1), bucket[current].val, arrayIndex++

                il.Call(setValueMethodInfo); // array.SetValue(bucket[current].val, old_arrayIndex);

                il.MarkLabel(cycleNextLabel);

                il.EmitMinusMinusX(length); // stack: --current
                il.Ldc_I4(0); // stack+: 0
                il.Bge(cycleStartLabel, false); // jump(cycleStart) if --current >= 0

                il.MarkLabel(endLabel);
                il.Ret();
            }

            return (Action<Hashtable, Array, int>)dynamicMethod.CreateDelegate(typeof(Action<Hashtable, Array, int>));
        }

        public static T[] GetValues<T>(this Hashtable hashtable)
        {
            var result = new T[hashtable.Count];
            copyValuesDelegate(hashtable, result, result.Length);
            return result;
        }

        private static readonly Action<Hashtable, Array, int> copyValuesDelegate = EmitCopyValues();
    }
}