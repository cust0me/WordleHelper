using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using WordleHelper.Web.Models;

namespace WordleHelper.Web.Components;

public partial class WordleLineComponent
{
    [Parameter]
    public Func<object,LetterState[], Task>? LetterStatesChanged { get; set; }

    public bool IsReadonly { get; private set; } = false;

    private LetterState[] _letterStates = [
        LetterState.Unknown,
        LetterState.Unknown,
        LetterState.Unknown,
        LetterState.Unknown,
        LetterState.Unknown
    ];

    public string Word { get; private set; } = string.Empty.PadRight(5);

    public async Task SetWordAsync(string word)
    {
        Word = word.PadRight(5);
        _letterStates = [.. _letterStates.Select(_ => LetterState.Unknown)];
        StateHasChanged();

        if (LetterStatesChanged is not null)
        {
            await LetterStatesChanged.Invoke(this, _letterStates);
        }
    }

    public async Task SetReadonlyAsync()
    {
        IsReadonly = true;
        _letterStates = [.. _letterStates.Select(_ => LetterState.Incorrect)];
        StateHasChanged();

        if (LetterStatesChanged is not null)
        {
            await LetterStatesChanged.Invoke(this, _letterStates);
        }
    }

    private async Task OnClick(MouseEventArgs mouseEventArgs, int index)
    {
        if (IsReadonly == false || mouseEventArgs is not { Button: 0 })
            return;

        _letterStates[index] = _letterStates[index] switch
        {
            LetterState.Incorrect => LetterState.WrongPosition,
            LetterState.WrongPosition => LetterState.Correct,
            LetterState.Correct => LetterState.Incorrect,
            _ => LetterState.Incorrect
        };

        if (LetterStatesChanged is not null)
        {
            await LetterStatesChanged.Invoke(this, _letterStates);
        }
    }

    private string GetLetterStateClass(int index)
    {
        var classes = _letterStates[index] switch
        {
            LetterState.Incorrect => "word-box__letter-absent",
            LetterState.WrongPosition => "word-box__letter-present",
            LetterState.Correct => "word-box__letter-correct",
            _ => string.Empty
        };

        if (IsReadonly)
        {
            classes += " word-box__letter-readonly";
        }

        return classes;
    }
}
