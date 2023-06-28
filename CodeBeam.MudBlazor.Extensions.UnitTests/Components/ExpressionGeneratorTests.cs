using FluentAssertions;
using System.Linq.Expressions;
using System.Reflection;

namespace MudExtensions.UnitTests.Components
{
    [TestFixture]
    public class ExpressionGeneratorTests
    {
        private MethodInfo _containsMethod;
        private MethodInfo _startsWithMethod;
        private MethodInfo _endsWithMethod;
        private MethodInfo _trimMethod;

        private static readonly ParameterExpression _parameterExpression = Expression.Parameter(typeof(TestClass), "x");
        private static readonly MethodInfo ContainsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });



        [SetUp]
        public void Setup()
        {
            _containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            _startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
            _endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
            _trimMethod = typeof(string).GetMethod("Trim", Type.EmptyTypes);
        }


        [Test]
        public void GenerateStringFilterExpression_ContainsOperator_ReturnsExpectedExpression()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = "test",
                Operator = FilterOperator.String.Contains,
                Member = "TestString",
                MemberType = typeof(string)
            };

            var propertyExpression = Expression.Property(_parameterExpression, atomicPredicate.Member);

            // Act
            var result = ExpressionGenerator.GenerateStringFilterExpression(atomicPredicate, propertyExpression);

            // Assert
            var expectedExpression = Expression.AndAlso(
                Expression.NotEqual(propertyExpression, Expression.Constant(null)),
                Expression.Call(propertyExpression, ContainsMethod, Expression.Constant(atomicPredicate.Value)));

            result.Should().BeEquivalentTo(expectedExpression);
        }
        public enum TestEnum
        {
            Value1,
            Value2
        }

        public class TestClass
        {
            public string TestString { get; set; }
            public TestEnum TestEnum { get; set; }
        }

        [Test]
        public void GenerateEnumFilterExpression_IsOperatorAndValueNotNull_ReturnsExpectedExpression()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = TestEnum.Value1.ToString(), // Use enum value as string
                Operator = FilterOperator.Enum.Is,
                Member = "TestEnum",
                MemberType = typeof(TestEnum)
            };

            var propertyExpression = Expression.Property(_parameterExpression, atomicPredicate.Member);

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, propertyExpression);

            // Assert
            var expectedExpression = Expression.Equal(propertyExpression, Expression.Constant(TestEnum.Value1));

            result.Should().BeEquivalentTo(expectedExpression, options => options
                .ComparingByValue<TestEnum>() // Compare enum values by value
                .ComparingEnumsByName() // Compare enum types by name
            );
        }



        [Test]
        public void GenerateEnumFilterExpression_IsOperatorAndValueNull_ReturnsExpectedExpression()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = null,
                Operator = FilterOperator.Enum.Is,
                Member = nameof(TestClass.TestEnum),
                MemberType = Nullable.GetUnderlyingType(typeof(TestEnum?)) ?? typeof(TestEnum)
            };

            var propertyExpression = Expression.Property(_parameterExpression, atomicPredicate.Member);

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, propertyExpression);

            // Assert
            var expectedExpression = Expression.Equal(
                Expression.Convert(propertyExpression, typeof(TestEnum?)),
                Expression.Constant(null, typeof(TestEnum?)));

            result.Should().BeEquivalentTo(expectedExpression);
        }






        [Test]
        public void TestGenerateEnumFilterExpression_IsOperatorNonNullValue()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = "Value1",
                Operator = FilterOperator.Enum.Is,
                Member = nameof(TestClass.TestEnum),
                MemberType = typeof(TestEnum)
            };

            var testObjectTrue = new TestClass { TestEnum = TestEnum.Value1 };
            var testObjectFalse = new TestClass { TestEnum = TestEnum.Value2 };

            // Create an Expression to access the 'TestEnum' property of 'TestClass'
            var memberExpression = Expression.Property(_parameterExpression, atomicPredicate.Member);

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, memberExpression);
            var compiled = Expression.Lambda<Func<TestClass, bool>>(result, _parameterExpression).Compile();

            Assert.Multiple(() =>
            {

                // Assert
                Assert.That(compiled(testObjectTrue), Is.True);
                Assert.That(compiled(testObjectFalse), Is.False);
            });
        }

        [Test]
        public void TestGenerateEnumFilterExpression_IsOperatorNullValue()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = null,
                Operator = FilterOperator.Enum.Is,
                Member = nameof(TestClass.TestEnum),
                MemberType = typeof(TestEnum)
            };

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, _parameterExpression);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void TestGenerateEnumFilterExpression_IsOperatorNonNullValueNullableEnum()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = "Value1",
                Operator = FilterOperator.Enum.Is,
                Member = nameof(TestClass.TestEnum),
                MemberType = Nullable.GetUnderlyingType(typeof(TestEnum?)) ?? typeof(TestEnum)
            };

            var testObjectTrue = new TestClass { TestEnum = TestEnum.Value1 };
            var testObjectFalse = new TestClass { TestEnum = TestEnum.Value2 };

            // Create an Expression to access the 'TestEnum' property of 'TestClass'
            var memberExpression = Expression.Property(_parameterExpression, atomicPredicate.Member);

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, memberExpression);
            var compiled = Expression.Lambda<Func<TestClass, bool>>(result, _parameterExpression).Compile();
            Assert.Multiple(() =>
            {

                // Assert
                Assert.That(compiled(testObjectTrue), Is.True);
                Assert.That(compiled(testObjectFalse), Is.False);
            });
        }

        [Test]
        public void TestGenerateEnumFilterExpression_IsOperatorNullValueNullableEnum()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = null,
                Operator = FilterOperator.Enum.Is,
                Member = nameof(TestClass.TestEnum),
                MemberType = Nullable.GetUnderlyingType(typeof(TestEnum?)) ?? typeof(TestEnum)
            };

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, _parameterExpression);

            // Assert
            Assert.That(result, Is.Not.Null);

            // More comprehensive assertions
            Assert.That(result.NodeType, Is.EqualTo(ExpressionType.Constant));
            Assert.That(result.Type, Is.EqualTo(typeof(bool)));

            var constantExpression = result as ConstantExpression;

            Assert.That(constantExpression, Is.Not.Null);
            Assert.That(constantExpression.Value, Is.EqualTo(true));
        }



        [Test]
        public void TestGenerateEnumFilterExpression_IsNotOperatorNonNullValue()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = "Value1",
                Operator = FilterOperator.Enum.IsNot,
                Member = nameof(TestClass.TestEnum),
                MemberType = Nullable.GetUnderlyingType(typeof(TestEnum)) ?? typeof(TestEnum)
            };

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, _parameterExpression);

            // Assert
            Assert.That(result, Is.Not.Null);
        }


        [Test]
        public void TestGenerateEnumFilterExpression_IsNotOperatorNullValue()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = null,
                Operator = FilterOperator.Enum.IsNot,
                Member = "TestEnum",
                MemberType = typeof(TestEnum)
            };

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, _parameterExpression);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void TestGenerateEnumFilterExpression_IsNotOperatorNonNullValueNullableEnum()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = "Value1",
                Operator = FilterOperator.Enum.IsNot,
                Member = "TestEnum",
                MemberType = typeof(TestEnum?)
            };

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, _parameterExpression);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void TestGenerateEnumFilterExpression_IsNotOperatorNullValueNullableEnum()
        {
            // Arrange
            var atomicPredicate = new AtomicPredicate<TestClass>
            {
                Value = null,
                Operator = FilterOperator.Enum.IsNot,
                Member = "TestEnum",
                MemberType = typeof(TestEnum?)
            };

            // Act
            var result = ExpressionGenerator.GenerateEnumFilterExpression(atomicPredicate, _parameterExpression);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

    }
}