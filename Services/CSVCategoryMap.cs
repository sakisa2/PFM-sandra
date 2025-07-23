using CsvHelper.Configuration;
using PFM.Backend.Database.Entities.CategoriesDTO;

public class CSVCategoryMap : ClassMap<CreateCategoryDTO>
{
    public CSVCategoryMap()
    {
        Map(m => m.Code).Name("code");
        Map(m => m.Name).Name("name");
        Map(m => m.ParentCode).Name("parent-code");
    }
}