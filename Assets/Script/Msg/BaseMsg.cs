using ProtoBuf;
using System;
using System.IO;
using UnityEngine;
[ProtoContract]
public class MsgBase
{
    [ProtoMember(100)]
    public virtual ProtocolEnum ProtoType { get; set; }

    /// <summary>
    /// 编码协议名
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] EncodeName(MsgBase msgBase)
    {
        byte[] nameBytes = System.Text.Encoding.UTF8.GetBytes(msgBase.ProtoType.ToString());
        Int16 len = (Int16)nameBytes.Length;
        byte[] bytes = new byte[2 + len];
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        Array.Copy(nameBytes, 0, bytes, 2, len);
        return bytes;
    }

    /// <summary>
    /// 解码协议名，前两位是协议名长度，后面是协议名
    /// </summary>
    /// <param name="bytes"></param>
    /// <returns></returns>
    public static ProtocolEnum DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        if (offset + 2 > bytes.Length) return ProtocolEnum.None;
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        if (offset + 2 + len > bytes.Length) return ProtocolEnum.None;
        count = 2 + len;
        try
        {
            string name = System.Text.Encoding.UTF8.GetString(bytes, offset + 2, len);
            return (ProtocolEnum)System.Enum.Parse(typeof(ProtocolEnum), name);
        }
        catch (Exception ex)
        {
            Debug.LogError("不存在的协议:" + ex.ToString());
            return ProtocolEnum.None;
        }
    }

    /// <summary>
    /// 协议序列化及加密
    /// </summary>
    /// <param name="msgBase"></param>
    /// <returns></returns>
    public static byte[] Encond(MsgBase msgBase)
    {   //创建一个内存缓冲区用来存放序列化后的数据，using确保在使用完后会执行distory方法，否则会造成内存泄漏
        using (var memory = new MemoryStream())
        {
            //将我们的协议类进行序列化转换成数组
            Serializer.Serialize(memory, msgBase);
            byte[] bytes = memory.ToArray();//获取到序列化后的字节数组
            return bytes;
        }
    }

    /// <summary>
    /// 协议解密
    /// </summary>
    /// <param name="protocol"></param>
    /// <param name="bytes"></param>
    /// <param name="offset"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static MsgBase Decode(ProtocolEnum protocol, byte[] bytes, int offset, int count)
    {
        if (count <= 0)
        {
            Debug.LogError("协议解密出错，数据长度为0");
            return null;
        }
        try
        {
            byte[] newBytes = new byte[count];
            Array.Copy(bytes, offset, newBytes, 0, count);
            using (var memory = new MemoryStream(newBytes, 0, newBytes.Length))
            {
                Type t = System.Type.GetType(protocol.ToString());//根据名称去（反射）获取到对应的类型信息
                return (MsgBase)Serializer.NonGeneric.Deserialize(t, memory);//内存流中的字节数组反序列化为对象
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("协议解密出错:" + ex.ToString());
            return null;
        }
    }
}

