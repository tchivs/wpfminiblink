# MiniBlink.Net
采用.net动态生成Miniblink的代理类，智能识别32位或64位系统自动加载相应DLL，可以只依赖于32位miniblink。

当前只实现了wpf的简单封装，由于MiniBlink的方法太多了还在慢慢封装，现在只先实现常用的和在项目中会用到的。
## 特性
* 可以根据客户端操作系统来加载32/64位的MiniBlink(动态编译代理类)

## 项目结构
* AutoDllProxy-用于生成代理类
* MiniBlink.Share-封装MiniBlink并在运行时创建代理类
* MiniBlink.Wpf-MiniBlink的Wpf自定义控件
* MiniBlink.WpfDemo-测试程序
## 致谢
本项目基于各开源代码修改而来，同时借鉴了C#对miniblink封装的各种代码，包括但不限于

* https://github.com/yuanfeng-net/wpfminiblink (我是通过该项目改造的)
* https://gitee.com/aochulai/NetMiniblink (这个是普通版的封装，较完善)  
* https://gitee.com/kyozy/miniblinknet (这个是普通版的封装)
* https://github.com/ampereufo/MiniBlink_VIPDemo (这个是定制版的封装)
* https://github.com/e024/miniblinkpinvokevip (这个是定制版的封装)
* https://github.com/duwenjie15/Raindrops.UI.WebView (这个是定制版的封装)

