using System.ComponentModel.DataAnnotations;

namespace DailyDo.Models
{
    public class TaskAndCategoryVM
    {
       public Task Task { get; set; }

       public List<Category> Categories { get; set; }
    }
}
