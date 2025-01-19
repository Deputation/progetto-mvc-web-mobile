using MediaJournal.Models.Entities;

namespace MediaJournal.Web.Models;

public class MediaFormViewModel
{
    public Media Media { get; set; } = null!;
    public string Action { get; set; } = "Create";
    public string Title => Action == "Create" ? "Add New Media" : "Edit Media";
    public bool IsEdit => Action == "Edit";
}