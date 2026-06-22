using System;
using R3;

namespace Gameplay.NodeSelection.UI
{
    public class MenuPanelViewModel : IDisposable
    {
        public string PlayerName { get;}
        public ReadOnlyReactiveProperty<int> RunNumber { get; }
        public ReadOnlyReactiveProperty<string> TurnText { get; }
        public ReadOnlyReactiveProperty<string> ClockText { get; }
        
        
        public MenuPanelViewModel(string playerName,RunState runState)
        {
            PlayerName = playerName;
            RunNumber = runState.RunNumber.ToReadOnlyReactiveProperty();
            
            TurnText = new ReactiveProperty<string>(string.Empty).ToReadOnlyReactiveProperty();
            ClockText = Observable
                .Interval(TimeSpan.FromSeconds(1))
                .Select(_ => DateTime.Now.ToString("HH:mm"))
                .ToReadOnlyReactiveProperty(DateTime.Now.ToString("HH:mm"));
            
        }

        public void Dispose()
        {
            RunNumber.Dispose();
            TurnText.Dispose();
            ClockText.Dispose();
        }
    }
}
