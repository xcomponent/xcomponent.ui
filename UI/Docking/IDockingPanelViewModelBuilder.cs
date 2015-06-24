using System.IO;

namespace XComponent.Common.UI.Docking
{
    public interface IDockingPanelViewModelBuilder
    {
        object CreateViewModel(TextReader reader);

        void SaveViewModel(object viewModel, TextWriter writer);

        string MigrateParameters(string parameters, int layoutVersion);
    }
}
