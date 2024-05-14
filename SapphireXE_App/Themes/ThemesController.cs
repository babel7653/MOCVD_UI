using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SapphireXE_App.Themes
{
    public partial class ThemesController : Component
    {
        public ThemesController()
        {
            InitializeComponent();
        }

        public ThemesController(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }
    }
}
