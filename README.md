# RollCraft

![NuGet](https://img.shields.io/nuget/v/RollCraft)
![NuGet](https://img.shields.io/nuget/dt/RollCraft)

A sophisticated library for creating dice expressions in .NET.

## Installation
```
dotnet add package RollCraft
```

## Features

### Error Handling

RollCraft uses the `Result` type to handle errors in a functional way. This uses the `LitePrimitives` library (which is another library of mine) which provides the `Result` type.

The rationale behind this decision is to avoid using exceptions, which can be expensive in terms of performance, especially since there's a lot of error handling in this library.

### Parsing

RollCraft can parse dice expressions through the use of strings into a `DiceExpression` object.

```csharp
using LitePrimitives;
using RollCraft;

Result<DiceExpression<int>> expression = DiceExpressionParser.Parse<int>("2d6 + 3");
```

RollCraft has the notion of specifying the type of the number result, which is done through generics.

***Note: Only `int` and `double` are supported at the moment.***

```csharp
using LitePrimitives;
using RollCraft;

Result<DiceExpression<double>> expression = DiceExpressionParser.Parse<double>("2d6 / 2.2");
```

### Evaluation

Basic usage of RollCraft involves creating a `DiceExpressionEvaluator` object and using it to evaluate a dice expression:

```csharp
using LitePrimitives;
using RollCraft;

DiceExpressionEvaluator evaluator = DiceExpresionEvaluator.CreateRandom();
Result<DiceExpressionResult<int>> result = evaluator.Evaluate("2d6 + 3");

result.Perform(
    success: (r) => Console.WriteLine(r),
    failure: (e) => Console.WriteLine($"Error: {e[0].Message}")
);
```

Besides providing the dice expression string directly, you can also use the `DiceExpression` object directly which has the benefit of already being parsed:

```csharp
using LitePrimitives;
using RollCraft;

DiceExpressionEvaluator evaluator = DiceExpresionEvaluator.CreateRandom();
Result<DiceExpression<int>> expression = DiceExpressionParser.Parse<int>("2d6 + 3");

Result<DiceExpressionResult<int>> result = evaluator.Evaluate(expression);
// Or
Result<DiceExpressionResult<int>> result = expression.Bind(e => evaluator.Evaluate(e));
```

The `DiceExpressionEvaluator` can be created with a variety of static constructors:

```csharp
CreateRandom() // Creates a roller that always rolls a random number on the dice
CreateSeededRandom(int seed) // Creates a roller that always rolls a random number on the dice with a seed
CreateMinimum() // Creates a roller that always returns the minimum value on the dice
CreateMaximum() // Creates a roller that always returns the maximum value on the dice
CreateFixedAverage() // Creates a roller that always returns the fixed average value of the dice
```
The `DiceExpressionResult` object contains the result of the evaluation, which includes the individual dice rolls and the final result. 
The number generic type that's used in parsing and evaluation will be the same type as the result.

### Expression Syntax

The syntax for dice expressions is as follows:

 - Arithmetic operators: `+`, `-`, `*`, `/`
 - Unary: `-`
 - Brackets: `(`, `)`
 - Dice: `NdM`, where `N` is the number of dice and `M` is the number of sides
 - Dice Modifiers:
   - `!`: Exploding (`4d6!` explodes on 6)
   - `k` or `kh`: Keep highest (`4d6kh3` keeps the highest 3 dice)
   - `r`: Reroll (`4d6r` keeps rerolling the dice on 1)
   - `ro`: Reroll once (`4d6ro` rerolls the dice on 1 once)
   - `min`: Minimum (`4d6min2` Rolls below 2 are treated as 2)
   - `max`: Maximum (`4d6max5` Rolls above 5 are treated as 5)
 - Compare operators (applies to Exploding, Reroll and Reroll once):
   - `<=`: Less than or equal to (4d6r<=2 rerolls the dice on 2 or less)
   - `<`: Less than (4d6r<3 rerolls the dice on 2 or less)
   - `>=`: Greater than or equal to (4d6!>=4 explodes on 4 or more)
   - `>`: Greater than (4d6!>4 explodes on more than 4)
   - `=`: Equal to (4d6r=4 rerolls the dice on 4)
   - `<>`: Not equal to (4d6r<>6 rerolls the dice on values other than 6)