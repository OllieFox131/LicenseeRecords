using LicenseeRecords.Models;

namespace LicenseeRecords.Web.Models;
public class ProductLicenceCreateEditModel
{
	public ProductLicence ProductLicence { get; set; } = new();
	public Product[] Products { get; set; } = [];
}
