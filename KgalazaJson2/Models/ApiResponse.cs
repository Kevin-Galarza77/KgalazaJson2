using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KgalazaJson2.Models
{
    public class ApiResponse<T>
    {
        public List<string> messages { get; set; }
        public T data { get; set; }
        public string alert { get; set; }
        public bool status { get; set; }
    }
}
