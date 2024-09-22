using LicenseeRecords.Models;

namespace LicenseeRecords.Web.Models;
public class HomeViewModel
{
    public Account[]? Accounts { get; set; }
    public Product[]? Products { get; set; }
}
