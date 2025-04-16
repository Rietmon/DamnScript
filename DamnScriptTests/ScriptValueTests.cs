using DamnScript.Runtimes.Cores.Pins;
using DamnScript.Runtimes.VirtualMachines.ScriptValues;

namespace DamnScriptTests
{
	using SV = ScriptValue;

// ReSharper disable EqualExpressionComparison
	public class ScriptValueTests
	{
		[Test]
		public void DiffTypesWithDifferentValuesTest()
		{
			SV l = 1;
			SV r = 2f;

			Assert.That(r, Is.Not.EqualTo(l));
		}

		[Test]
		public void SameTypesWithSameValuesTest()
		{
			SV l = 1;
			SV r = 1;

			Assert.That(r, Is.EqualTo(l));
		}

		[Test]
		public void SameTypesWithDifferentValuesTest()
		{
			SV l = 1;
			SV r = 2;

			Assert.That(r, Is.Not.EqualTo(l));
		}

		[Test]
		public void FloatAndIntWithSameValuesTest()
		{
			SV l = 1f;
			SV r = 1;

			Assert.That(r, Is.EqualTo(l));
		}

		[Test]
		public void FloatAndIntWithDifferentValuesTest()
		{
			SV l = 1f;
			SV r = 2;

			Assert.That(r, Is.Not.EqualTo(l));
		}

		[Test]
		public void FloatAndFloatGreaterThanTest()
		{
			SV l = 1f;
			SV r = 2f;

			Assert.That(l < r, Is.True);
		}

		[Test]
		public void IntAndFloatLessThanTest()
		{
			SV l = 1;
			SV r = 2f;

			Assert.That(l < r, Is.True);
		}

		[Test]
		public void IntAndFloatGreaterThanTest()
		{
			SV l = 2;
			SV r = 1f;

			Assert.That(l > r, Is.True);
		}

		[Test]
		public void IntAndIntEqualTest()
		{
			SV l = 2;
			SV r = 2;

			Assert.That(l == r, Is.True);
		}

		[Test]
		public void FloatAndFloatEqualTest()
		{
			SV l = 2f;
			SV r = 2f;

			Assert.That(l == r, Is.True);
		}

		[Test]
		public void IntAndFloatNotEqualTest()
		{
			SV l = 2;
			SV r = 3f;

			Assert.That(l != r, Is.True);
		}

		[Test]
		public void FloatAndFloatNotEqualTest()
		{
			SV l = 2f;
			SV r = 3f;

			Assert.That(l != r, Is.True);
		}

		[Test]
		public unsafe void PointerAndPointerEqualTest()
		{
			var intSame = 1;
			SV l = &intSame;
			SV r = &intSame;

			Assert.That(l == r, Is.True);
		}

		[Test]
		public unsafe void PointerAndPointerNotEqualTest()
		{
			var intSame = 1;
			var intDifferent = 2;
			SV l = &intSame;
			SV r = &intDifferent;

			Assert.That(l != r, Is.True);
		}

		[Test]
		public void IntAndDoubleWithSameValuesTest()
		{
			SV l = 1;
			SV r = 1.0;

			Assert.That(r, Is.EqualTo(l));
		}

		[Test]
		public void IntAndDoubleWithDifferentValuesTest()
		{
			SV l = 1;
			SV r = 2.0;

			Assert.That(r, Is.Not.EqualTo(l));
		}

		[Test]
		public void IntAndDoubleGreaterThanTest()
		{
			SV l = 2;
			SV r = 1.0;

			Assert.That(l > r, Is.True);
		}

		[Test]
		public void IntAndDoubleLessThanTest()
		{
			SV l = 1;
			SV r = 2.0;

			Assert.That(l < r, Is.True);
		}

		[Test]
		public void IntAndDoubleEqualTest()
		{
			SV l = 2;
			SV r = 2.0;

			Assert.That(l == r, Is.True);
		}

		[Test]
		public void IntAndDoubleNotEqualTest()
		{
			SV l = 2;
			SV r = 3.0;

			Assert.That(l != r, Is.True);
		}

		[Test]
		public void DoubleAndFloatWithSameValuesTest()
		{
			SV l = 1.0;
			SV r = 1f;

			Assert.That(r, Is.EqualTo(l));
		}

		[Test]
		public void DoubleAndFloatWithDifferentValuesTest()
		{
			SV l = 1.0;
			SV r = 2f;

			Assert.That(r, Is.Not.EqualTo(l));
		}

		[Test]
		public void DoubleAndFloatGreaterThanTest()
		{
			SV l = 2.0;
			SV r = 1f;

			Assert.That(l > r, Is.True);
		}

		[Test]
		public void DoubleAndFloatLessThanTest()
		{
			SV l = 1.0;
			SV r = 2f;

			Assert.That(l < r, Is.True);
		}

		[Test]
		public void DoubleAndFloatEqualTest()
		{
			SV l = 2.0;
			SV r = 2f;

			Assert.That(l == r, Is.True);
		}

		[Test]
		public void DoubleAndFloatNotEqualTest()
		{
			SV l = 2.0;
			SV r = 3f;

			Assert.That(l != r, Is.True);
		}

		[Test]
		public void DoubleAndDoubleEqualTest()
		{
			SV l = 2.0;
			SV r = 2.0;

			Assert.That(l == r, Is.True);
		}

		[Test]
		public void DoubleAndDoubleNotEqualTest()
		{
			SV l = 2.0;
			SV r = 3.0;

			Assert.That(l != r, Is.True);
		}

		[Test]
		public void IntAndIntBitwiseAndTest()
		{
			SV l = 3;
			SV r = 5;

			Assert.That((l & r).longValue, Is.EqualTo(1));
		}

		[Test]
		public void IntAndIntBitwiseOrTest()
		{
			SV l = 3;
			SV r = 5;

			Assert.That((l | r).longValue, Is.EqualTo(7));
		}

		[Test]
		public void IntAndIntBitwiseXorTest()
		{
			SV l = 3;
			SV r = 5;

			Assert.That((l ^ r).longValue, Is.EqualTo(6));
		}

		[Test]
		public void IntBitwiseNotTest()
		{
			SV value = 5;

			Assert.That((~value).longValue, Is.EqualTo(~5L));
		}

		[Test]
		public void IntBitwiseLeftShiftTest()
		{
			SV l = 5;
			int shift = 2;

			Assert.That((l << shift).longValue, Is.EqualTo(20));
		}

		[Test]
		public void IntBitwiseRightShiftTest()
		{
			SV l = 8;
			int shift = 2;

			Assert.That((l >> shift).longValue, Is.EqualTo(2));
		}

		[Test]
		public void IntIncrementTest()
		{
			SV value = 5;
			SV result = ++value;

			Assert.That(result.longValue, Is.EqualTo(6));
		}

		[Test]
		public void FloatIncrementTest()
		{
			SV value = 5.5f;
			SV result = ++value;

			Assert.That(result.floatValue, Is.EqualTo(6.5f));
		}

		[Test]
		public void DoubleIncrementTest()
		{
			SV value = 5.5;
			SV result = ++value;

			Assert.That(result.doubleValue, Is.EqualTo(6.5));
		}

		[Test]
		public void IntDecrementTest()
		{
			SV value = 5;
			SV result = --value;

			Assert.That(result.longValue, Is.EqualTo(4));
		}

		[Test]
		public void FloatDecrementTest()
		{
			SV value = 5.5f;
			SV result = --value;

			Assert.That(result.floatValue, Is.EqualTo(4.5f));
		}

		[Test]
		public void DoubleDecrementTest()
		{
			SV value = 5.5;
			SV result = --value;

			Assert.That(result.doubleValue, Is.EqualTo(4.5));
		}

		[Test]
		public void IntAndIntModuloTest()
		{
			SV l = 7;
			SV r = 3;

			Assert.That((l % r).longValue, Is.EqualTo(1));
		}

		[Test]
		public void FloatAndFloatModuloTest()
		{
			SV l = 7.5f;
			SV r = 2.5f;

			Assert.That((l % r).floatValue, Is.EqualTo(0f));
		}

		[Test]
		public void DoubleAndDoubleModuloTest()
		{
			SV l = 7.5;
			SV r = 2.5;

			Assert.That((l % r).doubleValue, Is.EqualTo(0.0));
		}

		[Test]
		public void IntAndFloatModuloTest()
		{
			SV l = 7;
			SV r = 2.5f;

			Assert.That((l % r).doubleValue, Is.EqualTo(2.0));
		}

		[Test]
		public void DoubleAndIntAdditionTest()
		{
			SV l = 3.5;
			SV r = 2;

			Assert.That((l + r).doubleValue, Is.EqualTo(5.5));
		}

		[Test]
		public void DoubleAndIntSubtractionTest()
		{
			SV l = 5.5;
			SV r = 2;

			Assert.That((l - r).doubleValue, Is.EqualTo(3.5));
		}

		[Test]
		public void FloatAndIntMultiplicationTest()
		{
			SV l = 2.5f;
			SV r = 2;

			Assert.That((l * r).doubleValue, Is.EqualTo(5.0));
		}

		[Test]
		public void DoubleAndIntDivisionTest()
		{
			SV l = 10.0;
			SV r = 2;

			Assert.That((l / r).doubleValue, Is.EqualTo(5.0));
		}

		[Test]
		public void SafeNumberTypeConversionTest()
		{
			SV intValue = 42;
			SV floatValue = 42.5f;
			SV doubleValue = 42.75;

			// SafeFloatValue tests
			Assert.That(intValue.SafeFloatValue, Is.EqualTo(42f));
			Assert.That(floatValue.SafeFloatValue, Is.EqualTo(42.5f));
			Assert.That(doubleValue.SafeFloatValue, Is.EqualTo(42.75f));

			// SafeDoubleValue tests
			Assert.That(intValue.SafeDoubleValue, Is.EqualTo(42.0));
			Assert.That(floatValue.SafeDoubleValue, Is.EqualTo(42.5));
			Assert.That(doubleValue.SafeDoubleValue, Is.EqualTo(42.75));

			// SafeIntegerValue tests
			Assert.That(intValue.SafeIntegerValue, Is.EqualTo(42L));
			Assert.That(floatValue.SafeIntegerValue, Is.EqualTo(42L));
			Assert.That(doubleValue.SafeIntegerValue, Is.EqualTo(42L));
		}

		[Test]
		public void StructHandlingTest()
		{
			var testStruct = new TestStruct { x = 10, y = 20 };
			var value = SV.FromStructAlloc(testStruct);

			Assert.That(value.type, Is.EqualTo(SV.ValueType.Pointer));

			var retrievedStruct = value.GetStruct<TestStruct>();
			Assert.That(retrievedStruct.x, Is.EqualTo(10));
			Assert.That(retrievedStruct.y, Is.EqualTo(20));
		}

		[Test]
		public void ReferencePinningTest()
		{
			var testObject = new TestClass { Value = "Test" };
			var value = SV.FromReferencePin(testObject);

			Assert.That(value.type, Is.EqualTo(SV.ValueType.ReferenceSafePointer));

			var retrievedObject = value.GetReference<TestClass>();
			Assert.That(retrievedObject.Value, Is.EqualTo("Test"));
		
			value.UnpinManagedPointer();
		}

		[Test]
		public void ReferenceUnsafeTest()
		{
			var testObject = new TestClass { Value = "Test" };
			var value = SV.FromReferenceUnsafe(testObject);

			Assert.That(value.type, Is.EqualTo(SV.ValueType.ReferenceUnsafePointer));

			var retrievedObject = value.GetReferenceUnsafe<TestClass>();
			Assert.That(retrievedObject.Value, Is.EqualTo("Test"));
		}

		[Test]
		public void NegativeOperatorTest()
		{
			SV intValue = 42;
			SV floatValue = 42.5f;
			SV doubleValue = 42.75;

			Assert.That((-intValue).longValue, Is.EqualTo(-42L));
			Assert.That((-floatValue).floatValue, Is.EqualTo(-42.5f));
			Assert.That((-doubleValue).doubleValue, Is.EqualTo(-42.75));
		}

		[Test]
		public void PositiveOperatorTest()
		{
			SV intValue = -42;
			SV floatValue = -42.5f;
			SV doubleValue = -42.75;

			// The + operator as a unary operator doesn't negate, it just returns the value
			Assert.That((+intValue).longValue, Is.EqualTo(-42L));
			Assert.That((+floatValue).floatValue, Is.EqualTo(-42.5f));
			Assert.That((+doubleValue).doubleValue, Is.EqualTo(-42.75));
		}

		[Test]
		public unsafe void GetReferencePointerTest()
		{
			var testObject = new TestClass { Value = "Test" };

			// Test with unsafe reference
			var unsafeValue = SV.FromReferenceUnsafe(testObject);
			var unsafePointer = (IntPtr)unsafeValue.GetReferencePointer();
			Assert.That(unsafePointer, Is.Not.EqualTo(IntPtr.Zero));

			// Test with safe reference
			var safeValue = SV.FromReferencePin(testObject);
			var safePointer = (IntPtr)safeValue.GetReferencePointer();
			Assert.That(safePointer, Is.Not.EqualTo(IntPtr.Zero));
			safeValue.UnpinManagedPointer();
		}

		[Test]
		public unsafe void ScriptValuePtrTest()
		{
			SV value = 42;
			SV* valuePtr = UnsafeUtilities.AsPointer(ref value);
			var scriptValuePtr = new ScriptValuePtr(valuePtr);

			Assert.That(scriptValuePtr.Type, Is.EqualTo(SV.ValueType.Integer));
			Assert.That(scriptValuePtr.IntValue, Is.EqualTo(42));
		}

		[Test]
		public void ImplicitConversionTest()
		{
			const bool boolValue = true;
			const byte byteValue = 255;
			const sbyte sbyteValue = -128;
			const short shortValue = 32767;
			const ushort ushortValue = 65535;
			const int intValue = 2147483647;
			const uint uintValue = 4294967295;
			const long longValue = 9223372036854775807;
			const ulong ulongValue = 18446744073709551615;
			const float floatValue = 3.14159f;
			const double doubleValue = 3.14159265359;
			const char charValue = 'A';

			SV svBool = boolValue;
			SV svByte = byteValue;
			SV svSByte = sbyteValue;
			SV svShort = shortValue;
			SV svUShort = ushortValue;
			SV svInt = intValue;
			SV svUInt = uintValue;
			SV svLong = longValue;
			SV svULong = ulongValue;
			SV svFloat = floatValue;
			SV svDouble = doubleValue;
			SV svChar = charValue;

			Assert.That(svBool.boolValue, Is.EqualTo(boolValue));
			Assert.That(svByte.byteValue, Is.EqualTo(byteValue));
			Assert.That(svSByte.sbyteValue, Is.EqualTo(sbyteValue));
			Assert.That(svShort.shortValue, Is.EqualTo(shortValue));
			Assert.That(svUShort.ushortValue, Is.EqualTo(ushortValue));
			Assert.That(svInt.intValue, Is.EqualTo(intValue));
			Assert.That(svUInt.uintValue, Is.EqualTo(uintValue));
			Assert.That(svLong.longValue, Is.EqualTo(longValue));
			Assert.That(svULong.ulongValue, Is.EqualTo(ulongValue));
			Assert.That(svFloat.floatValue, Is.EqualTo(floatValue));
			Assert.That(svDouble.doubleValue, Is.EqualTo(doubleValue));
			Assert.That(svChar.charValue, Is.EqualTo(charValue));
		}

		[Test]
		public void ToStringTest()
		{
			SV intValue = 42;
			SV floatValue = 3.14159f;
			SV doubleValue = 3.14159265359;

			Assert.That(intValue.ToString(), Is.EqualTo("42"));
			Assert.That(floatValue.ToString(), Is.EqualTo("3.14159"));
			Assert.That(doubleValue.ToString(), Is.EqualTo("3.14159265359"));
		}

		[Test]
		public void GreaterOrEqualTest()
		{
			SV l1 = 5;
			SV r1 = 5;
			Assert.That(l1 >= r1, Is.True);

			SV l2 = 6;
			SV r2 = 5;
			Assert.That(l2 >= r2, Is.True);

			SV l3 = 5.0;
			SV r3 = 5;
			Assert.That(l3 >= r3, Is.True);

			SV l4 = 4;
			SV r4 = 5;
			Assert.That(l4 >= r4, Is.False);
		}

		[Test]
		public void LessOrEqualTest()
		{
			SV l1 = 5;
			SV r1 = 5;
			Assert.That(l1 <= r1, Is.True);

			SV l2 = 4;
			SV r2 = 5;
			Assert.That(l2 <= r2, Is.True);

			SV l3 = 5.0;
			SV r3 = 5;
			Assert.That(l3 <= r3, Is.True);

			SV l4 = 6;
			SV r4 = 5;
			Assert.That(l4 <= r4, Is.False);
		}

		private struct TestStruct
		{
			public int x;
			public int y;
		}

		private class TestClass
		{
			public string? Value { get; init; }
		}

		[Test]
		public void ExceptionOnInvalidBitwiseOperation()
		{
			SV floatValue = 1.5f;
			SV intValue = 2;

			Assert.Throws<Exception>(() => { _ = floatValue & intValue; });
			Assert.Throws<Exception>(() => { _ = floatValue | intValue; });
			Assert.Throws<Exception>(() => { _ = floatValue ^ intValue; });
			Assert.Throws<Exception>(() => { _ = ~floatValue; });
		}

		[Test]
		public void ExceptionOnInvalidShiftOperation()
		{
			const int shift = 2;
			SV floatValue = 1.5f;

			Assert.Throws<Exception>(() => { _ = floatValue << shift; });
			Assert.Throws<Exception>(() => { _ = floatValue >> shift; });
		}

		[Test]
		public void HashCodeConsistencyTest()
		{
			SV value1 = 42;
			SV value2 = 42;
			SV value3 = 43;

			Assert.That(value1.GetHashCode(), Is.EqualTo(value2.GetHashCode()));
			Assert.That(value1.GetHashCode(), Is.Not.EqualTo(value3.GetHashCode()));
		}

		[Test]
		public void ComplexOperationChainingTest()
		{
			SV a = 5;
			SV b = 2;
			SV c = 3;

			SV result = a + b * c - a / b;

			Assert.That(result.longValue, Is.EqualTo(9));
		}

		[Test]
		public void NumericLimitsTest()
		{
			SV maxInt = int.MaxValue;
			SV minInt = int.MinValue;
			SV maxLong = long.MaxValue;

			Assert.That((maxInt + 1).longValue, Is.EqualTo((long)int.MaxValue + 1));
			Assert.That((minInt - 1).longValue, Is.EqualTo((long)int.MinValue - 1));

			Assert.That((maxLong * 2).longValue, Is.EqualTo(unchecked(long.MaxValue * 2)));
		}

		[Test]
		public void ScriptValuePtrReferenceTest()
		{
			var testObject = new TestClass { Value = "RefTest" };
			var value = SV.FromReferencePin(testObject);
			var valuePtr = new ScriptValuePtr(ref value);

			Assert.That(valuePtr.Type, Is.EqualTo(SV.ValueType.ReferenceSafePointer));
			var retrievedObject = valuePtr.GetReference<TestClass>();
			Assert.That(retrievedObject.Value, Is.EqualTo("RefTest"));
		
			value.UnpinManagedPointer();
		}

		[Test]
		public void ConversionBetweenNumericTypesTest()
		{
			SV intValue = 10;
			SV floatValue = 10.5f;
			SV doubleValue = 10.75;

			SV intToFloat = intValue + 0.5f;
			SV floatToDouble = floatValue + 0.25;
			SV doubleToInt = doubleValue * 0;

			Assert.That(intToFloat.type, Is.EqualTo(SV.ValueType.Float64));
			Assert.That(intToFloat.doubleValue, Is.EqualTo(10.5));

			Assert.That(floatToDouble.type, Is.EqualTo(SV.ValueType.Float64));
			Assert.That(floatToDouble.doubleValue, Is.EqualTo(10.75));

			Assert.That(doubleToInt.type, Is.EqualTo(SV.ValueType.Float64));
			Assert.That(doubleToInt.doubleValue, Is.EqualTo(0.0));
		}

		[Test]
		public void ComparisonEdgeCasesTest()
		{
			SV nanValue = double.NaN;
			SV infValue = double.PositiveInfinity;
			SV negInfValue = double.NegativeInfinity;
			SV zeroValue = 0.0;

			Assert.That(nanValue != nanValue, Is.True, "NaN != NaN should be true");
			Assert.That(nanValue == nanValue, Is.False, "NaN == NaN should be false");

			Assert.That(infValue > zeroValue, Is.True);
			Assert.That(negInfValue < zeroValue, Is.True);
			Assert.That(infValue > negInfValue, Is.True);
		}

		[Test]
		public void GetStructWithoutFreeTest()
		{
			var testStruct = new TestStruct { x = 15, y = 25 };
			var value = SV.FromStructAlloc(testStruct);

			var retrievedStruct1 = value.GetStruct<TestStruct>();
			Assert.That(retrievedStruct1.x, Is.EqualTo(15));

			var retrievedStruct2 = value.GetStruct<TestStruct>();
			Assert.That(retrievedStruct2.y, Is.EqualTo(25));
		
			value.FreeUnmanagedPointer();
		}

		[Test]
		public void MultipleOperationsChainTest()
		{
			SV a = 10;
			SV b = 5;
			SV c = 2;
			SV d = 20;

			SV result = ((a + b) * c) - (d / b);

			Assert.That(result.longValue, Is.EqualTo(26));
		}

		[Test]
		public void StringHandlingTest()
		{
			var testString = "Test String";
			var value = SV.FromReferencePin(testString);

			Assert.That(value.type, Is.EqualTo(SV.ValueType.ReferenceSafePointer));

			var retrievedString = value.GetStringWrapper().ToString();
			Assert.That(retrievedString, Is.EqualTo(testString));
		
			value.UnpinManagedPointer();
		}

		[Test]
		public void NestedStructTest()
		{
			var outer = new OuterStruct
			{
				inner = new InnerStruct { x = 10, y = 20 },
				value = 30
			};

			var value = SV.FromStructAlloc(outer);
			var retrieved = value.GetStruct<OuterStruct>();

			Assert.That(retrieved.inner.x, Is.EqualTo(10));
			Assert.That(retrieved.inner.y, Is.EqualTo(20));
			Assert.That(retrieved.value, Is.EqualTo(30));
		}

		[Test]
		public void ScriptValuePtrOperationsTest()
		{
			SV value = 42;
			var valuePtr = new ScriptValuePtr(ref value);

			valuePtr.RefValue.longValue = 84;

			Assert.That(value.longValue, Is.EqualTo(84));
		}

		[Test]
		public void TypePromotionTest()
		{
			SV intValue = 10;
			Assert.That(intValue.type, Is.EqualTo(SV.ValueType.Integer));

			SV floatResult = intValue + 10.5f;
			Assert.That(floatResult.type, Is.EqualTo(SV.ValueType.Float64));

			SV floatValue = 10.5f;
			Assert.That(floatValue.type, Is.EqualTo(SV.ValueType.Float32));

			SV doubleResult = floatValue + 10.5;
			Assert.That(doubleResult.type, Is.EqualTo(SV.ValueType.Float64));
		}

		[Test]
		public void RoundTripConversionTest()
		{
			SV originalInt = 42;
			SV floatValue = originalInt + 0.0f;
			SV backToInt = new SV((long)floatValue.SafeDoubleValue);

			Assert.That(backToInt.longValue, Is.EqualTo(originalInt.longValue));

			SV originalFloat = 42.5f;
			SV doubleValue = originalFloat + 0.0;
			SV backToFloat = new SV((float)doubleValue.doubleValue);

			Assert.That(backToFloat.floatValue, Is.EqualTo(originalFloat.floatValue));
		}

		[Test]
		public void ScientificNotationNumberTest()
		{
			SV value = 1e6;
			Assert.That(value.doubleValue, Is.EqualTo(1000000.0));

			SV floatValue = 1.5e-2f;
			Assert.That(floatValue.floatValue, Is.EqualTo(0.015f));
		}

		[Test]
		public void ExtremeValueComparisonTest()
		{
			SV smallestPositive = double.Epsilon;
			SV zero = 0.0;
			SV largestNegative = -double.Epsilon;

			Assert.That(smallestPositive > zero, Is.True);
			Assert.That(zero > largestNegative, Is.True);
			Assert.That(smallestPositive > largestNegative, Is.True);
		}

		[Test]
		public void OperatorPrecedenceTest()
		{
			SV a = 2;
			SV b = 3;
			SV c = 4;

			SV result1 = a + b * c;
			Assert.That(result1.longValue, Is.EqualTo(14));

			SV result2 = (a + b) * c;
			Assert.That(result2.longValue, Is.EqualTo(20));
		}

		[Test]
		public unsafe void ScriptValueArrayHandlingTest()
		{
			var array = new SV[5];
			for (var i = 0; i < 5; i++)
				array[i] = i * 10;

			Assert.That(array[0].longValue, Is.EqualTo(0));
			Assert.That(array[2].longValue, Is.EqualTo(20));
			Assert.That(array[4].longValue, Is.EqualTo(40));

			fixed (SV* ptr = &array[2])
			{
				var scriptValuePtr = new ScriptValuePtr(ptr);
				Assert.That(scriptValuePtr.LongValue, Is.EqualTo(20));

				scriptValuePtr.RefValue.longValue = 25;
				Assert.That(array[2].longValue, Is.EqualTo(25));
			}
		}

		[Test]
		public void SafeIntegerValueRoundingTest()
		{
			SV floatValue = 42.7f;
			SV doubleValue = 42.2;

			Assert.That(floatValue.SafeIntegerValue, Is.EqualTo(42));
			Assert.That(doubleValue.SafeIntegerValue, Is.EqualTo(42));

			SV negativeFloat = -42.7f;
			SV negativeDouble = -42.2;

			Assert.That(negativeFloat.SafeIntegerValue, Is.EqualTo(-42));
			Assert.That(negativeDouble.SafeIntegerValue, Is.EqualTo(-42));
		}

		private struct InnerStruct
		{
			public int x;
			public int y;
		}

		private struct OuterStruct
		{
			public InnerStruct inner;
			public int value;
		}

		[Test]
		public void SequentialPinUnpinTest()
		{
			var count = PinHelper.PinsCount;

			for (var i = 0; i < 10; i++)
			{
				var obj = new TestClass { Value = $"Test{i}" };
				var value = SV.FromReferencePin(obj);
				Assert.That(PinHelper.PinsCount, Is.EqualTo(count + 1));
				value.UnpinManagedPointer();
				Assert.That(PinHelper.PinsCount, Is.EqualTo(count));
			}
		}

		[Test]
		public unsafe void NullPointerHandlingTest()
		{
			void* nullPtr = null;
			SV value = new SV(nullPtr, SV.ValueType.Pointer);

			// Check that null pointers are handled consistently
			Assert.That((IntPtr)value.pointerValue, Is.EqualTo((IntPtr)nullPtr));
			Assert.That(value.type, Is.EqualTo(SV.ValueType.Pointer));
		}

		[Test]
		public void ComplexNestedStructTest()
		{
			var nested = new OuterStruct
			{
				inner = new InnerStruct { x = 10, y = 20 },
				value = 30
			};

			var value = SV.FromStructAlloc(nested);

			var retrieved = value.GetStruct<OuterStruct>();
			Assert.That(retrieved.value, Is.EqualTo(30));
			Assert.That(retrieved.inner.x, Is.EqualTo(10));
			Assert.That(retrieved.inner.y, Is.EqualTo(20));
		}

		[Test]
		public void InheritanceHierarchyTest()
		{
			var derivedObj = new DerivedTestClass { Value = "Base", ExtraValue = "Derived" };
			var value = SV.FromReferencePin(derivedObj);

			// Test retrieving as base type
			var asBase = value.GetReferencePin<TestClass>();
			Assert.That(asBase.Value, Is.EqualTo("Base"));

			// Test retrieving as derived type
			var asDerived = value.GetReferencePin<DerivedTestClass>();
			Assert.That(asDerived.Value, Is.EqualTo("Base"));
			Assert.That(asDerived.ExtraValue, Is.EqualTo("Derived"));
		
			value.UnpinManagedPointer();
		}

		[Test]
		public void ArrayOfStructsTest()
		{
			var structArray = new InnerStruct[3];
			structArray[0] = new InnerStruct { x = 10, y = 20 };
			structArray[1] = new InnerStruct { x = 30, y = 40 };
			structArray[2] = new InnerStruct { x = 50, y = 60 };

			// Pin the array
			var value = SV.FromReferencePin(structArray);
			var retrieved = value.GetReferencePin<InnerStruct[]>();

			Assert.That(retrieved.Length, Is.EqualTo(3));
			Assert.That(retrieved[0].x, Is.EqualTo(10));
			Assert.That(retrieved[1].y, Is.EqualTo(40));
			Assert.That(retrieved[2].x, Is.EqualTo(50));
		
			value.UnpinManagedPointer();
		}

		[Test]
		public void CyclicReferenceHandlingTest()
		{
			// Create objects with circular references
			var obj1 = new LinkedNode();
			var obj2 = new LinkedNode();
			obj1.Next = obj2;
			obj2.Next = obj1;

			var value1 = SV.FromReferencePin(obj1);
			var retrieved = value1.GetReferencePin<LinkedNode>();

			// Verify circular reference is maintained
			Assert.That(retrieved.Next, Is.Not.Null);
			Assert.That(retrieved.Next.Next, Is.SameAs(retrieved));
		
			value1.UnpinManagedPointer();
		}

		[Test]
		public unsafe void PointerArithmeticSafetyTest()
		{
			// Setup a buffer with known values
			var buffer = new byte[100];
			for (int i = 0; i < buffer.Length; i++)
				buffer[i] = (byte)i;

			fixed (byte* ptr = buffer)
			{
				// Create ScriptValues for different positions
				SV basePtr = new SV(ptr, SV.ValueType.Pointer);

				// Manually create pointer to offset position (simulating pointer arithmetic)
				SV offsetPtr = new SV(ptr + 10, SV.ValueType.Pointer);

				// Verify correct behavior
				Assert.That(*(byte*)basePtr.pointerValue, Is.EqualTo(0));
				Assert.That(*(byte*)offsetPtr.pointerValue, Is.EqualTo(10));
			}
		}

		private class DerivedTestClass : TestClass
		{
			public string? ExtraValue { get; init; }
		}

		private class LinkedNode
		{
			public LinkedNode? Next { get; set; }
		}
	
		[Test]
		public void StringMathTest()
		{
			SV str1 = SV.FromReferenceUnsafe("Hello ");
			SV str2 = SV.FromReferenceUnsafe("World");

			Assert.Throws<Exception>(() => { _ = str1 + str2; });
		}
	}
}