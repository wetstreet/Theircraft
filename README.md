# Theircraft-unity
踩坑项目，目标是实现Minecraft的功能。

参考：

- https://github.com/skligys/cardboard-creeper

- https://github.com/fogleman/Craft

- https://github.com/Team-RTG/Realistic-Terrain-Generation

Unity版本: Unity 2018.2.11f1

需求池（想到的点子都丢进去，有兴趣的就接下来并挂在自己名下，一个模块可以挂在多人名下）：

背包系统  roshan

合成系统

战斗系统

怪物及AI

设置界面

VR支持

移动端支持  roshan

热更机制

移动 ericchan

地图生成 ericchan

支持每个面显示不同材质的方块 ericchan

UI系统 ericchan

网络层 ericchan

指示当前指向的方块 ericchan

准星

方块类

支持读取材质包 ericchan

数据库


已踩的坑：

1.玩家的移动使用CharacterController，而不是rigidbody，否则碰撞时会疯狂鬼畜。

2.贴图的fileter mode需要选择point（no filter）

3.材质需要勾选enable gpu instancing，否则不会动态合并drawcall

4.使用Rider第一次打开项目报错: .netframework 3.5找不到，解决方法是在unity 设置中选择Rider作为编辑器，再使用Rider自带的.net库
