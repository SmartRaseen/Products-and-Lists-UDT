using System;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;
using System.Text;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlClient;
using System.Collections;
using System.Data;

[Serializable]
[SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 6000)]


public class ListProduct : INullable, IBinarySerialize
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

    public static ListProduct Null
    {
        get
        {
            ListProduct list = new ListProduct();
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
    public static ListProduct Parse(SqlString sqlString)
    {
        if (sqlString.IsNull)
            return Null;
        ListProduct list = new ListProduct();
        list.ProductList = list.GetProductList(sqlString.Value);    
        return list;
    }

    public List<Product> GetProductList(string listData)
    {
        string[] delimiter = new string[5] { "#", "#", "#", "#", "#" };
        List<Product> list = new List<Product>();
        string[] listofProduct = listData.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
        Count = listofProduct.Length > list_Length ? list_Length : listofProduct.Length;
        for(int i = 0; i < Count;i++)
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

    [SqlFunction(
    DataAccess = DataAccessKind.Read,
    FillRowMethodName = "ProductFillRow",
    TableDefinition = "   ProductID INT, ProductName NVARCHAR(100),ProductQuantity INT,ProductPrice DECIMAL")]
    public static IEnumerable GetAsProductTable(ListProduct obj)
    {
        ArrayList resultCollection = new ArrayList();

        var arrayObject = obj.ProductList;
        foreach(var items in arrayObject)
        {
            resultCollection.Add(new Product() { ProductID = items.ProductID, ProductName = items.ProductName, ProductQuantity = items.ProductQuantity, ProductPrice = items.ProductPrice });
        }
        return resultCollection;
    }

    public static void ProductFillRow(object result, out SqlInt32 ProductID, out SqlString ProductName, out SqlInt32 ProductQuantity, out SqlDecimal ProductPrice)
    {
        if(result is Product rowObject)
        {
            ProductID = new SqlInt32(rowObject.ProductID);
            ProductName = new SqlString(rowObject.ProductName);
            ProductQuantity = new SqlInt32(rowObject.ProductQuantity);
            ProductPrice = new SqlDecimal(rowObject.ProductPrice);
        }
        else
        {
            ProductID = default(SqlInt32);
            ProductName = default(SqlString);
            ProductQuantity = default(SqlInt32);
            ProductPrice = default(SqlDecimal);
        }
    }

    public static void ProductTable(ListProduct listObject)
    {
        using (SqlConnection connection = new SqlConnection("context connection=true"))
        {
            connection.Open();
            foreach (Product product in listObject.ProductList)
            {
                SqlCommand command = connection.CreateCommand();
                command.CommandText = "Insert into Product(productID,productName,productQuantity,productPrice) values(@ID,@name, @quantity, @price)";
                command.Parameters.AddWithValue("@ID", product.ProductID);
                command.Parameters.AddWithValue("@name", product.ProductName);
                command.Parameters.AddWithValue("@quantity", product.ProductQuantity);
                command.Parameters.AddWithValue("@price", product.ProductPrice);

                command.ExecuteNonQuery();
            }
        }
    }

    public string ToXml()
    {
        var serializer = new XmlSerializer(typeof(ListProduct));
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

    public ListProduct AddProducts(SqlString sqlString)
    {
        var product = Product.Parse(sqlString);

        if(Count<list_Length)
        {
            ProductList.Add(product);
            Count++;
        }
        return this;
    }

    public ListProduct RemoveProduct()
    {
        if (Count > 0)
        {
            ProductList.RemoveAt(Count - 1);
            Count--;
        }
        return this;
    }
}