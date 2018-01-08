using NUnit.Framework;

namespace Tests.TypesHelperTests.GenericConstraints
{
    public class CreateProperConstrainedGenericTest : TypesHelperGenericTestBase
    {
        private interface IConstraint
        {
        }

        private class Entity1
        {
        }

        private class Entity2 : IConstraint
        {
        }

        private struct Entity3
        {
        }

        private class Entity4
        {
            private Entity4()
            {
            }
        }

        private interface IAbstract1<T1, T2>
        {
        }

        private interface IAbstract2<T>
        {
        }

        private class GenericClassWithTypeConstraint<T1, T2> : IAbstract1<T2, T1>
            where T2 : IConstraint
        {
        }

        private class GenericClassWithValueTypeConstraint<T> : IAbstract2<T> where T : struct
        {
        }

        private class GenericClassWithReferenceTypeConstraint<T> : IAbstract2<T> where T : class
        {
        }

        private class GenericClassWithDefaultConstructorConstraint<T> : IAbstract2<T> where T : new()
        {
        }

        private class ArrayGenericClass<T> : IAbstract2<GenericClassWithReferenceTypeConstraint<T>[]> where T : class
        {
        }

        private interface IGeneric<T, TConstraint>
            where T : TConstraint
        {
        }

        private class ConstrainedImpl<T, TConstraint> : IGeneric<T, TConstraint>
            where T : TConstraint
        {
        }

        private class Constraint
        {
        }

        private class ConstrainedEntity : Constraint
        {
        }

        private class ConstrainedEntity2 : ConstrainedEntity
        {
        }

        private interface IGenericFromSelf<T> where T : IGenericFromSelf<T>
        {
        }

        private class GenericFromSelf<T> : IGenericFromSelf<T> where T : GenericFromSelf<T>
        {
        }

        private class GenericArg : GenericFromSelf<GenericArg>
        {
        }

        private interface IGenericFromManyArguments<T1, T2, T3, T4>
            where T1 : T2
            where T2 : T3, T4
        {
        }

        private class GenericFromManyArguments<T1, T2, T3, T4> : IGenericFromManyArguments<T1, T2, T3, T4>
            where T1 : T2
            where T2 : T3, T4
        {
        }

        [Test]
        public void TestConstraintOnGenericArgument()
        {
            CheckTrue<IGeneric<ConstrainedEntity, Constraint>, ConstrainedImpl<ConstrainedEntity, Constraint>>(
                typeof(ConstrainedImpl<,>));
        }

        [Test]
        public void TestConstraintOnGenericFromSelf()
        {
            CheckTrue<IGenericFromSelf<GenericArg>, GenericFromSelf<GenericArg>>(typeof(GenericFromSelf<>));
        }

        [Test]
        public void TestDefaultConstructorConstraintFailed()
        {
            CheckFalse<IAbstract2<Entity4>>(typeof(GenericClassWithDefaultConstructorConstraint<>));
        }

        [Test]
        public void TestDefaultConstructorConstraintPassedOnReferenceType()
        {
            CheckTrue<IAbstract2<Entity1>, GenericClassWithDefaultConstructorConstraint<Entity1>>(
                typeof(GenericClassWithDefaultConstructorConstraint<>));
        }

        [Test]
        public void TestDefaultConstructorConstraintPassedOnValueType()
        {
            CheckTrue<IAbstract2<Entity3>, GenericClassWithDefaultConstructorConstraint<Entity3>>(
                typeof(GenericClassWithDefaultConstructorConstraint<>));
        }

        [Test]
        public void TestHardConstraints()
        {
            CheckTrue<IGeneric<GenericArg, GenericFromSelf<GenericArg>>,
                ConstrainedImpl<GenericArg, GenericFromSelf<GenericArg>>>(typeof(ConstrainedImpl<,>));
        }

        [Test]
        public void TestManyConstraintsOnGenericArguments()
        {
            CheckTrue<IGenericFromManyArguments<ConstrainedEntity2, ConstrainedEntity, Constraint, Constraint>,
                GenericFromManyArguments<ConstrainedEntity2, ConstrainedEntity, Constraint, Constraint>>(
                typeof(GenericFromManyArguments<,,,>));
        }

        [Test]
        public void TestReferenceTypeConstraintFailed()
        {
            CheckFalse<IAbstract2<Entity3>>(typeof(GenericClassWithReferenceTypeConstraint<>));
        }

        [Test]
        public void TestReferenceTypeConstraintPassed()
        {
            CheckTrue<IAbstract2<Entity1>, GenericClassWithReferenceTypeConstraint<Entity1>>(
                typeof(GenericClassWithReferenceTypeConstraint<>));
        }

        [Test]
        public void TestResolveTypeOfArrayElement()
        {
            CheckTrue<IAbstract2<GenericClassWithReferenceTypeConstraint<string>[]>, ArrayGenericClass<string>>(
                typeof(ArrayGenericClass<>));
        }

        [Test]
        public void TestTypeConstraintFailed()
        {
            CheckFalse<IAbstract1<Entity1, Entity2>>(typeof(GenericClassWithTypeConstraint<,>));
        }

        [Test]
        public void TestTypeConstraintPassed()
        {
            CheckTrue<IAbstract1<Entity2, Entity1>,
                GenericClassWithTypeConstraint<Entity1, Entity2>>(
                typeof(GenericClassWithTypeConstraint<,>));
        }

        [Test]
        public void TestValueTypeConstraintFailed()
        {
            CheckFalse<IAbstract2<Entity1>>(typeof(GenericClassWithValueTypeConstraint<>));
        }

        [Test]
        public void TestValueTypeConstraintPassed()
        {
            CheckTrue<IAbstract2<Entity3>, GenericClassWithValueTypeConstraint<Entity3>>(
                typeof(GenericClassWithValueTypeConstraint<>));
        }
    }
}