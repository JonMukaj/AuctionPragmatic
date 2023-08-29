using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models;
public class Auction : BaseCreatedAndModified
{
    [Key] public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public decimal StartingBid { get; set; }
    public decimal? CurrentBid { get; set; }
    public decimal? MaxBid { get; set; }
    public bool IsEnded { get; set; }

    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public ApplicationUser? User { get; set; }
    public virtual IEnumerable<Bid>? Bids { get; set; }
}

