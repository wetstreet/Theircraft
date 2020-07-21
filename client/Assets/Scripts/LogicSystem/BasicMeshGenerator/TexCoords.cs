using UnityEngine;

public class TexCoords
{
    public bool isTransparent = false;  // side blocks don't cull face
    public bool isCollidable = true;    // colliding with player
    public bool isRotatable = false;    // stores orientation
    public bool isPlant = false;        // uses plant mesh generator
    public bool isStair = false;        // uses stair mesh generator
    public bool isWall = false;         // uses wall mesh generator
    public bool isSlab = false;         // uses slab mesh generator
    public bool isVerticalSlab = false; // uses vertical slab mesh generator

    public Vector2Int front;
    public Vector2Int right;
    public Vector2Int left;
    public Vector2Int back;
    public Vector2Int top;
    public Vector2Int bottom;

    private TexCoords() { }

    public static TexCoords None()
    {
        return new TexCoords();
    }

    public static TexCoords Block_1(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = uv,
            right = uv,
            left = uv,
            back = uv,
            top = uv,
            bottom = uv
        };
    }

    public static TexCoords Block_1_transparent(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = uv,
            right = uv,
            left = uv,
            back = uv,
            top = uv,
            bottom = uv
        };
    }

    public static TexCoords Block_front_polar_side(Vector2Int front, Vector2Int polar, Vector2Int side, bool isRotatable = false)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = isRotatable,
            isCollidable = true,
            front = front,
            right = side,
            left = side,
            back = side,
            top = polar,
            bottom = polar
        };
    }

    public static TexCoords Block_polar_side(Vector2Int polar, Vector2Int side, bool isRotatable = false)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = isRotatable,
            isCollidable = true,
            front = side,
            right = side,
            left = side,
            back = side,
            top = polar,
            bottom = polar
        };
    }

    public static TexCoords Block_top_bottom_side(Vector2Int top, Vector2Int bottom, Vector2Int side)
    {
        return new TexCoords
        {
            isTransparent = false,
            isPlant = false,
            isRotatable = false,
            isCollidable = true,
            front = side,
            right = side,
            left = side,
            back = side,
            top = top,
            bottom = bottom
        };
    }

    public static TexCoords Plant(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isPlant = true,
            front = uv
        };
    }

    public static TexCoords Stair(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isRotatable = true,
            isStair = true,
            front = uv
        };
    }

    public static TexCoords Wall(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isWall = true,
            front = uv,
        };
    }

    public static TexCoords Torch(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isCollidable = false,
            front = uv
        };
    }

    public static TexCoords Slab(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isSlab = true,
            front = uv,
        };
    }
    
    public static TexCoords VerticalSlab(Vector2Int uv)
    {
        return new TexCoords
        {
            isTransparent = true,
            isVerticalSlab = true,
            front = uv,
        };
    }

    public static TexCoords Chest()
    {
        return new TexCoords
        {
            isTransparent = true,
        };
    }
}