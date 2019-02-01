# Theircraft-unity
踩坑项目，目标是实现Minecraft的功能。

19/02/01 之后更新或安装的项目：

需要打开window->package manager，安装entities和hybrid renderer。然后在project视窗里选中packages下的Unity.Entities.PerformanceTests文件夹，打开同名的asmdef后缀文件，去掉第4行的Unity.Entities.PerformanceTests并保存.

Unity版本: 跟着最新版走（2018的Official Releases）

需求池：

背包系统  roshan

合成系统

战斗系统

怪物及AI

设置界面 ericchan

VR支持 ericchan

移动端支持  roshan

热更机制 roshan 

地图生成 ericchan


已踩的坑：

1.玩家的移动使用CharacterController，而不是rigidbody，否则碰撞时会疯狂鬼畜。

2.贴图的fileter mode需要选择point（no filter）

3.材质需要勾选enable gpu instancing，否则不会开启gpu instancing来动态合批

4.使用Rider第一次打开项目报错: .netframework 3.5找不到，解决方法是在unity 设置中选择Rider作为编辑器，再使用Rider自带的.net库

5.外网服务器发送包时会因为发送速度比客户端的接收速度慢，导致客户端的接收函数把已发送的部分数据读取完后以为包已结束就返回了。这个问题在本地服务器不存在，耗费了很多时间排查问题。解决方法是客户端接通过包头拿到该包的长度，在没有接收完这个包之前不进行下一步处理。
