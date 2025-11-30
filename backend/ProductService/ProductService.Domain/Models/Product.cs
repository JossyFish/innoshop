using ProductService.Domain.Enums;
using ProductService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Domain.Models
{
    public class Product
    {
        public Guid Id { get; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public Price Price { get; private set; }
        public int Quantity { get; private set; }
        public bool IsActive { get; private set; }
        public Guid UserId { get; }
        public DateTime CreatedAt { get; }

        public Product(Guid id, string name, string description, Price price, int quantity, bool isActive, Guid userId, DateTime createdAt)
        {
            Id = id;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            IsActive = isActive;
            UserId = userId;
            CreatedAt = createdAt;
        }
        public static Product Create(Guid id, string name, string description, Price price, int quantity, bool isActive, Guid userId,
            DateTime createdAt)
        {
            return new Product(id, name, description, price, quantity, isActive, userId, createdAt);
        }


        public void ChangeName(string newName)
        {
            Name = newName;
        }

        public void ChangeDescription(string newDescription)
        {
            Description = newDescription;
        }

        public void ChangePrice(decimal newCost, Currency newCurrency)
        {
            Price = new Price(newCost, newCurrency);
        }

        public void ChangeQuantity(int newQuantity)
        {
            Quantity = newQuantity;
        }

        public void ChangeActive(bool isActive)
        {
            IsActive = isActive;
        }
    }
}
