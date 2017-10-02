using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DryIoc;
using Moq;
using RemoteFileDialog.Entries;
using RemoteFileDialog.Services;

namespace RemoteFileDialog.Utility
{
    public static class DesignerObjects
    {
        private static readonly IContainer _container;

        static DesignerObjects()
        {
            _container = new Container();
            _container.Register<IEntryViewModel, EntryViewModel>();
            _container.UseInstance<ICollection<Entry>>(new List<Entry>());
            _container.Register<IRemoteFileDialogViewModel, RemoteFileDialogViewModel>();


            var entryServiceStub = new Mock<IEntryService>();

            var rootEntries = new List<Entry>
            {
                new Entry
                {
                    Name = "D",
                    IsDirectory = true,
                    Path = "D:\\"
                }

            };
            entryServiceStub.Setup(entryService => entryService.LoadRootEntries()).Returns(rootEntries);

            entryServiceStub
                .Setup(entryService => entryService.LoadChildEntriesAsync(It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(() => new List<Entry>
                {
                    new Entry
                    {
                        Name = "qwe",
                        IsDirectory = false,
                        Path = "D:\\qwe"
                    },
                    new Entry
                    {
                        Name = "asd",
                        IsDirectory = false,
                        Path = "D:\\asd"
                    }
                });

            _container.UseInstance(entryServiceStub.Object);


            DeisgnerRemoteFileDialogViewModel = _container.Resolve<IRemoteFileDialogViewModel>();
            //DeisgnerRemoteFileDialogViewModel.RootEntryViewModels.ElementAt(0).IsExpanded = true;
        }

        public static IRemoteFileDialogViewModel DeisgnerRemoteFileDialogViewModel { get; }
    }
}