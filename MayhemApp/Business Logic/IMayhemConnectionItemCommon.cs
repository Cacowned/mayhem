using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace MayhemApp.Business_Logic
{
    /**<summary>
 *  Common properties of Mayhem events and actions
 * </summary>*/
    public interface IMayhemConnectionItemCommon
    {
        void OnDoubleClick(object sender, MouseEventArgs e);
    }
}
