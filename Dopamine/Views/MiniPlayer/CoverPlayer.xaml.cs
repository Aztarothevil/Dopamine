using Dopamine.Views.Common.Base;
using System;
using System.Windows;
using System.Windows.Input;

namespace Dopamine.Views.MiniPlayer
{
    public partial class CoverPlayer : MiniPlayerViewBase
    {
        public CoverPlayer()
        {
            InitializeComponent();
        }

        protected void CoverGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseLeftButtonDownHandler(sender, e);
        }
    }
}
