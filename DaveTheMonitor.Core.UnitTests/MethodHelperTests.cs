using DaveTheMonitor.Core.Helpers;
using System.Reflection;

namespace DaveTheMonitor.Core.UnitTests
{
    [TestClass]
    public class MethodHelperTests
    {
        private class TestClass1 : TestInterface1
        {
            public static MethodInfo StaticNoRetInfo { get; private set; }
            public static MethodInfo StaticNoRetArgInfo { get; private set; }
            public static MethodInfo StaticNoRetBoxedArgInfo { get; private set; }
            public static MethodInfo StaticNoRetObjInfo { get; private set; }
            public static MethodInfo StaticNoRetStringInfo { get; private set; }
            public static MethodInfo InstanceNoRetInfo { get; private set; }
            public static MethodInfo InstanceNoRetArgInfo { get; private set; }
            public static MethodInfo InstanceNoRetTC2Info { get; private set; }
            public static MethodInfo InstanceNoRetInterfaceInfo { get; private set; }
            public static MethodInfo InstanceRetInfo { get; private set; }
            public static int StaticValue { get; private set; }
            public static object StaticObject { get; private set; }
            public static int StaticTarget { get; set; }
            public int Value { get; private set; }
            public int Target { get; set; }
            int TestInterface1.IntValue1 => Value;

            public static void Setup()
            {
                Type type = typeof(TestClass1);
                StaticNoRetInfo = type.GetMethod(nameof(StaticNoRet), BindingFlags.NonPublic | BindingFlags.Static);
                StaticNoRetArgInfo = type.GetMethod(nameof(StaticNoRetArg), BindingFlags.NonPublic | BindingFlags.Static);
                StaticNoRetBoxedArgInfo = type.GetMethod(nameof(StaticNoRetBoxedArg), BindingFlags.NonPublic | BindingFlags.Static);
                StaticNoRetObjInfo = type.GetMethod(nameof(StaticNoRetObj), BindingFlags.NonPublic | BindingFlags.Static);
                StaticNoRetStringInfo = type.GetMethod(nameof(StaticNoRetString), BindingFlags.NonPublic | BindingFlags.Static);
                InstanceNoRetInfo = type.GetMethod(nameof(InstanceNoRet), BindingFlags.NonPublic | BindingFlags.Instance);
                InstanceNoRetArgInfo = type.GetMethod(nameof(InstanceNoRetArg), BindingFlags.NonPublic | BindingFlags.Instance);
                InstanceNoRetTC2Info = type.GetMethod(nameof(InstanceNoRetTC2), BindingFlags.NonPublic | BindingFlags.Instance);
                InstanceNoRetInterfaceInfo = type.GetMethod(nameof(InstanceNoRetInterface), BindingFlags.NonPublic | BindingFlags.Instance);
                InstanceRetInfo = type.GetMethod(nameof(InstanceRet), BindingFlags.NonPublic | BindingFlags.Instance);
                StaticValue = 0;
                StaticTarget = 0;
            }

            private static void StaticNoRet()
            {
                StaticValue = 10;
            }

            private static void StaticNoRetArg(int value)
            {
                StaticValue = value;
            }

            private static void StaticNoRetBoxedArg(object value)
            {
                StaticValue = (int)value;
            }

            private static void StaticNoRetObj(object value)
            {
                StaticObject = value;
            }

            private static void StaticNoRetString(string value)
            {
                StaticObject = value;
            }

            private void InstanceNoRet()
            {
                Value = Target;
            }

            private void InstanceNoRetArg(int value)
            {
                Value = value;
            }

            private void InstanceNoRetTC2(TestClass2 value)
            {
                Value = value.Value;
            }

            private void InstanceNoRetInterface(TestInterface2 value)
            {
                Value = value.IntValue2;
            }

            private int InstanceRet()
            {
                return Value;
            }

            public TestClass1()
            {
                Value = 0;
                Target = 0;
            }

            public TestClass1(int value)
            {
                Value = value;
                Target = value;
            }
        }

        public class TestClass2 : TestInterface2
        {
            public int Value { get; private set; }
            int TestInterface2.IntValue2 => Value;

            public TestClass2(int value)
            {
                Value = value;
            }
        }

        public interface TestInterface1
        {
            public int IntValue1 { get; }
        }

        public interface TestInterface2
        {
            public int IntValue2 { get; }
        }

        [ClassInitialize]
        public static void Setup(TestContext context)
        {
            TestClass1.Setup();
        }

        [TestMethod]
        public void StaticNoRet()
        {
            MethodInfo target = TestClass1.StaticNoRetInfo;
            int targetValue = 10;
            TestClass1.StaticTarget = targetValue;

            var invoker = MethodHelper.CreateInvoker<Action>(target);
            invoker();
            Assert.AreEqual(TestClass1.StaticValue, targetValue);
        }

        [TestMethod]
        public void StaticNoRetArg()
        {
            MethodInfo target = TestClass1.StaticNoRetArgInfo;
            int targetValue = 30;

            var invoker = MethodHelper.CreateInvoker<Action<int>>(target);
            invoker(targetValue);
            Assert.AreEqual(TestClass1.StaticValue, targetValue);
        }

        [TestMethod]
        public void StaticNoRetArg_InvokerUnbox()
        {
            MethodInfo target = TestClass1.StaticNoRetArgInfo;
            int targetValue = 30;

            var invoker = MethodHelper.CreateInvoker<Action<object>>(target);
            invoker(targetValue);
            Assert.AreEqual(TestClass1.StaticValue, targetValue);
        }

        [TestMethod]
        public void StaticNoRetArg_InvokerBox()
        {
            MethodInfo target = TestClass1.StaticNoRetBoxedArgInfo;
            int targetValue = 15;

            var invoker = MethodHelper.CreateInvoker<Action<int>>(target);
            invoker(targetValue);
            Assert.AreEqual(TestClass1.StaticValue, targetValue);
        }

        [TestMethod]
        public void StaticNoRetObj_InvokerString()
        {
            MethodInfo target = TestClass1.StaticNoRetObjInfo;
            string targetValue = "Target1";

            var invoker = MethodHelper.CreateInvoker<Action<string>>(target);
            invoker(targetValue);
            Assert.AreEqual(TestClass1.StaticObject, targetValue);
        }

        [TestMethod]
        public void StaticNoRetObj_InvokerObj()
        {
            MethodInfo target = TestClass1.StaticNoRetObjInfo;
            string targetValue = "Target2";

            var invoker = MethodHelper.CreateInvoker<Action<object>>(target);
            invoker(targetValue);
            Assert.AreEqual(TestClass1.StaticObject, targetValue);
        }

        [TestMethod]
        public void StaticNoRetString_InvokerConvert()
        {
            MethodInfo target = TestClass1.StaticNoRetStringInfo;
            string targetValue = "Target3";

            var invoker = MethodHelper.CreateInvoker<Action<object>>(target);
            invoker(targetValue);
            Assert.AreEqual(TestClass1.StaticObject, targetValue);
        }

        [TestMethod]
        public void StaticNoRetString_InvokerNoConvert()
        {
            MethodInfo target = TestClass1.StaticNoRetStringInfo;
            string targetValue = "Target4";

            var invoker = MethodHelper.CreateInvoker<Action<string>>(target);
            invoker(targetValue);
            Assert.AreEqual(TestClass1.StaticObject, targetValue);
        }

        [TestMethod]
        public void InstanceNoRet_InvokerNoConvert()
        {
            MethodInfo target = TestClass1.InstanceNoRetInfo;
            var instance = new TestClass1();
            int targetValue = 40;
            instance.Target = targetValue;

            var invoker = MethodHelper.CreateInvoker<Action<TestClass1>>(target);
            invoker(instance);
            Assert.AreEqual(instance.Value, targetValue);
        }

        [TestMethod]
        public void InstanceNoRet_InvokerConvert()
        {
            MethodInfo target = TestClass1.InstanceNoRetInfo;
            var instance = new TestClass1();
            int targetValue = 40;
            instance.Target = targetValue;

            var invoker = MethodHelper.CreateInvoker<Action<object>>(target);
            invoker(instance);
            Assert.AreEqual(instance.Value, targetValue);
        }

        [TestMethod]
        public void InstanceNoRetArg_InvokerConvert()
        {
            MethodInfo target = TestClass1.InstanceNoRetArgInfo;
            var instance = new TestClass1();
            int targetValue = 55;

            var invoker = MethodHelper.CreateInvoker<Action<object, int>>(target);
            invoker(instance, targetValue);
            Assert.AreEqual(instance.Value, targetValue);
        }

        [TestMethod]
        public void InstanceNoRetTC2_InvokerConvert()
        {
            MethodInfo target = TestClass1.InstanceNoRetTC2Info;
            var instance = new TestClass1();
            int targetValue = 80;

            var invoker = MethodHelper.CreateInvoker<Action<object, TestClass2>>(target);
            invoker(instance, new TestClass2(targetValue));
            Assert.AreEqual(instance.Value, targetValue);
        }

        [TestMethod]
        public void InstanceNoRetInterface_InvokerConvert()
        {
            MethodInfo target = TestClass1.InstanceNoRetTC2Info;
            var instance = new TestClass1();
            int targetValue = 90;

            var invoker = MethodHelper.CreateInvoker<Action<object, TestInterface2>>(target);
            invoker(instance, new TestClass2(targetValue));
            Assert.AreEqual(instance.Value, targetValue);
        }

        [TestMethod]
        public void InstanceRet_InvokerConvert()
        {
            MethodInfo target = TestClass1.InstanceRetInfo;
            int targetValue = 75;
            var instance = new TestClass1(targetValue);

            var invoker = MethodHelper.CreateInvoker<Func<object, int>>(target);
            int value = invoker(instance);
            Assert.AreEqual(value, targetValue);
        }

        [TestMethod]
        public void InstanceRet_InvokerConvertInterface()
        {
            MethodInfo target = TestClass1.InstanceRetInfo;
            int targetValue = 85;
            var instance = new TestClass1(targetValue);

            var invoker = MethodHelper.CreateInvoker<Func<TestInterface1, int>>(target);
            int value = invoker(instance);
            Assert.AreEqual(value, targetValue);
        }
    }
}