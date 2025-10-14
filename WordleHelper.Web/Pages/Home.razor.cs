using Microsoft.AspNetCore.Components.Web;
using System.Collections.ObjectModel;
using WordleHelper.Core.Services;
using WordleHelper.Web.Components;
using WordleHelper.Web.Models;

namespace WordleHelper.Web.Pages;

public partial class Home(IWordlePredictionService wordlePredictionService, IWordStorageService wordStorageService)
{
    private readonly IWordlePredictionService _wordlePredictionService = wordlePredictionService ?? throw new ArgumentNullException(nameof(wordlePredictionService));
    private readonly IWordStorageService _wordStorageService = wordStorageService ?? throw new ArgumentNullException(nameof(wordStorageService));

    public ObservableCollection<string> PredictionStrings = [];

    public WordleLineComponent? CurrentWordleLineComponent;

    public WordleLineComponent? WordleLineComponent1;
    public LetterState[]? LetterStates1;

    public WordleLineComponent? WordleLineComponent2;
    public LetterState[]? LetterStates2;

    public WordleLineComponent? WordleLineComponent3;
    public LetterState[]? LetterStates3;

    public WordleLineComponent? WordleLineComponent4;
    public LetterState[]? LetterStates4;

    public WordleLineComponent? WordleLineComponent5;
    public LetterState[]? LetterStates5;

    public WordleLineComponent? WordleLineComponent6;
    public LetterState[]? LetterStates6;

    public string WrittenText { get; set; } = string.Empty;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            CurrentWordleLineComponent = WordleLineComponent1;
            string[] words = await _wordStorageService.GetWordsAsync();
            PredictionStrings.Clear();
            words.ToList().ForEach(PredictionStrings.Add);
            StateHasChanged();
        }
    }

    public async Task OnInputChanged()
    {
        for (int i = WrittenText.Length - 1; i >= 0; i--)
        {
            if (char.IsAsciiLetter(WrittenText[i]) == false)
            {
                WrittenText = WrittenText.Remove(i);
            }
        }

        if (CurrentWordleLineComponent is not null)
        {
            await CurrentWordleLineComponent.SetWordAsync(WrittenText.PadRight(5));
        }
    }

    private async Task OnInputKeyDown(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            if (WrittenText.Trim().Length != 5 || PredictionStrings.Contains(WrittenText) == false)
            {
                return;
            }

            if (CurrentWordleLineComponent is not null)
            {
                await CurrentWordleLineComponent.SetReadonlyAsync();
            }

            if (ReferenceEquals(CurrentWordleLineComponent, WordleLineComponent1))
            {
                CurrentWordleLineComponent = WordleLineComponent2;
            }
            else if (ReferenceEquals(CurrentWordleLineComponent, WordleLineComponent2))
            {
                CurrentWordleLineComponent = WordleLineComponent3;
            }
            else if (ReferenceEquals(CurrentWordleLineComponent, WordleLineComponent3))
            {
                CurrentWordleLineComponent = WordleLineComponent4;
            }
            else if (ReferenceEquals(CurrentWordleLineComponent, WordleLineComponent4))
            {
                CurrentWordleLineComponent = WordleLineComponent5;
            }
            else if (ReferenceEquals(CurrentWordleLineComponent, WordleLineComponent5))
            {
                CurrentWordleLineComponent = WordleLineComponent6;
            }
            else if (ReferenceEquals(CurrentWordleLineComponent, WordleLineComponent6))
            {
                CurrentWordleLineComponent = null;
            }

            if (CurrentWordleLineComponent is not null)
            {
                WrittenText = string.Empty;
            }
        }
    }

    private async Task OnLetterStatesChanged(object sender, LetterState[] letterStates)
    {
        if (ReferenceEquals(sender, WordleLineComponent1))
        {
            LetterStates1 = letterStates;
        }
        else if (ReferenceEquals(sender, WordleLineComponent2))
        {
            LetterStates2 = letterStates;
        }
        else if (ReferenceEquals(sender, WordleLineComponent3))
        {
            LetterStates3 = letterStates;
        }
        else if (ReferenceEquals(sender, WordleLineComponent4))
        {
            LetterStates4 = letterStates;
        }
        else if (ReferenceEquals(sender, WordleLineComponent5))
        {
            LetterStates5 = letterStates;
        }
        else if (ReferenceEquals(sender, WordleLineComponent6))
        {
            LetterStates6 = letterStates;
        }

        await PredictWordsAsync();
    }

    private async Task PredictWordsAsync()
    {
        char[] blacklist = [];

        char[][] wrongPositions =
        [
            [],
            [],
            [],
            [],
            [],
        ];

        char[] mustInclude = [

        ];

        char[] known = [
            ' ',
            ' ',
            ' ',
            ' ',
            ' '
        ];

        LetterState[][] allLetterStates = [
            LetterStates1 ?? [.. Enumerable.Range(0, 5).Select(e => LetterState.Unknown)],
            LetterStates2 ?? [.. Enumerable.Range(0, 5).Select(e => LetterState.Unknown)],
            LetterStates3 ?? [.. Enumerable.Range(0, 5).Select(e => LetterState.Unknown)],
            LetterStates4 ?? [.. Enumerable.Range(0, 5).Select(e => LetterState.Unknown)],
            LetterStates5 ?? [.. Enumerable.Range(0, 5).Select(e => LetterState.Unknown)],
            LetterStates6 ?? [.. Enumerable.Range(0, 5).Select(e => LetterState.Unknown)],
        ];

        string[] allWords = [
            WordleLineComponent1?.Word!,
            WordleLineComponent2?.Word!,
            WordleLineComponent3?.Word!,
            WordleLineComponent4?.Word!,
            WordleLineComponent5?.Word!,
            WordleLineComponent6?.Word!,
        ];

        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                char letter = allWords[i][j];
                switch (allLetterStates[i][j])
                {
                    case LetterState.Correct:
                        known[j] = letter;
                        mustInclude = [.. mustInclude, letter];
                        break;
                    case LetterState.WrongPosition:
                        if (wrongPositions[j].Contains(letter) == false)
                        {
                            wrongPositions[j] = [.. wrongPositions[j], letter];
                        }

                        if (mustInclude.Contains(letter) == false)
                        {
                            mustInclude = [.. mustInclude, letter];
                        }
                        break;
                    case LetterState.Incorrect:
                        if (blacklist.Contains(letter) == false && mustInclude.Contains(letter) == false)
                        {
                            blacklist = [.. blacklist, letter];
                        }
                        break;
                    case LetterState.Unknown:
                    default:
                        break;
                }
            }
        }

        List<string> result = await _wordlePredictionService.GetPredictions(known, wrongPositions, mustInclude, blacklist);
        PredictionStrings.Clear();
        result.ForEach(PredictionStrings.Add);
        StateHasChanged();
    }
}