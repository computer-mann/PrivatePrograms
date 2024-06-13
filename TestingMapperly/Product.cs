using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingMapperly
{
    internal class Product
    {
        public string Id { get; set; } = Guid.NewGuid().ToString("N");
        public string Name { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now.ToLocalTime();
        public int CurrentQuantity { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Price: {Price}, CreatedAt: {CreatedAt}, CurrentQuantity: {CurrentQuantity}";
        }
    }
}
