using System;
using AutoDllProxy;
using MiniBlink.Share;
using NUnit.Framework;
using System.Runtime.InteropServices;

namespace MiniBlink.WpfDemo.Tests
{
    public static class Api
    {
        [DllImport("node_x64.dll", EntryPoint = "wkeGetVersionString", CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr wkeGetVersionString();
    }

    [TestFixture]
    public class TestDll
    {
        private readonly IMiniBlinkProxy p;

        public TestDll()
        {
            this.p = DllModuleBuilder.Create(Platform.Any).Build<IMiniBlinkProxy>();
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
        public void GetVersionTest()
        {
            var v = p.GetVersion();
            Assert.True(v != 0);
        }
        [Test]
        public void CreateWebViewTest()
        {
            var v = p.CreateWebView();
            Assert.True(v!=IntPtr.Zero);
        }
    }
}