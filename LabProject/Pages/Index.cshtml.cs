using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LabProject.Helpers;
using System.Threading.Tasks;

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

        [BindProperty(SupportsGet = true)]
        public string? SelectedRowIds { get; set; }

        public List<int> PersistedSelectedRowIds { get; set; } = new();

        private bool IsUserAuthenticated()
        {
            return HttpContext.Session.GetString("username") != null &&
                   Request.Cookies["username"] == HttpContext.Session.GetString("username") &&
                   Request.Cookies["token"] == HttpContext.Session.GetString("token") &&
                   Request.Cookies["session_id"] == HttpContext.Session.GetString("session_id");
        }

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

        public async Task<IActionResult> OnGetAsync(int? editId)
        {
            if (!IsUserAuthenticated())
            {
                TempData["ErrorMessage"] = "Username or password is incorrect.";
                return RedirectToPage("/Login");
            }

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

            if (!FilteredClassList.Any())
            {
                FilteredClassList = query.ToList();
            }

            if (!string.IsNullOrWhiteSpace(SelectedRowIds))
            {
                PersistedSelectedRowIds = SelectedRowIds
                    .Split(',')
                    .Select(idStr => int.TryParse(idStr, out int id) ? id : -1)
                    .Where(id => id > 0)
                    .ToList();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!IsUserAuthenticated())
            {
                TempData["ErrorMessage"] = "Username or password is incorrect.";
                return RedirectToPage("/Login");
            }

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

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            if (!IsUserAuthenticated())
            {
                TempData["ErrorMessage"] = "Username or password is incorrect.";
                return RedirectToPage("/Login");
            }

            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
                ClassList.Remove(item);

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportAsync(string SelectedColumns, string SelectedRowIds)
        {
            if (!IsUserAuthenticated())
            {
                TempData["ErrorMessage"] = "Username or password is incorrect.";
                return RedirectToPage("/Login");
            }

            List<string> columns = string.IsNullOrWhiteSpace(SelectedColumns)
                ? new List<string> { "Id", "ClassName", "StudentCount", "Description" }
                : SelectedColumns.Split(',').ToList();

            List<int> selectedIds = string.IsNullOrWhiteSpace(SelectedRowIds)
                ? new List<int>()
                : SelectedRowIds.Split(',').Select(int.Parse).ToList();

            List<ClassInformationModel> selectedData = selectedIds.Any()
                ? ClassList.Where(c => selectedIds.Contains(c.Id)).ToList()
                : ClassList.ToList();

            var exportData = selectedData.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();

            string json = Utils.Instance.ExportToJson(exportData, columns);
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            return File(jsonBytes, "application/json", "class_export.json");
        }
    }
}