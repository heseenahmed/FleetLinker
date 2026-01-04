using FleetLinker.Domain.Entity;

namespace FleetLinker.Domain.IRepository
{
    public interface IEquipmentRepository : IBaseRepository<Equipment>
    {
        Microsoft.EntityFrameworkCore.DbSet<Equipment> _dbSet { get; }
    }
}
