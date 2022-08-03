using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Model;

public class Promotion
{
    public string Id { get; set; } = default!;
    public PromotionType PromotionType { get; set; } = default!;

    [StringLength(30)]
    public string? Description { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; } = DateTime.Now;

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

    public List<string> ListSelectedStore { get; set; } = default!;
    public List<string> ListItem { get; set; } = default!;
}
