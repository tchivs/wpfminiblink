using System;
using System.IO;

namespace MiniBlink.Share
{
    public static class MiniBlinkExtensions
    {
        public static void Destroy(this MiniBlink miniBlink)
        {
            MiniBlink.Proxy.DestroyWebView(miniBlink);
        }

        public static MiniBlink SetNavigationToNewWindowEnable(this MiniBlink miniBlink, bool enable)
        {
            MiniBlink.Proxy.SetNavigationToNewWindowEnable(miniBlink, enable);
            return miniBlink;
        }

        public static MiniBlink SetDragDropEnable(this MiniBlink miniBlink, bool enable)
        {
            MiniBlink.Proxy.SetDragDropEnable(miniBlink, enable);
            return miniBlink;
        }

        public static MiniBlink SetDragEnable(this MiniBlink miniBlink, bool enable)
        {
            MiniBlink.Proxy.SetDragEnable(miniBlink, enable);
            return miniBlink;
        }

        /// <summary>
        /// 显示调试窗口（需要程序目录下存在front_end目录，如果没有就把res里面的目录复制出来）
        /// </summary>
        /// <param name="miniBlink"></param>
        /// <returns></returns>
        public static MiniBlink ShowDevTools(this MiniBlink miniBlink)
        {
            var path = Path.Combine(System.Environment.CurrentDirectory, "front_end", "inspector.html");
            if (File.Exists(path))
            {
                MiniBlink.Proxy.ShowDevTools(miniBlink, path, null, IntPtr.Zero);
            }

            return miniBlink;
        }

        public static MiniBlink LoadUrl(this MiniBlink miniBlink, string url)
        {
            MiniBlink.Proxy.LoadURL(miniBlink, url);
            return miniBlink;
        }

        public static MiniBlink SetUserAgent(this MiniBlink miniBlink, string userAgent)
        {
            MiniBlink.Proxy.SetUserAgent(miniBlink, userAgent);
            return miniBlink;
        }

        public static string GetUserAgent(this MiniBlink miniBlink)
        {
            return MiniBlink.Proxy.GetUserAgent(miniBlink);
        }
    }
}