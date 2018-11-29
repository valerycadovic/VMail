using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMail.NewVMail.Views
{

    public class ListViewMenuItem
    {
        public ListViewMenuItem()
        {
            TargetType = typeof(ListViewDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }

        public Type TargetType { get; set; }
    }
}