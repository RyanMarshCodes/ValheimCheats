using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValehimOverlay
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var overlay = new Overlay())
            {
                overlay.Run();
            }
        }
    }
}
