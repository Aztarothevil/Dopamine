using Digimezzo.Foundation.Core.Utils;
using Dopamine.Core.Base;
using Dopamine.Services.JumpList;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;

namespace Dopamine.Services.JumpList
{
    public class JumpListService : IJumpListService
    {
        private System.Windows.Shell.JumpList jumpList;
      
        public JumpListService()
        {
            this.jumpList = System.Windows.Shell.JumpList.GetJumpList(Application.Current);
        }
       
        public async void PopulateJumpListAsync()
        {
            await Task.Run(() =>
            {
                if (this.jumpList != null)
                {
                    this.jumpList.JumpItems.Clear();
                    this.jumpList.ShowFrequentCategory = false;
                    this.jumpList.ShowRecentCategory = false;
                }

            });

            if (this.jumpList != null) this.jumpList.Apply();
        }
    }
}
