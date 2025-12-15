
namespace BenzenyMain.Domain.Entity
{
    public class CarModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Car> Cars { get; set; } = new List<Car>();
    }
}
