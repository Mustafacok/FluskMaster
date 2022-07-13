using FluskMasterUpdate.m2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FluskMasterUpdate.Models
{
    public class PortfolyoBLL : GenericRepository<Portfolyo>
    {
    }
    public partial class Portfolyo
    {
        public string SEOLink
        {
            get
            {
                return GenelAraclarBLL.Duzelt(Adi);
            }
        }
    }
}