using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Reflection;

namespace MayhemCore
{
    public class ActionList : ModuleList, IEnumerable<ActionBase>
    {
        List<ActionBase> actionList;

        public ActionList()
        {
            actionList = FindTypes<ActionBase>(Application.StartupPath);
            foreach (var action in actionList)
            {
                Console.WriteLine(action.Name);
            }
            Console.WriteLine("--------");

        }

        public IEnumerator GetEnumerator()
        {
            return actionList.GetEnumerator();
        }

        IEnumerator<ActionBase> IEnumerable<ActionBase>.GetEnumerator()
        {
            return actionList.GetEnumerator();
        }
    }
}
