using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutofacDIExample.Models
{
    public class AlternateGoodbyer : IGoodbyer
    {
        public string Goodbye()
        {
            return "Farewell!";
        }
    }
}
