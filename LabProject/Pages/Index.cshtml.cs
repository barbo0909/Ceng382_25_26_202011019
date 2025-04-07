using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace LabProject.Pages
{
    public class ClassInformationModel
    {
        public int Id { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string? Description { get; set; }
    }

    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        public List<ClassInformationTable> FilteredClassList { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchClassName { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? MinStudentCount { get; set; }

        [BindProperty(SupportsGet = true)]
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        static IndexModel()
        {
            
            for (int i = 1; i <= 100; i++)
            {
                ClassList.Add(new ClassInformationModel
                {
                    Id = i,
                    ClassName = $"Class {i}",
                    StudentCount = 10 + (i % 30),
                    Description = $"Sample description for Class {i}"
                });
            }
        }

        public void OnGet(int? editId)
        {
            if (editId.HasValue)
            {
                var classToEdit = ClassList.FirstOrDefault(c => c.Id == editId.Value);
                if (classToEdit != null)
                {
                    NewClass = new ClassInformationModel
                    {
                        ClassName = classToEdit.ClassName,
                        StudentCount = classToEdit.StudentCount,
                        Description = classToEdit.Description
                    };
                    EditId = editId;
                }
            }

            var query = ClassList.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            });

            if (!string.IsNullOrWhiteSpace(SearchClassName))
            {
                query = query.Where(c => c.ClassName.Contains(SearchClassName, System.StringComparison.OrdinalIgnoreCase));
            }

            if (MinStudentCount.HasValue)
            {
                query = query.Where(c => c.StudentCount >= MinStudentCount.Value);
            }

            int totalItems = query.Count();
            TotalPages = (int)System.Math.Ceiling(totalItems / (double)PageSize);

            FilteredClassList = query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            if (EditId.HasValue)
            {
                var existing = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                if (existing != null)
                {
                    existing.ClassName = NewClass.ClassName;
                    existing.StudentCount = NewClass.StudentCount;
                    existing.Description = NewClass.Description;
                }
            }
            else
            {
                int nextId = ClassList.Any() ? ClassList.Max(c => c.Id) + 1 : 1;
                NewClass.Id = nextId;
                ClassList.Add(NewClass);
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                ClassList.Remove(item);

            return RedirectToPage();
        }
    }
}
