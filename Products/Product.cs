using System;
using System.Data.SqlTypes;
using System.IO;
using Microsoft.SqlServer.Server;
using System.Text;

[Serializable]
[SqlUserDefinedType(Format.UserDefined, IsByteOrdered = true, MaxByteSize = 6000)]
public class Product : INullable, IBinarySerialize
{
    private bool is_Null;
    private int productID;
    private string productName;
    private int productQuantity;
    private decimal productPrice;

    public bool IsNull
    {
        get
        {
            return is_Null;
        }
    }

    public static Product Null
    {
        get
        {
            Product list = new Product();
            list.is_Null = true;
            return list;
        }
    }

    public int ProductID
    {
        get
        {
            return productID;
        }
        set
        {
            int temp = productID;
            productID = value;
            if (!ValidateProduct())
            {
                productID = temp;
                throw new ArgumentException("Invalid ProductID..");
            }
        }
    }

    public string ProductName
    {
        get
        {
            return productName;
        }
        set
        {
            string temp = productName;
            productName = value;
            if (!ValidateProduct())
            {
                productName = temp;
                throw new ArgumentException("Invalid ProductName..");
            }
        }
    }

    public int ProductQuantity
    {
        get
        {
            return productQuantity;
        }
        set
        {
            int temp = productQuantity;
            productQuantity = value;
            if (!ValidateProduct())
            {
                productQuantity = temp;
                throw new ArgumentException("Invalid ProductQuantity..");
            }
        }
    }

    public decimal ProductPrice
    {
        get
        {
            return productPrice;
        }
        set
        {
            decimal temp = productPrice;
            productPrice = value;
            if (!ValidateProduct())
            {
                productPrice = temp;
                throw new ArgumentException("Invalid ProductPrice..");
            }
        }
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append(productID);
        builder.Append(",");
        builder.Append(productName);
        builder.Append(",");
        builder.Append(productQuantity);
        builder.Append(",");
        builder.Append(productPrice);
        return builder.ToString();
    }

    [SqlMethod(OnNullCall = false)]
    public static Product Parse(SqlString s)
    {
        if (s.IsNull)
            return Null;

        else
        {
            Product list = new Product();
            string[] productData = s.Value.Split(",".ToCharArray());
            list.productID = int.Parse(productData[0]);
            list.productName = productData[1].ToString();
            list.productQuantity = int.Parse(productData[2]);
            list.productPrice = decimal.Parse(productData[3]);

            if (!list.ValidateAllProduct())
                throw new ArgumentException("Invalid Product Values..");
            return list;
        }

    }

    public bool ValidateProduct()
    {
        if ((productID > 0) || (productQuantity > 0) || (productPrice > 0) || !String.IsNullOrEmpty(productName))
            return true;
        else
            return false;
    }

    public bool ValidateAllProduct()
    {
        if ((productID > 0) && (productQuantity > 0) && (productPrice > 0) && !String.IsNullOrEmpty(productName))
            return true;
        else
            return false;
    }

    public void Write(BinaryWriter w)
    {
        w.Write(productID);
        w.Write(productName);
        w.Write(productQuantity);
        w.Write(productPrice);
    }

    public void Read(BinaryReader r)
    {
        productID = r.ReadInt32();
        productName = r.ReadString();
        productQuantity = r.ReadInt32();
        productPrice = r.ReadDecimal();
    }
}