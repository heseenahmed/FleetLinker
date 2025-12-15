
using Benzeny.Infra.Data;
using Benzeny.Infra.Repository;
using BenzenyMain.Domain.Entity;
using BenzenyMain.Domain.Entity.Dto.Tag;
using BenzenyMain.Domain.IRepository;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using System;

namespace BenzenyMain.Infra.Repository
{
    public class TagRepository : BaseRepository<Tag> , ITagRepository
    {
        private readonly ApplicationDbContext _context;
        public TagRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<TagLookupDto>> GetAllAsync(CancellationToken ct)
        {
            // Assuming Tag entity has Id (int), Name (string)
            return await _context.Tags
                .AsNoTracking()
                .OrderBy(t => t.Title)
                .Select(t => new TagLookupDto { Id = t.Id, Name = t.Title })
                .ToListAsync(ct);
        }
    }
}
