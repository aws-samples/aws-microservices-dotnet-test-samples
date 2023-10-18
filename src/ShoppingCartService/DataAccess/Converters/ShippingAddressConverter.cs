using System.Xml.Serialization;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using ShoppingCartService.BusinessLogic.Models;

namespace ShoppingCartService.DataAccess.Converters;

public class ShippingAddressConverter : IPropertyConverter
{
    private readonly XmlSerializer _serializer = new(typeof(ShippingAddress));
    
    public object? FromEntry(DynamoDBEntry entry)
    {
        if (entry is not Primitive primitive) return null;

        if (primitive.Type != DynamoDBEntryType.String) throw new InvalidCastException();
        var xml = primitive.AsString();
        
        using var reader = new StringReader(xml);
        return _serializer.Deserialize(reader);
    }

    public DynamoDBEntry? ToEntry(object value)
    {
        if (value is not ShippingAddress address) return null;

        string xml;
        using (var stringWriter = new StringWriter())
        {
            _serializer.Serialize(stringWriter, address);
            xml = stringWriter.ToString();
        }
        return new Primitive(xml);
    }
}