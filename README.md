# RollCraft

![NuGet](https://img.shields.io/nuget/v/RollCraft)
![NuGet](https://img.shields.io/nuget/dt/RollCraft)

A sophisticated, high-performance dice expression parser and evaluator for .NET. RollCraft supports complex dice notation with modifiers, mathematical functions, variables, and conditional expressions.

## Installation

```bash
dotnet add package RollCraft
```

## Quick Start

```csharp
using RollCraft;

// Create an evaluator with random dice rolls
var evaluator = DiceExpressionEvaluator<int>.CreateRandom();

// Evaluate a dice expression
var result = evaluator.Evaluate("2d6 + 5");

result.Switch(
    onSuccess: r => Console.WriteLine($"Result: {r.Result}"),
    onFailure: e => Console.WriteLine($"Error: {e.Message}")
);
```

## Features

- **Dice Notation**: Standard dice syntax (`NdM`) with extensive modifier support
- **Math Operations**: Full arithmetic support with proper operator precedence
- **Dice Modifiers**: Exploding dice, keep highest/lowest, reroll, min/max clamping
- **Math Functions**: `floor`, `ceil`, `round`, `min`, `max`, `abs`, `sqrt`
- **Variables**: Dynamic variable substitution with `[VariableName]` syntax
- **Conditionals**: Ternary-style conditional expressions with `if(condition, trueValue, falseValue)`
- **Generic Numeric Types**: Support for `short`, `int`, `long`, `float`, `double`, and `decimal` result types
- **Functional Error Handling**: Uses `Result<TError, TValue>` pattern (no exceptions)
- **Multiple Roller Strategies**: Random, seeded random, minimum, maximum, fixed average, or custom

## Table of Contents

- [Parsing](#parsing)
- [Evaluation](#evaluation)
- [Expression Syntax](#expression-syntax)
  - [Basic Dice](#basic-dice)
  - [Arithmetic Operations](#arithmetic-operations)
  - [Dice Modifiers](#dice-modifiers)
  - [Math Functions](#math-functions)
  - [Variables](#variables)
  - [Conditional Expressions](#conditional-expressions)
- [Roller Types](#roller-types)
- [Custom Rollers](#custom-rollers)
- [Error Handling](#error-handling)
- [Results and Rolls](#results-and-rolls)

## Parsing

RollCraft parses dice expressions into a `DiceExpression<TNumber>` AST that can be evaluated multiple times.

```csharp
using RollCraft;

// Parse returns a Result type
var parseResult = DiceExpressionParser.Parse<int>("2d6 + 3");

parseResult.Switch(
    onSuccess: expression => Console.WriteLine("Parsed successfully!"),
    onFailure: error => Console.WriteLine($"Parse error: {error.Message}")
);
```

### TryParse Pattern

For a more traditional approach, use `TryParse`:

```csharp
ParserError? error = DiceExpressionParser.TryParse<int>("2d6 + 3", out var expression);

if (error is null)
{
    // Use expression
}
else
{
    Console.WriteLine($"Error: {error.Value.Message} at position {error.Value.Position}");
}
```

### Numeric Types

RollCraft supports multiple generic numeric types:

| Type | Description | Use Case |
|------|-------------|----------|
| `short` | 16-bit integer | Memory-constrained scenarios |
| `int` | 32-bit integer | Standard whole number results |
| `long` | 64-bit integer | Large whole number results |
| `float` | Single-precision floating-point | Decimal results with lower precision |
| `double` | Double-precision floating-point | Decimal results with high precision |
| `decimal` | 128-bit decimal | Financial/high-precision calculations |

```csharp
// Integer results (most common)
var intResult = DiceExpressionParser.Parse<int>("2d6 + 3");

// Double results (useful for division)
var doubleResult = DiceExpressionParser.Parse<double>("2d6 / 2.5");

// Decimal results (high precision)
var decimalResult = DiceExpressionParser.Parse<decimal>("1d100 / 3");

// Long results (large numbers)
var longResult = DiceExpressionParser.Parse<long>("100d1000");
```

## Evaluation

Create a `DiceExpressionEvaluator<TNumber>` to evaluate expressions:

```csharp
var evaluator = DiceExpressionEvaluator<int>.CreateRandom();

// Evaluate a string directly
var result = evaluator.Evaluate("4d6kh3"); // Roll 4d6, keep highest 3

// Or evaluate a pre-parsed expression (more efficient for repeated evaluations)
var expression = DiceExpressionParser.Parse<int>("4d6kh3");
var result = evaluator.Evaluate(expression.Value);
```

### TryEvaluate Pattern

```csharp
var evaluator = DiceExpressionEvaluator<double>.CreateRandom();

// For pre-parsed expressions, returns EvaluatorError?
EvaluatorError? error = evaluator.TryEvaluate(expression, out var result);

// For string expressions, returns IRollError? (can be ParserError or EvaluatorError)
IRollError? error = evaluator.TryEvaluate("2d6 + 3", out var result);
```

### Evaluating with Variables

```csharp
var evaluator = DiceExpressionEvaluator<int>.CreateRandom();
var variables = new Dictionary<string, int>
{
    ["STR"] = 5,
    ["Level"] = 10
};

var result = evaluator.Evaluate("1d20 + [STR] + [Level]", variables);
```

### Multiple Evaluations

Evaluate the same expression multiple times:

```csharp
var evaluator = DiceExpressionEvaluator<int>.CreateRandom();
var expression = DiceExpressionParser.Parse<int>("1d20").Value;

// Roll 10 times
var results = evaluator.Evaluate(expression, repeatCount: 10);

foreach (var result in results)
{
    result.Switch(
        onSuccess: r => Console.WriteLine(r.Result),
        onFailure: e => Console.WriteLine(e.Message)
    );
}
```

## Expression Syntax

### Basic Dice

| Syntax | Description | Example |
|--------|-------------|---------|
| `NdM` | Roll N dice with M sides | `2d6` (roll 2 six-sided dice) |
| `dM` | Roll 1 die with M sides | `d20` (roll 1 twenty-sided die) |

The number of dice and sides can be expressions:

```
(2+1)d6      → Roll 3d6
2d(4+2)     → Roll 2d6
[DICE]d[SIDES] → Use variables for dice count and sides
```

### Arithmetic Operations

| Operator | Description | Precedence |
|----------|-------------|------------|
| `+` | Addition | Low |
| `-` | Subtraction | Low |
| `*` | Multiplication | High |
| `/` | Division | High |
| `%` | Modulo (remainder) | High |
| `-` (unary) | Negation | Highest |
| `()` | Grouping | - |

Examples:
```
2d6 + 5         → Add 5 to the roll
2d6 * 2         → Double the roll
(2d6 + 3) * 2   → Add 3, then double
-1d6            → Negate the roll
10 % 3          → Remainder of 10/3 = 1
1d20 % 2        → 0 if even, 1 if odd
```

### Dice Modifiers

Modifiers are applied to dice expressions in the order they appear.

#### Exploding Dice (`!`)

Dice "explode" (roll again and add) when they hit the maximum value or a specified condition.

| Syntax | Description |
|--------|-------------|
| `4d6!` | Explode on maximum (6) |
| `4d6!=5` | Explode on exactly 5 |
| `4d6!>4` | Explode on greater than 4 |
| `4d6!>=4` | Explode on 4 or greater |
| `4d6!<3` | Explode on less than 3 |
| `4d6!<=3` | Explode on 3 or less |
| `4d6!<>1` | Explode on anything except 1 |

#### Keep Dice (`k`, `kh`, `kl`)

Keep only some of the rolled dice.

| Syntax | Description |
|--------|-------------|
| `4d6k3` | Keep highest 3 (same as `kh`) |
| `4d6kh3` | Keep highest 3 |
| `4d6kl3` | Keep lowest 3 |

#### Reroll (`r`, `ro`)

Reroll dice that meet a condition.

| Syntax | Description |
|--------|-------------|
| `4d6r` | Reroll 1s (indefinitely) |
| `4d6r=1` | Reroll 1s (indefinitely) |
| `4d6r<3` | Reroll 1s and 2s (indefinitely) |
| `4d6ro` | Reroll 1s (once only) |
| `4d6ro<=2` | Reroll 1s and 2s (once only) |

#### Minimum/Maximum Clamping (`min`, `max`)

Clamp individual die results to a minimum or maximum value.

| Syntax | Description |
|--------|-------------|
| `4d6min2` | Treat any roll below 2 as 2 |
| `4d6max5` | Treat any roll above 5 as 5 |
| `4d6min2max5` | Clamp rolls between 2 and 5 |

#### Combining Modifiers

Modifiers can be combined:

```
4d6kh3          → Roll 4d6, keep highest 3
4d6!kh3         → Roll 4d6 exploding, keep highest 3
4d6r<3kh3       → Roll 4d6, reroll <3, keep highest 3
4d6min2max5!k3  → Complex modifier chain
```

### Math Functions

All function names are case-insensitive.

| Function | Description | Example |
|----------|-------------|---------|
| `floor(x)` | Round down | `floor(3.7)` → 3 |
| `ceil(x)` | Round up | `ceil(3.2)` → 4 |
| `round(x)` | Round to nearest (banker's rounding) | `round(3.5)` → 4 |
| `abs(x)` | Absolute value | `abs(-5)` → 5 |
| `sqrt(x)` | Square root | `sqrt(16)` → 4 |
| `min(a, b, ...)` | Minimum value (2+ args) | `min(1d6, 1d8, 1d10)` |
| `max(a, b, ...)` | Maximum value (2+ args) | `max(1d6, 5)` |

Functions can be nested and combined with expressions:

```
floor(2d6 / 2)           → Roll 2d6, divide by 2, round down
max(1d6, 1d8)            → Roll both, take higher
min(1d20 + 5, 20)        → Cap result at 20
sqrt(abs(-16))           → Nested functions
```

> **Note**: `min()` and `max()` as functions require parentheses and commas. The modifier syntax (`1d6min3`) is different and clamps individual die values.

### Variables

Variables use bracket syntax `[VariableName]` and are case-insensitive.

```csharp
var variables = new Dictionary<string, int>
{
    ["STR"] = 18,
    ["ProfBonus"] = 3
};

// All of these work (case-insensitive):
// [STR], [str], [Str], [sTR]
evaluator.Evaluate("1d20 + [STR] + [ProfBonus]", variables);
```

Variables can be used anywhere a number is expected:

```
[DICE]d[SIDES]           → Variable dice count and sides
1d20 + [Modifier]        → Variable modifier
if([HP] <= 0, 0, [HP])   → Variables in conditionals
```

### Conditional Expressions

Use `if(condition, trueValue, falseValue)` for conditional logic.

**Syntax**: `if(left OPERATOR right, valueIfTrue, valueIfFalse)`

**Comparison Operators**:

| Operator | Description |
|----------|-------------|
| `=` | Equal |
| `<>` | Not equal |
| `>` | Greater than |
| `>=` | Greater than or equal |
| `<` | Less than |
| `<=` | Less than or equal |

**Examples**:

```
if(1d20 >= 10, 2d6, 1d6)           → Critical hit logic
if([HP] <= 0, 0, [HP])             → Clamp HP to minimum 0
if(1d20 = 20, 2 * 2d6, 2d6)        → Double damage on nat 20
if(1d6 > 1d6, 1, 0)                → Compare two rolls
```

Conditionals can be nested:

```
if([Level] >= 5, if([Level] >= 11, 3d6, 2d6), 1d6)  → Scaling damage
```

## Roller Types

Create evaluators with different rolling strategies:

```csharp
// Random rolls (default for games)
var random = DiceExpressionEvaluator<int>.CreateRandom();

// Seeded random (reproducible results)
var seeded = DiceExpressionEvaluator<int>.CreateSeededRandom(42);

// Always roll minimum (1)
var minimum = DiceExpressionEvaluator<int>.CreateMinimum();

// Always roll maximum
var maximum = DiceExpressionEvaluator<int>.CreateMaximum();

// Fixed average ((min + max) / 2, rounded down)
var average = DiceExpressionEvaluator<int>.CreateFixedAverage();
```

## Custom Rollers

Implement `IRoller` for custom rolling behavior:

```csharp
public class LoadedDiceRoller : IRoller
{
    public int RollDice(int dieSize)
    {
        // Always roll high!
        return dieSize - 1 + Random.Shared.Next(1, 3);
    }
}

var evaluator = DiceExpressionEvaluator<int>.CreateCustom(new LoadedDiceRoller());
```

## Error Handling

RollCraft uses the `Result<TError, TValue>` pattern from the [MonadCraft](https://github.com/hquinn/MonadCraft) library for functional error handling without exceptions.

### Error Types

| Error Type | Description |
|------------|-------------|
| `ParserError` | Syntax errors during parsing (includes position) |
| `EvaluatorError` | Runtime errors during evaluation |

### Handling Results

```csharp
var result = evaluator.Evaluate("2d6 + 3");

// Pattern matching with Switch
result.Switch(
    onSuccess: r => Console.WriteLine($"Result: {r.Result}"),
    onFailure: e => Console.WriteLine($"Error: {e.Message}")
);

// Async version
await result.SwitchAsync(
    onSuccess: async r => await SaveResultAsync(r),
    onFailure: async e => await LogErrorAsync(e)
);

// Check success/failure
if (result.IsSuccess)
{
    var value = result.Value;
}

// Chain operations with Bind
var finalResult = parseResult.Bind(expr => evaluator.Evaluate(expr));
```

### Common Errors

**Parser Errors**:
- Invalid token
- Unexpected end of input
- Missing parenthesis
- Invalid function arguments

**Evaluator Errors**:
- Division by zero
- Non-integer dice count/sides
- Invalid modifier values
- Undefined variables
- Negative sqrt argument

## Results and Rolls

The `DiceExpressionResult<TError, TNumber>` contains both the final result and detailed roll information.

```csharp
var result = evaluator.Evaluate("4d6kh3");

result.Switch(
    onSuccess: r =>
    {
        Console.WriteLine($"Total: {r.Result}");
        
        foreach (var roll in r.Rolls)
        {
            Console.WriteLine($"  d{roll.Sides}: {roll.Roll} {GetModifiers(roll.Modifier)}");
        }
    },
    onFailure: e => Console.WriteLine(e.Message)
);

string GetModifiers(DiceModifier mod)
{
    var parts = new List<string>();
    if ((mod & DiceModifier.Dropped) != 0) parts.Add("dropped");
    if ((mod & DiceModifier.Exploded) != 0) parts.Add("exploded");
    if ((mod & DiceModifier.Rerolled) != 0) parts.Add("rerolled");
    if ((mod & DiceModifier.Minimum) != 0) parts.Add("min-clamped");
    if ((mod & DiceModifier.Maximum) != 0) parts.Add("max-clamped");
    return parts.Count > 0 ? $"({string.Join(", ", parts)})" : "";
}
```

### DiceModifier Flags

| Flag | Description |
|------|-------------|
| `None` | No modifier applied |
| `Minimum` | Roll was clamped to minimum |
| `Maximum` | Roll was clamped to maximum |
| `Exploded` | Roll triggered an explosion |
| `Dropped` | Roll was dropped (keep modifier) |
| `Rerolled` | Roll was rerolled |

## License

MIT License - see [LICENSE](LICENSE) for details.