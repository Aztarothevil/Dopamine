using Dopamine.Views.Common.Base;
using System;
using System.Windows;
using System.Windows.Input;


namespace Dopamine.Views.MiniPlayer
{
    public partial class MicroPlayer : MiniPlayerViewBase
    {
        public MicroPlayer()
        {
            InitializeComponent();
        }

        private void CoverGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseLeftButtonDownHandler(sender, e);
        }
    }
}
