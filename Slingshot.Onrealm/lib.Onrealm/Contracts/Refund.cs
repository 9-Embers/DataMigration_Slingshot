using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lib.Onrealm.Contracts;

public class Refund
{
    public string? PersonId { get; set; }
    public string? RefundedDate { get; set; }
    public string? BatchNumber { get; set; }

    public string? Account { get; set; }
    public string? Fund { get; set; }
    public string? RefundedAmount { get; set; }
}
