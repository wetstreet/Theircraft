# Theircraft-unity
A Minecraft clone using Unity.

Unity Version: Unity 2018.2.11f1

功能：

移动（已完成，待优化）

地图生成（已完成，待优化）

支持每个面显示不同材质的方块（已完成，待优化）

方块类（准备开发）

支持读取材质包（准备开发）

UI系统（准备开发）

网络底层（未来计划）

数据库（未来计划）


已踩的坑：

1.玩家的移动使用CharacterController，而不是rigidbody，否则碰撞时会疯狂鬼畜。

2.贴图的fileter mode需要选择point（no filter）

3.材质需要勾选enable gpu instancing，否则不会动态合并drawcall

4.使用Rider第一次打开项目报错: .netframework 3.5找不到，解决方法是在unity 设置中选择Rider作为编辑器，再使用Rider自带的.net库
