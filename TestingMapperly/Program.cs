using Riok.Mapperly.Abstractions;

namespace TestingMapperly
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dto=new ProductRequestDto()
            {
                Name = "Product 10",
                Price = 100,
                NumberOfItems = 10
            };
            var mapper = new ProductDtoToProductModelMapper();
            var product = mapper.Map(dto);
            
            Console.WriteLine($"{product}");

        }
    }


    [Mapper]
    internal partial class ProductDtoToProductModelMapper
    {
        [MapProperty(nameof(ProductRequestDto.NumberOfItems), nameof(Product.CurrentQuantity))]
        public partial Product Map(ProductRequestDto productRequestDto);
        
    }
}
