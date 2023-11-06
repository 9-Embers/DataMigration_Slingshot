using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.Onrealm.Contracts;

public class Individual
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Image { get; set; }
    public bool HasAccount { get; set; }
    public bool IsDeactivated { get; set; }
    public string? DateMarkedDeceased { get; set; }
    public string? DeceasedDate { get; set; }
    public string? DeceasedAge { get; set; }
    public string? FullName { get; set; }
    public string? ChurchPosition { get; set; }
}
