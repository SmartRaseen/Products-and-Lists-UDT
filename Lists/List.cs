using System;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
[SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 6000)]
public class List: INullable, IBinarySerialize
{
    private bool is_Null;
    private const int list_Length = 20;
    private int list_Size;
    public bool IsNull
    {
        get
        {
            return is_Null;
        }
    }

    public static List Null
    {
        get
        {
            List list = new List();
            list.is_Null = true;
            return list;
        }
    }

    public int ListSize
    {
        get
        {
            return list_Size;
        }
        set
        {
            if (value > list_Length)
                list_Size = list_Length;
            else
                list_Size = value;
        }
    }
    
    public List<Product> ProductList { get; set; }
    public int Count { get; set; }
    public static List Parse(SqlString sqlString)
    {
        if (sqlString.IsNull)
            return Null;
        List list = new List();
        list.ProductList = list.GetProductList(sqlString.Value);    
        return list;
    }

    public List<Product> GetProductList(string listData)
    {
        string[] limits = new string[5] { "#", "#", "#", "#", "#" };
        List<Product> list = new List<Product>();
        string[] listofProduct = listData.Split(limits,StringSplitOptions.RemoveEmptyEntries);
        Count = listofProduct.Length > list_Length ? list_Length : listofProduct.Length;
        for(int i=0;i<Count;i++)
        {
            list.Add(Product.Parse(listofProduct[i]));
        }
        return list;
    }

    public override string ToString()
    {
        if (this.is_Null)
            return null;

        return string.Join(",",ProductList);
    }

    public string ToXml()
    {
        var serializer = new XmlSerializer(typeof(List));
        var stringWriter = new StringWriter();
        serializer.Serialize(stringWriter,this);
        return stringWriter.ToString();
    }
    
    public void Write(BinaryWriter binaryWriter)
    {
        var formatter = new BinaryFormatter();
        var stream = new MemoryStream();
        formatter.Serialize(stream,ProductList);
        var byteArray = stream.ToArray();
        var byteLength = byteArray.Length;
        binaryWriter.Write(byteLength);
        binaryWriter.Write(byteArray);
        binaryWriter.Write(Count);
        binaryWriter.Write(is_Null);
    }

    public void Read(BinaryReader binaryReader)
    {
        var byteLength = binaryReader.ReadInt32();
        var byteArray = binaryReader.ReadBytes(byteLength);
        var formatter = new BinaryFormatter();
        var stream = new MemoryStream(byteArray);
        ProductList = (List<Product>)formatter.Deserialize(stream);
        Count = binaryReader.ReadInt32();
        is_Null = binaryReader.ReadBoolean();
    }

    public List AddProducts(SqlString sqlString)
    {
        var product = Product.Parse(sqlString);

        if(Count<list_Length)
        {
            ProductList.Add(product);
            Count++;
        }
        return this;
    }

    public List RemoveProduct()
    {
        if (Count > 0)
        {
            ProductList.RemoveAt(Count - 1);
            Count--;
        }
        return this;
    }
}