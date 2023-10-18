using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InventoryService.DataAccess.Entities;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;
    
    public double Price { get; set; }
    
    public uint Quantity { get; set; }

    protected bool Equals(Product other)
    {
        return Id == other.Id && Name == other.Name && Price.Equals(other.Price) && Quantity == other.Quantity;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Product)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id, Name, Price, Quantity);
    }

    public override string ToString()
    {
        return $"{nameof(Id)}: {Id}, {nameof(Name)}: {Name}, {nameof(Price)}: {Price}, {nameof(Quantity)}: {Quantity}";
    }
}
