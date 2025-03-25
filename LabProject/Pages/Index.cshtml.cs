using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using LabProject.Models;
using System.Collections.Generic;
using System.Linq;

namespace LabProject.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new();

        [BindProperty]
        public ClassInformationModel NewClass { get; set; } = new();

        [BindProperty]
        public int? EditId { get; set; }

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
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (EditId.HasValue)
            {
                var existingClass = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
                if (existingClass != null)
                {
                    existingClass.ClassName = NewClass.ClassName;
                    existingClass.StudentCount = NewClass.StudentCount;
                    existingClass.Description = NewClass.Description;
                }
            }
            else
            {
                ClassList.Add(new ClassInformationModel
                {
                    ClassName = NewClass.ClassName,
                    StudentCount = NewClass.StudentCount,
                    Description = NewClass.Description
                });
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassList.Remove(item);
            }

            return RedirectToPage();
        }
    }
}
