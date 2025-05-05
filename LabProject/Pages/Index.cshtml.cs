using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using LabProject.Data;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Linq;
 using LabProject.Helpers; 

namespace LabProject.Pages
{
    public class IndexModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public IndexModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Class NewClass { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

        public List<Class> FilteredClassList { get; set; } = new();

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

        public async Task<IActionResult> OnGetAsync(int? editId)
        {
            if (!IsUserAuthenticated())
            {
                TempData["ErrorMessage"] = "Username or password is incorrect.";
                return RedirectToPage("/Login");
            }

            var query = _context.Classes.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchClassName))
                query = query.Where(c => c.Name.Contains(SearchClassName));

            if (MinStudentCount.HasValue)
                query = query.Where(c => c.PersonCount >= MinStudentCount.Value);

            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            FilteredClassList = await query
                .Skip((PageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            if (editId.HasValue)
            {
                var classToEdit = await _context.Classes.FindAsync(editId.Value);
                if (classToEdit != null)
                {
                    NewClass = new Class
                    {
                        Id = classToEdit.Id,
                        Name = classToEdit.Name,
                        PersonCount = classToEdit.PersonCount,
                        Description = classToEdit.Description,
                        IsActive = classToEdit.IsActive
                    };
                    EditId = editId;
                }
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
                var existing = await _context.Classes.FindAsync(EditId.Value);
                if (existing != null)
                {
                    existing.Name = NewClass.Name;
                    existing.PersonCount = NewClass.PersonCount;
                    existing.Description = NewClass.Description;
                    existing.IsActive = NewClass.IsActive;
                    await _context.SaveChangesAsync();
                }
            }
            else
{
    var newClass = new Class
    {
        Name = NewClass.Name,
        PersonCount = NewClass.PersonCount, 
        Description = NewClass.Description,
        IsActive = NewClass.IsActive 
    };

                _context.Classes.Add(newClass);
                await _context.SaveChangesAsync();
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

            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                _context.Classes.Remove(item);
                await _context.SaveChangesAsync();
            }

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
                ? new List<string> { "Id", "Name", "PersonCount", "Description", "IsActive" }
                : SelectedColumns.Split(',').ToList();

            List<int> selectedIds = string.IsNullOrWhiteSpace(SelectedRowIds)
                ? new List<int>()
                : SelectedRowIds.Split(',').Select(int.Parse).ToList();

            var query = _context.Classes.AsQueryable();

            if (selectedIds.Any())
                query = query.Where(c => selectedIds.Contains(c.Id));

            var exportData = await query.ToListAsync();

            string json = Utils.Instance.ExportToJson(exportData, columns); // TODO: Utils sınıfını kontrol et
            byte[] jsonBytes = Encoding.UTF8.GetBytes(json);
            return File(jsonBytes, "application/json", "class_export.json");
        }
    }
}