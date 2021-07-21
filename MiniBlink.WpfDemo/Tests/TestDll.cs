using System;
using AutoDllProxy;
using MiniBlink.Share;
using NUnit.Framework;

namespace MiniBlink.WpfDemo.Tests
{
    [TestFixture]
    public class TestDll
    {
        private readonly IMiniBlinkProxy p;

        public TestDll()
        {
            this.p = DllModuleBuilder.Create().Build<IMiniBlinkProxy>();
        }

        [SetUp]
        public void Initialize()
        {
            //这里写运行每一个测试用例时需要初始化的代码
            if (!p.IsInitialize())
            {
                p.Initialize();
            }
        }

        /// <summary>
        /// 测试 custom.ini 配置信息的读取
        /// </summary>
        [Test]
        public void TestIsInitialize()
        {
            Assert.True(p.IsInitialize());
        }  
        [Test]
        public void GetVersionStringTest()
        {
            var v=p.GetVersionString();
            Assert.NotNull(v);
            Console.WriteLine(v);
        }
    }
}