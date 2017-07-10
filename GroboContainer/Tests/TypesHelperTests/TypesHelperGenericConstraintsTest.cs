using NUnit.Framework;

namespace Tests.TypesHelperTests
{
    public class TypesHelperGenericConstraintsTest : TypesHelperGenericTestBase
    {
        private interface IConstraint
        {
        }

        private class ConcreteEntity1
        {
        }

        private class ConcreteEntity2 : IConstraint
        {
        }

        private struct ConcreteEntity3
        {
        }

        private class ConcreteEntity4
        {
            private ConcreteEntity4()
            {
            }
        }

        private interface IAbstract1<T1, T2>
        {
        }

        private interface IAbstract2<T>
        {
        }

        private class GenericClassWithTypeConstraint<T1, T2> : IAbstract1<T2, T1> where T2 : IConstraint
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

        public interface IGeneric<T, TConstraint>
            where T : TConstraint
        {
        }

        public class ConstrainedImpl<T, TConstraint> : IGeneric<T, TConstraint>
            where T : TConstraint
        { }

        public class Constraint { }

        public class ConstrainedEntity : Constraint { }

        public class ConstrainedEntity2 : ConstrainedEntity { }

        public interface IGenericFromSelf<T> where T : IGenericFromSelf<T>
        {
        }

        public class GenericFromSelf<T> : IGenericFromSelf<T> where T : GenericFromSelf<T>
        {
        }

        public class GenericArg : GenericFromSelf<GenericArg>
        {
        }

        public interface IGenericFromManyArguments<T1, T2, T3, T4> 
            where T1 : T2 
            where T2 : T3, T4
        { }

        public class GenericFromManyArguments<T1, T2, T3, T4> : IGenericFromManyArguments<T1, T2, T3, T4>
            where T1 : T2 
            where T2 : T3, T4
        { }

        [Test]
        public void TestConstraintOnGenericArgument()
        {
            CheckTrue<IGeneric<ConstrainedEntity, Constraint>, ConstrainedImpl<ConstrainedEntity, Constraint>>(
                typeof(ConstrainedImpl<,>));
        }

        [Test]
        public void TestManyConstraintsOnGenericArguments()
        {
            CheckTrue<IGenericFromManyArguments<ConstrainedEntity2, ConstrainedEntity, Constraint, Constraint>,
                GenericFromManyArguments<ConstrainedEntity2, ConstrainedEntity, Constraint, Constraint>>(
                typeof(GenericFromManyArguments<,,,>));
        }

        [Test]
        public void TestConstraintOnGenericFromSelf()
        {
            CheckTrue<IGenericFromSelf<GenericArg>, GenericFromSelf<GenericArg>>(typeof(GenericFromSelf<>));
        }

        [Test]
        public void TestHardConstraints()
        {
            CheckTrue<IGeneric<GenericArg, GenericFromSelf<GenericArg>>, ConstrainedImpl<GenericArg, GenericFromSelf<GenericArg>>>(typeof(ConstrainedImpl<,>));
        }

        [Test]
        public void TestDefaultConstructorConstraintFailed()
        {
            CheckFalse<IAbstract2<ConcreteEntity4>>(typeof(GenericClassWithDefaultConstructorConstraint<>));
        }

        [Test]
        public void TestDefaultConstructorConstraintPassedOnReferenceType()
        {
            CheckTrue<IAbstract2<ConcreteEntity1>, GenericClassWithDefaultConstructorConstraint<ConcreteEntity1>>(
                typeof(GenericClassWithDefaultConstructorConstraint<>));
        }

        [Test]
        public void TestDefaultConstructorConstraintPassedOnValueType()
        {
            CheckTrue<IAbstract2<ConcreteEntity3>, GenericClassWithDefaultConstructorConstraint<ConcreteEntity3>>(
                typeof(GenericClassWithDefaultConstructorConstraint<>));
        }

        [Test]
        public void TestReferenceTypeConstraintFailed()
        {
            CheckFalse<IAbstract2<ConcreteEntity3>>(typeof(GenericClassWithReferenceTypeConstraint<>));
        }

        [Test]
        public void TestReferenceTypeConstraintPassed()
        {
            CheckTrue<IAbstract2<ConcreteEntity1>, GenericClassWithReferenceTypeConstraint<ConcreteEntity1>>(
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
            CheckFalse<IAbstract1<ConcreteEntity1, ConcreteEntity2>>(typeof(GenericClassWithTypeConstraint<,>));
        }

        [Test]
        public void TestTypeConstraintPassed()
        {
            CheckTrue<IAbstract1<ConcreteEntity2, ConcreteEntity1>,
                GenericClassWithTypeConstraint<ConcreteEntity1, ConcreteEntity2>>(
                typeof(GenericClassWithTypeConstraint<,>));
        }

        [Test]
        public void TestValueTypeConstraintFailed()
        {
            CheckFalse<IAbstract2<ConcreteEntity1>>(typeof(GenericClassWithValueTypeConstraint<>));
        }

        [Test]
        public void TestValueTypeConstraintPassed()
        {
            CheckTrue<IAbstract2<ConcreteEntity3>, GenericClassWithValueTypeConstraint<ConcreteEntity3>>(
                typeof(GenericClassWithValueTypeConstraint<>));
        }
    }
}