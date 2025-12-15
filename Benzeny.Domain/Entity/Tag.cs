
namespace BenzenyMain.Domain.Entity
{
    public class Tag
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<Driver> Drivers { get; set; } = new List<Driver>();
    }
}
