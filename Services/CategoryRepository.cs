using Microsoft.EntityFrameworkCore;
using PFM.Data;
using PFM.Backend.Database.Entities;
using PFM.Backend.Database.Entities.CategoriesDTO;
using PFM.Backend.Services;

public class CategoryRepository : ICategoryRepository
{
    private readonly TransactionDbContext _context;

    public CategoryRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task<(bool success, List<string> errors)> SaveCategoriesAsync(CreateCategoryListDTO dto)
    {
        var errors = new List<string>();
        var allInCsv = dto.Categories.Select(c => c.Code).ToHashSet();
        var existing = await _context.Categories.Select(c => c.Code).ToListAsync();
        var combined = allInCsv.Union(existing).ToHashSet();

        foreach (var cat in dto.Categories)
        {
            if (!string.IsNullOrWhiteSpace(cat.ParentCode) && !combined.Contains(cat.ParentCode))
                errors.Add($"Parent code not found: {cat.ParentCode} for {cat.Code}");
        }

        if (errors.Any()) return (false, errors);

        var root = dto.Categories.Where(c => string.IsNullOrWhiteSpace(c.ParentCode)).ToList();
        var children = dto.Categories.Where(c => !string.IsNullOrWhiteSpace(c.ParentCode)).ToList();

        foreach (var c in root)
        {
            var existingCat = await _context.Categories.FindAsync(c.Code);
            if (existingCat != null)
            {
                existingCat.Name = c.Name;
                _context.Categories.Update(existingCat);
            }
            else
            {
                _context.Categories.Add(new Category
                {
                    Code = c.Code,
                    Name = c.Name,
                    ParentCode = null
                });
            }
        }

        await _context.SaveChangesAsync();

        foreach (var c in children)
        {
            var existingCat = await _context.Categories.FindAsync(c.Code);
            if (existingCat != null)
            {
                existingCat.Name = c.Name;
                existingCat.ParentCode = c.ParentCode;
                _context.Categories.Update(existingCat);
            }
            else
            {
                _context.Categories.Add(new Category
                {
                    Code = c.Code,
                    Name = c.Name,
                    ParentCode = c.ParentCode
                });
            }
        }

        await _context.SaveChangesAsync();

        return (true, []);
    }
}
