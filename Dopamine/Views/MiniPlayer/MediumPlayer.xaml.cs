using Digimezzo.Foundation.WPF.Controls;
using Dopamine.Views.Common.Base;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Dopamine.Views.MiniPlayer
{
    /// <summary>
    /// Interaction logic for MediumPlayer.xaml
    /// </summary>
    public partial class MediumPlayer : MiniPlayerViewBase
    {
        public MediumPlayer()
        {
            InitializeComponent();
        }

        protected void CoverGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.MouseLeftButtonDownHandler(sender, e);
        }
    }
}
