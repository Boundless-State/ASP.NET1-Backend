using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Dtos;

public class StatusFormData
{
    public int Id { get; set; }
    public string StatusName { get; set; } = null!;
}
