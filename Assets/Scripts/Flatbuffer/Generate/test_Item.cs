// <auto-generated>
//  automatically generated by the FlatBuffers compiler, do not modify
// </auto-generated>

using global::System;
using global::FlatBuffers;

public struct Test_Item : IFlatbufferObject
{
  private Table __p;
  public ByteBuffer ByteBuffer { get { return __p.bb; } }
  public static Test_Item GetRootAsTest_Item(ByteBuffer _bb) { return GetRootAsTest_Item(_bb, new Test_Item()); }
  public static Test_Item GetRootAsTest_Item(ByteBuffer _bb, Test_Item obj) { return (obj.__assign(_bb.GetInt(_bb.Position) + _bb.Position, _bb)); }
  public void __init(int _i, ByteBuffer _bb) { __p.bb_pos = _i; __p.bb = _bb; }
  public Test_Item __assign(int _i, ByteBuffer _bb) { __init(_i, _bb); return this; }

  public int ID { get { int o = __p.__offset(4); return o != 0 ? __p.bb.GetInt(o + __p.bb_pos) : (int)0; } }
  public string Name { get { int o = __p.__offset(6); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetNameBytes() { return __p.__vector_as_span(6); }
#else
  public ArraySegment<byte>? GetNameBytes() { return __p.__vector_as_arraysegment(6); }
#endif
  public byte[] GetNameArray() { return __p.__vector_as_array<byte>(6); }
  public double Money { get { int o = __p.__offset(8); return o != 0 ? __p.bb.GetDouble(o + __p.bb_pos) : (double)0.0; } }
  public string Desc { get { int o = __p.__offset(10); return o != 0 ? __p.__string(o + __p.bb_pos) : null; } }
#if ENABLE_SPAN_T
  public Span<byte> GetDescBytes() { return __p.__vector_as_span(10); }
#else
  public ArraySegment<byte>? GetDescBytes() { return __p.__vector_as_arraysegment(10); }
#endif
  public byte[] GetDescArray() { return __p.__vector_as_array<byte>(10); }

  public static Offset<Test_Item> CreateTest_Item(FlatBufferBuilder builder,
      int ID = 0,
      StringOffset NameOffset = default(StringOffset),
      double Money = 0.0,
      StringOffset DescOffset = default(StringOffset)) {
    builder.StartObject(4);
    Test_Item.AddMoney(builder, Money);
    Test_Item.AddDesc(builder, DescOffset);
    Test_Item.AddName(builder, NameOffset);
    Test_Item.AddID(builder, ID);
    return Test_Item.EndTest_Item(builder);
  }

  public static void StartTest_Item(FlatBufferBuilder builder) { builder.StartObject(4); }
  public static void AddID(FlatBufferBuilder builder, int ID) { builder.AddInt(0, ID, 0); }
  public static void AddName(FlatBufferBuilder builder, StringOffset NameOffset) { builder.AddOffset(1, NameOffset.Value, 0); }
  public static void AddMoney(FlatBufferBuilder builder, double Money) { builder.AddDouble(2, Money, 0.0); }
  public static void AddDesc(FlatBufferBuilder builder, StringOffset DescOffset) { builder.AddOffset(3, DescOffset.Value, 0); }
  public static Offset<Test_Item> EndTest_Item(FlatBufferBuilder builder) {
    int o = builder.EndObject();
    return new Offset<Test_Item>(o);
  }
};
