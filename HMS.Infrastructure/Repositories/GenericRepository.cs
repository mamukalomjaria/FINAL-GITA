using HMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    private readonly DbSet<T> _db;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context;
        _db = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _db.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _db.ToListAsync();
    }

    public async Task AddAsync(T entity)
    {
        await _db.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _db.Update(entity);
    }

    public void Delete(T entity)
    {
        _db.Remove(entity);
    }

    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }
}