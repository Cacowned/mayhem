using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemApp.Business_Logic.Facebook
{
    class MayhemFacebook
    {
        private static MayhemFacebook _instance = null;

        public static MayhemFacebook Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MayhemFacebook();
                }
                return _instance;
            }

        }

        public static string TAG = "[MayhemFacebook] :";

        // client ID: 156807084372094
        // client secret: fb062aed34fc9a5bab5698fb921e807b
    }
}
