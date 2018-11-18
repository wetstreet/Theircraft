---
--- 负责和CsLuaBase挂载GameObject
--- Created by roshan.
--- DateTime: 2018/11/17 17:33
---

luaObjMgr = {}
local ctorTable = {}
local luaObj = {}

--[[--
 * @Description: 注册某类  
 * @param:       luaClassName : 某类的名字
                 constructFunc: 某类的构造函数
 ]]
function luaObjMgr.registerLuaClass(luaClassName, constructFunc)
    ctorTable[luaClassName] = constructFunc
end

--[[--
 * @Description: 根据luaClassName创建某lua类 
 * @param:       luaClassName 某类的名字 
 ]]
function luaObjMgr.createLuaObject(luaClassName, gameObject, ID)
    local constructFunc = ctorTable[luaClassName]
    if (nil ~= ctorTable[luaClassName]) then
        local luaObject = constructFunc()
        luaObj[gameObject] = luaObject
        luaObject.gameObject = gameObject
        luaObject.transform = gameObject.transform
        luaObject.ID = ID
    end
end

function luaObjMgr.AddLuaUIObject(go, luaObj)
    luaObj[go] = luaObj
end

function luaObjMgr.RemoveLuaUIObject(go)
    luaObj[go] = nil
end

--[[--
 * @Description: 根据gameobject得到绑定的相应lua对象
 * @param:       gameObject u3d object对象 
 ]]
function luaObjMgr.getLuaObjByGameObj(gameObject)
    return luaObj[gameObject]
end