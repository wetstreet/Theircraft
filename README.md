# Theircraft-unity
A Minecraft clone using Unity.

Unity Version: Unity 2018.2.11f1

功能：

移动（基本功能已完成）

地图生成（基本功能已完成）

方块类（正在开发）

支持每个面显示不同材质的方块（正在开发）

支持读取材质包（准备开发）



网络底层（未来计划）

数据库（未来计划）


已踩的坑：

1.玩家的移动使用CharacterController，而不是rigidbody，否则碰撞时会疯狂鬼畜。

2.贴图的fileter mode需要选择point（no filter）

3.材质需要勾选enable gpu instancing，否则不会动态合并drawcall
