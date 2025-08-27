using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonadCraft;

namespace RollCraft.Avalonia.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EvaluateCommand))]
    private string? _diceExpression;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(EvaluateCommand))]
    private int _repeatInput = 1;

    [ObservableProperty]
    private ObservableCollection<EvaluationResultViewModel> _results = new();

    [ObservableProperty]
    private string? _errorText;

    private readonly DiceExpressionEvaluator<double> _randomEvaluator;
    private readonly DiceExpressionEvaluator<double> _minimumEvaluator;
    private readonly DiceExpressionEvaluator<double> _maximumEvaluator;
    private readonly DiceExpressionEvaluator<double> _fixedAverageEvaluator;

    public MainViewModel()
    {
        _randomEvaluator = DiceExpressionEvaluator<double>.CreateRandom();
        _minimumEvaluator = DiceExpressionEvaluator<double>.CreateMinimum();
        _maximumEvaluator = DiceExpressionEvaluator<double>.CreateMaximum();
        _fixedAverageEvaluator = DiceExpressionEvaluator<double>.CreateFixedAverage();
    }

    [RelayCommand(CanExecute = nameof(CanEvaluate))]
    private void Evaluate()
    {
        ErrorText = null;
        Results.Clear();

        for (var i = 0; i < RepeatInput; i++)
        {
            var diceExpression = DiceExpressionParser.Parse<double>(DiceExpression!);

            var resultViewModel = new EvaluationResultViewModel();

            EvaluateAndAdd(resultViewModel, "Result", _randomEvaluator, diceExpression);
            EvaluateAndAdd(resultViewModel, "Minimum", _minimumEvaluator, diceExpression);
            EvaluateAndAdd(resultViewModel, "Maximum", _maximumEvaluator, diceExpression);
            EvaluateAndAdd(resultViewModel, "Fixed Average", _fixedAverageEvaluator, diceExpression);
            
            if (string.IsNullOrEmpty(resultViewModel.Error))
            {
                Results.Add(resultViewModel);
            }
            else
            {
                ErrorText = resultViewModel.Error;
                break; 
            }
        }
    }

    private void EvaluateAndAdd(
        EvaluationResultViewModel resultViewModel, 
        string category, 
        DiceExpressionEvaluator<double> evaluator, 
        Result<IRollError, DiceExpression<double>> expression)
    {
        evaluator.Evaluate(expression)
            .Switch(
                success => resultViewModel.AddRow(
                    category, 
                    success.Result.ToString("0.######"), 
                    $"[{string.Join(", ", success.Rolls)}]"),
                failure => resultViewModel.Error = $"Failed on {category}: {failure.Message}"
            );
    }

    private bool CanEvaluate()
    {
        return !string.IsNullOrWhiteSpace(DiceExpression) && RepeatInput > 0;
    }

    [RelayCommand]
    private void Clear()
    {
        DiceExpression = string.Empty;
        RepeatInput = 1;
        Results.Clear();
        ErrorText = null;
    }
}

public partial class EvaluationResultViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ResultRowViewModel> _rows = new();

    [ObservableProperty]
    private string? _error;

    public void AddRow(string category, string result, string rolls)
    {
        Rows.Add(new ResultRowViewModel { Category = category, Result = result, Rolls = rolls });
    }
}

public class ResultRowViewModel
{
    public string? Category { get; set; }
    public string? Result { get; set; }
    public string? Rolls { get; set; }
}
