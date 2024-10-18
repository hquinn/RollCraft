using LitePrimitives;
using RollCraft;
using Spectre.Console;

var randomEvaluator = DiceExpressionEvaluator<double>.CreateRandom();
var minimumEvaluator = DiceExpressionEvaluator<double>.CreateMinimum();
var maximumEvaluator = DiceExpressionEvaluator<double>.CreateMaximum();
var fixedAverageEvaluator = DiceExpressionEvaluator<double>.CreateFixedAverage();

bool doYouWishToContinue;

do
{
   AnsiConsole.Write(
      new FigletText("RollCraft")
         .LeftJustified()
         .Color(Color.Red));

   var diceExpressionInput = AnsiConsole.Prompt(
      new TextPrompt<string>("Please enter the [green]dice expression[/] to evaluate:")
         .PromptStyle(new Style().Foreground(Color.Yellow))
         .Validate(n =>
         {
            if (string.IsNullOrWhiteSpace(n))
            {
               return ValidationResult.Error("Cannot have an empty dice expression");
            }

            return ValidationResult.Success();
         }));

   var diceExpression = DiceExpressionParser.Parse<double>(diceExpressionInput);

   Result<Grid>.Success(CreateGrid())
      .Bind(grid => EvaluateFormatResult(grid, "Result", randomEvaluator, diceExpression))
      .Bind(grid => EvaluateFormatResult(grid, "Minimum", minimumEvaluator, diceExpression))
      .Bind(grid => EvaluateFormatResult(grid, "Maximum", maximumEvaluator, diceExpression))
      .Bind(grid => EvaluateFormatResult(grid, "Fixed Average", fixedAverageEvaluator, diceExpression))
      .Perform(
         success: grid =>
         {
            grid.AddEmptyRow();
            AnsiConsole.Write(grid);
         },
         failure: errors => AnsiConsole.Write(new Markup($"[red]Failed:[/] [yellow]{errors.Message}[/]{Environment.NewLine}{Environment.NewLine}")));
   
   doYouWishToContinue = AnsiConsole.Prompt(
      new TextPrompt<bool>("Do you wish to continue?")
         .AddChoice(true)
         .AddChoice(false)
         .DefaultValue(true)
         .WithConverter(choice => choice ? "y" : "n"));

   if (doYouWishToContinue)
   {
      AnsiConsole.Clear();
   }
} while (doYouWishToContinue);

static Result<Grid> EvaluateFormatResult(
   Grid grid,
   string category,
   DiceExpressionEvaluator<double> evaluator,
   Result<DiceExpression<double>> expression)
{
   return evaluator.Evaluate(expression)
      .Map<Grid>(r =>
      {
         grid.AddRow(
            new Text(category, new Style(Color.Blue)), 
            new Text(r.Result.ToString("0.######"), new Style(Color.Yellow)), 
            new Markup($"[red][[[/] [green]{string.Join(", ", r.Rolls)}[/] [red]]][/]"));

         return grid;
      });
}

static Grid CreateGrid()
{
   var grid = new Grid();
   grid.AddColumns(3);
   grid.AddEmptyRow();
   
   return grid;
}