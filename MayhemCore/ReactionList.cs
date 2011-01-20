using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace MayhemCore
{
    public class ReactionList : ModuleList, IEnumerable<ActionBase>
    {
        List<ReactionBase> reactionList;

        public ReactionList()
        {
            reactionList = FindTypes<ReactionBase>(Application.StartupPath);
            foreach (var reaction in reactionList)
            {
                Console.WriteLine(reaction.Name);
            }
        }

        public IEnumerator GetEnumerator()
        {
            return reactionList.GetEnumerator();
        }


        IEnumerator<ActionBase> IEnumerable<ActionBase>.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
