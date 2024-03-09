using Dopamine.Data.Entities;
using Dopamine.Services.Utils;
using Prism.Mvvm;

namespace Dopamine.Services.Entities
{
    public class FolderViewModel : BindableBase, ISemanticZoomable
    {
        private Folder folder;

        public FolderViewModel(Folder folder, bool isHeader)
        {
            this.folder = folder;
            this.isHeader = isHeader;
        }

        public Folder Folder => this.folder;

        public string Path => this.folder.Path;

        public string SafePath => this.folder.SafePath;

        public long FolderId => this.folder.FolderID;
       
        public string Directory => System.IO.Path.GetFileName(this.folder.Path);

        public bool ShowInCollection
        {
            get { return this.folder.ShowInCollection == 1 ? true : false; }

            set
            {
                this.folder.ShowInCollection = value ? 1 : 0;
                RaisePropertyChanged(nameof(this.ShowInCollection));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !GetType().Equals(obj.GetType()))
            {
                return false;
            }

            return string.Equals(this.Directory, ((FolderViewModel)obj).Directory);
        }

        public override int GetHashCode()
        {
            return this.Path.GetHashCode();
        }

        public override string ToString()
        {
            return this.Directory; 
        }

        private bool isHeader;

        public bool IsHeader
        {
            get { return this.isHeader; }
            set { SetProperty<bool>(ref this.isHeader, value); }
        }

        public string Header => SemanticZoomUtils.GetGroupHeader(this.Directory, true);
    }
}
