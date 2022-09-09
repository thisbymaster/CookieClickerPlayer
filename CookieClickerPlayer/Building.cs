using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CookieClickerPlayer
{
    class Building
    {
        public Building()
        {
        }
        //public Building(int Index, decimal Percent, int Total)
        //{
        //    index = Index;
        //    percent = Percent;
        //    totalbought = Total;
        //}
        public Building(int Index, string tooltiphtml)
        {
            index = Index;
            totalbought = int.Parse(tooltiphtml.Substring(tooltiphtml.IndexOf("</div><small>[owned : ") + 22, tooltiphtml.IndexOf("</small>]<div class=\"line\"></div>") - (tooltiphtml.IndexOf("</div><small>[owned : ") + 22)));
            percent = totalbought==0?0:Decimal.Parse(tooltiphtml.Substring(tooltiphtml.IndexOf(" cookies per second (<b>")+24 , tooltiphtml.IndexOf("%</b> of total CpS)") -(tooltiphtml.IndexOf(" cookies per second (<b>") + 24)) );
            
        }
        public int index;
        public decimal percent;
        public int totalbought;
        public decimal Weight { get { return percent / totalbought; } }
    }
}
