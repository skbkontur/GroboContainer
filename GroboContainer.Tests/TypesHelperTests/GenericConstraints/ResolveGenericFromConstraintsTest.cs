using System;

using NUnit.Framework;

namespace GroboContainer.Tests.TypesHelperTests.GenericConstraints
{
    public class ResolveGenericFromConstraintsTest : TypesHelperGenericTestBase
    {
        // Добавить тест такой, чтобы T1 наследовался от нескольких интерфейсов, а может и ещё некоторые constraint-ы на него навесить. И сделать так, чтобы только один класс подошёл.
        public interface IEntity
        {
        }

        public class Entity : IEntity
        {
        }

        public interface IInterface
        {
        }

        public class Generic1<T> : IInterface
            where T : IEntity
        {
        }

        public interface I1
        {
        }

        public interface I2
        {
        }

        public class Entity1 : I1, I2
        {
            public Entity1(int x)
            {
            }
        }

        public struct Entity2 : I1, I2
        {
        }

        public class Entity3 : I1
        {
        }

        public class Entity4 : I2
        {
        }

        public class Entity5 : I1, I2
        {
        }

        public class Generic2<T> : IInterface
            where T : class, I1, I2, new()
        {
        }

        [Test]
        public void TestResolveGenericArgumentFromConstraint()
        {
            CheckTrue<IInterface, Generic1<Entity>>(
                typeof(Generic1<>),
                t =>
                {
                    if (t == typeof(IEntity))
                        return new[] {typeof(Entity)};
                    return new Type[0];
                });
        }

        [Test]
        public void TestResolveGenericArgumentFromManyConstraints()
        {
            CheckTrue<IInterface, Generic2<Entity5>>(typeof(Generic2<>), t =>
            {
                if (t == typeof(I1))
                    return new[] {typeof(Entity1), typeof(Entity2), typeof(Entity3), typeof(Entity5)};
                if (t == typeof(I2))
                    return new[] {typeof(Entity1), typeof(Entity2), typeof(Entity4), typeof(Entity5)};
                return new Type[0];
            });
        }
    }
}