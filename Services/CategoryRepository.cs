using Microsoft.EntityFrameworkCore;
using PFM.Backend.Database.Entities;
using PFM.Backend.Database.Entities.CategoriesDTO;
using PFM.Backend.Models.Validation;
using PFM.Backend.Services;
using PFM.Data;


public class CategoryRepository : ICategoryRepository
{
    private readonly TransactionDbContext _context;

    public CategoryRepository(TransactionDbContext context)
    {
        _context = context;
    }

    public async Task<(bool success, List<ValidationError> errors)> SaveCategoriesAsync(CreateCategoryListDTO dto)
     {
         var errors = new List<ValidationError>();

         var allInCsv = dto.Categories.Select(c => c.Code).ToHashSet();
         var existing = await _context.Categories.Select(c => c.Code).ToListAsync();
         var combined = allInCsv.Union(existing).ToHashSet();

         foreach (var cat in dto.Categories)
         {
             if (!string.IsNullOrWhiteSpace(cat.ParentCode) && !combined.Contains(cat.ParentCode))
             {
                 errors.Add(new ValidationError(
                     tag: "parent-code",
                     error: "not-found",
                     message: $"Parent code not found: {cat.ParentCode} for {cat.Code}"
                 ));
             }

             if (existing.Contains(cat.Code))
             {
                 errors.Add(new ValidationError(
                     tag: "code",
                     error: "category-already-exists",
                     message: $"Category already exists: {cat.Code}"
                 ));
             }
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
