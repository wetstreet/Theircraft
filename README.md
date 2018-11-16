# Theircraft-unity
踩坑项目，目标是实现Minecraft的功能。

Unity版本: Unity 2018.2.11f1

待领取需求池（想到的点子都丢进去，有兴趣的就接下来并挂在自己名下，一个模块可以挂在多人名下）：

背包系统  roshan

合成系统

战斗系统

怪物及AI

设置界面

VR支持

移动端支持

热更机制

ericchan:

移动（基本功能已完成，待优化）

地图生成（基本功能已完成，待优化）

支持每个面显示不同材质的方块（基本功能已完成，待优化）

UI系统（基本功能已实现，待优化）

网络层（基本功能已实现，待优化）

指示当前指向的方块（目前暂时用改变颜色来实现，但是只能用于standard的shader，后续看下怎么改成线框）

准星（在考虑用ui来做还是用底层api）

方块类（准备开发）

支持读取材质包（准备开发）

数据库（准备开发）


已踩的坑：

1.玩家的移动使用CharacterController，而不是rigidbody，否则碰撞时会疯狂鬼畜。

2.贴图的fileter mode需要选择point（no filter）

3.材质需要勾选enable gpu instancing，否则不会动态合并drawcall

4.使用Rider第一次打开项目报错: .netframework 3.5找不到，解决方法是在unity 设置中选择Rider作为编辑器，再使用Rider自带的.net库
