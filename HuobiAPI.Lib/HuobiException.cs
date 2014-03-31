using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HuobiAPI.Lib
{
    public class HuobiException : Exception 
    {
        public string Code;
        public string Text;

        public HuobiException(string _code,string _text)
        {
            this.Code = _code;
            this.Text = _text;
        }
    }
}
