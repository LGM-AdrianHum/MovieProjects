using System.Windows.Automation;
using System.Windows.Automation.Provider;
using System.Windows.Controls;

namespace TvFileBot.Controls
{
    public class ScrollableDataGrid : DataGrid
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private IScrollProvider _mScrollProvider;
        public ScrollableDataGrid()
        {
            // ReSharper disable once VirtualMemberCallInConstructor
            _mScrollProvider = OnCreateAutomationPeer() as IScrollProvider;
        }
        public void ScrollToBottom()
        {
            while (_mScrollProvider?.VerticalScrollPercent < 100)
            {
                _mScrollProvider.Scroll(ScrollAmount.NoAmount, ScrollAmount.LargeIncrement);
            }
        }
    }
}
