using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _5PaisaLibrary
{
    internal interface IWebsocket
    {
       
        void Connect(string jwttoken, string clientcode);
       
       
   
    }
}
