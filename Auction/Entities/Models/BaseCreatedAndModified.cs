using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class BaseCreatedAndModified
{
    public DateTime DateCreated { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? DateModified { get; set; }
    public int? ModifiedBy { get; set; }

    [NotMapped]
    public ApplicationUser? Created { get; set; }
    [NotMapped]
    public ApplicationUser? Modified { get; set; }
}