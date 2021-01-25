using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UwuForms
{
    public class Editor
    {
        List<string> lines;
        int nlines;
        int top;
        Terminal term;
      
        public Editor(string body, Terminal t)
        {
            term = t;
            top = 0;
            lines = new List<string>();
            lines.AddRange(body.Split('\n'));
            nlines = lines.Count;
        }

        public void Repaint()
        {
            //term.ClearScreen();
            if (top >= nlines) return;
            for (int i = top; i < lines.Count; i++)
            {
                term.Print(lines[i]);
                term.Print("\r\n");
            }
        }
    }
}
